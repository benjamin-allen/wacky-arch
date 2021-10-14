using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using Console = SadConsole.Console;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;
using System.Linq;

namespace Emulator.UIComponents
{
	public class TextBox : ScrollingConsole
	{
		private int width, height;
		private List<string> text;
		public string Text 
		{
			get { return string.Join("\n", text); }
		}
		public int ScrollOffset { get; set; }

		private int blinkTimeMs;
		private int timeSinceBlinkSwitchMs;
		private ColoredGlyph cursorGlyph;
		private bool showCursor;
		private Point cursor;

		private static List<Keys> typeableKeys = new List<Keys>()
		{ Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I,
		  Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R,
		  Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z,
		  Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9,
		  Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4,
		  Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9,
		  Keys.Space, Keys.OemPeriod, Keys.OemPlus, Keys.OemMinus, Keys.OemPipe, Keys.OemOpenBrackets, Keys.OemOpenBrackets,
		  Keys.OemCloseBrackets, Keys.OemTilde, Keys.OemQuestion, Keys.OemComma, Keys.OemSemicolon, Keys.OemQuotes};

		public TextBox(int width, int height) : base(width, height)
		{
			this.width = width;
			this.height = height;


			Cursor.SetPrintAppearance(new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark));

			cursor = new Point(0, 0);
			cursorGlyph = new ColoredGlyph(0, Color.Green, Color.Green);
			showCursor = true;
			blinkTimeMs = 500;
			timeSinceBlinkSwitchMs = 0;

			text = new List<string>() { "" };
		}

		public override void Draw(TimeSpan timeElapsed)
		{
			base.Draw(timeElapsed);
			Clear();
			timeSinceBlinkSwitchMs += (int)timeElapsed.TotalMilliseconds;

			for (int i = 0; i < Height - 1 && i < text.Count; i++)
			{
				Print(0, i, text[i+ScrollOffset], Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);
			}

			if (timeSinceBlinkSwitchMs >= blinkTimeMs)
			{
				timeSinceBlinkSwitchMs %= blinkTimeMs;
				showCursor = !showCursor;
			}
			if (showCursor)
			{ 
				Print(cursor.X, cursor.Y-ScrollOffset, cursorGlyph);
			}
		}

		public override bool ProcessKeyboard(SadConsole.Input.Keyboard info)
		{
			if (MoveCursor(info) || TypeCharacter(info))
			{
				showCursor = true;
				timeSinceBlinkSwitchMs = 0;
				return true;
			}

			return false;
		}

		public void ScrollTo(int line)
		{
			if (line < ScrollOffset)
			{
				ScrollOffset = line;
				cursor.Y = line;
				showCursor = false;
			}
			else if (line > Height + ScrollOffset - 2)
			{
				ScrollOffset = line - Height + 2;
				cursor.Y = line;
				showCursor = false;
			}
		}

		private bool MoveCursor(SadConsole.Input.Keyboard info)
		{
			if (info.IsKeyPressed(Keys.Up))
			{
				if (cursor.Y == 0)
					cursor.X = 0;
				if (cursor.Y > 0)
				{
					cursor.Y -= 1;
					if (cursor.Y < ScrollOffset)
						ScrollOffset -= 1;
				}
				if (cursor.X > text[cursor.Y].Length)
					cursor.X = text[cursor.Y].Length;
				return true;
			}
			if (info.IsKeyPressed(Keys.Down))
			{
				if (cursor.Y == text.Count - 1)
					cursor.X = text[cursor.Y].Length;
				if (cursor.Y < text.Count - 1)
				{
					cursor.Y += 1;
					if (cursor.Y > ScrollOffset + Height - 2)
						ScrollOffset += 1;
				}
				if (cursor.X > text[cursor.Y].Length)
					cursor.X = text[cursor.Y].Length;
				return true;
			}
			if (info.IsKeyPressed(Keys.Left))
			{
				if (cursor.X > 0)
					cursor.X -= 1;
				return true;
			}
			if (info.IsKeyPressed(Keys.Right))
			{
				if (cursor.X < text[cursor.Y].Length)
					cursor.X += 1;
				return true;
			}
			if (info.IsKeyPressed(Keys.Home))
			{
				cursor.X = 0;
				return true;
			}
			if (info.IsKeyPressed(Keys.End))
			{
				cursor.X = text[cursor.Y].Length;
				return true;
			}


			return false;
		}
	
		private bool TypeCharacter(SadConsole.Input.Keyboard info)
		{
			
			bool returnValue = false;

			// Refactor to use info.KeysPressed. Durh.
			foreach (AsciiKey key in info.KeysPressed)
			{
				if (typeableKeys.Contains(key.Key))
				{
					insertCharacter(key.Character.ToString());
					returnValue = true;
				}

				if (key.Key == Keys.Enter)
				{
					string fullLine = text[cursor.Y];
					text[cursor.Y] = fullLine.Substring(0, cursor.X);
					text.Insert(cursor.Y + 1, fullLine.Substring(cursor.X));
					cursor.Y += 1;
					cursor.X = 0;
					returnValue = true;
					if (text.Count >= height)
					{
						// scroll the console until the cursor is in view
						ScrollOffset += 1;
					}
				}

				if (key.Key == Keys.Back)
				{
					if (cursor.X == 0 && cursor.Y == 0)
						continue;
					if (cursor.X == 0)
					{
						cursor.X = text[cursor.Y - 1].Length;
						text[cursor.Y - 1] = text[cursor.Y - 1] + text[cursor.Y];
						text.RemoveAt(cursor.Y);
						cursor.Y -= 1;
						if (ScrollOffset > 0)
						{
							ScrollOffset -= 1;
						}
						returnValue = true;
					}
					else if (cursor.X < text[cursor.Y].Length)
					{
						text[cursor.Y] = text[cursor.Y].Substring(0, cursor.X - 1) + text[cursor.Y].Substring(cursor.X);
						cursor.X -= 1;
						returnValue = true;
					}
					else
					{
						text[cursor.Y] = text[cursor.Y].Substring(0, cursor.X - 1);
						cursor.X -= 1;
						returnValue = true;
					}
				}

				if (key.Key == Keys.Delete)
				{
					if (cursor.X == 0 && cursor.Y == text.Count - 1)
						continue;
					if (cursor.X == text[cursor.Y].Length)
					{
						text[cursor.Y] = text[cursor.Y] + text[cursor.Y + 1];
						text.RemoveAt(cursor.Y + 1);
						if (ScrollOffset > 0)
						{
							ScrollOffset -= 1;
						}
						returnValue = true;
					}
					else
					{
						text[cursor.Y] = text[cursor.Y].Substring(0, cursor.X) + text[cursor.Y].Substring(cursor.X + 1);
						returnValue = true;
					}
				}
			}

			return returnValue;

			void insertCharacter(string character)
			{
				if(cursor.X < width-1)
				{
					text[cursor.Y] = text[cursor.Y].Insert(cursor.X, character);
					cursor.X += 1;
				}
			}
		}
	}
}
