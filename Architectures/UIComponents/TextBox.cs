using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using Console = SadConsole.Console;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;

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
		  Keys.Space };

		public TextBox(int width, int height) : base(width, height)
		{
			this.width = width;
			this.height = height;

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

			for (int i = 0; i < text.Count; i++)
			{
				Print(0, i, text[i], Color.White, Color.Black);
			}

			if (timeSinceBlinkSwitchMs >= blinkTimeMs)
			{
				timeSinceBlinkSwitchMs %= blinkTimeMs;
				showCursor = !showCursor;
			}
			if (showCursor)
			{ 
				Print(cursor.X, cursor.Y, cursorGlyph);
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

		private bool MoveCursor(SadConsole.Input.Keyboard info)
		{
			if (info.IsKeyPressed(Keys.Up))
			{
				if (cursor.Y == 0)
					cursor.X = 0;
				if (cursor.Y > 0)
					cursor.Y -= 1;
				if (cursor.X > text[cursor.Y].Length)
					cursor.X = text[cursor.Y].Length;
				return true;
			}
			if (info.IsKeyPressed(Keys.Down))
			{
				if (cursor.Y == text.Count - 1)
					cursor.X = text[cursor.Y].Length;
				if (cursor.Y < text.Count - 1)
					cursor.Y += 1;
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
			// need special handling for 
			if (info.IsKeyDown(Keys.LeftShift) || info.IsKeyDown(Keys.RightShift))
			{
				if (info.IsKeyPressed(Keys.D3))
				{
					insertCharacter("#");
					return true;
				}
				if (info.IsKeyPressed(Keys.D2))
				{
					insertCharacter("@");
					return true;
				}
			}

			// Refactor to use info.KeysPressed. Durh.
			foreach (Keys key in typeableKeys)
			{
				if (info.IsKeyPressed(key))
				{
					insertCharacter(((char)key).ToString());
					return true;
				}
			}

			if (info.IsKeyPressed(Keys.Enter))
			{
				if(text.Count < height-1)
				{
					string fullLine = text[cursor.Y];
					text[cursor.Y] = fullLine.Substring(0, cursor.X);
					text.Insert(cursor.Y + 1, fullLine.Substring(cursor.X));
					cursor.Y += 1;
					cursor.X = 0;
					return true;
				}
			}

			if (info.IsKeyPressed(Keys.Back))
			{
				if (cursor.X == 0 && cursor.Y == 0)
					return false;
				if (cursor.X == 0)
				{
					cursor.X = text[cursor.Y - 1].Length;
					text[cursor.Y - 1] = text[cursor.Y - 1] + text[cursor.Y];
					text.RemoveAt(cursor.Y);
					cursor.Y -= 1;
					return true;
				}
				if (cursor.X < text[cursor.Y].Length)
				{
					text[cursor.Y] = text[cursor.Y].Substring(0, cursor.X - 1) + text[cursor.Y].Substring(cursor.X);
					cursor.X -= 1;
					return true;
				}
				else
				{
					text[cursor.Y] = text[cursor.Y].Substring(0, cursor.X - 1);
					cursor.X -= 1;
					return true;
				}
			}

			if(info.IsKeyPressed(Keys.Delete))
			{
				if (cursor.X == 0 && cursor.Y == text.Count - 1)
					return false;
				if (cursor.X == text[cursor.Y].Length)
				{
					text[cursor.Y] = text[cursor.Y] + text[cursor.Y + 1];
					text.RemoveAt(cursor.Y + 1);
					return true;
				}
				else
				{
					text[cursor.Y] = text[cursor.Y].Substring(0, cursor.X) + text[cursor.Y].Substring(cursor.X + 1);
					return true;
				}
			}

			return false;

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
