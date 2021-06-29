using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emulator.UIComponents
{
	public class CodeBox : ScrollingConsole
	{
		private TextBox textBox;
		private int gutterWidth;

		public string Status { get; set; }
		public int ErrorLine = -1;

		/// <summary>
		/// Create a codebox with the specified width and height. The typeable area will
		/// be slightly smaller.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public CodeBox(int width, int height) : base(width, height)
		{
			gutterWidth = 3;
			if (height - 2 >= Math.Pow(10, gutterWidth - 1)) 
				throw new ArgumentException("Too many lines to display line numbers");

			textBox = new TextBox(width - 4 - gutterWidth, height - 4);
			textBox.Position = new Point(gutterWidth + 3, 1);
			textBox.Parent = this;

			Status = "";
		}

		public string Text
		{
			get { return textBox.Text; }
		}

		public override void Draw(TimeSpan timeElapsed)
		{
			base.Draw(timeElapsed);
			Clear();
			textBox.Draw(timeElapsed);

			// Draw the gutter and surrounding box
			DrawBox(new Rectangle(0, 0, Width, Height), new Cell(Color.White, Color.Black, '-'), null, CellSurface.ConnectedLineThin);
			DrawLine(new Point(gutterWidth + 1, 0), new Point(gutterWidth + 1, textBox.Height), Color.White, Color.Black, 179);
			DrawLine(new Point(0, textBox.Height), new Point(Width, textBox.Height), Color.White, Color.Black, 196);
			SetGlyph(gutterWidth + 1, 0, 194);
			SetGlyph(gutterWidth + 1, textBox.Height, 193);
			SetGlyph(0, textBox.Height, 195);
			SetGlyph(Width - 1, textBox.Height, 180);

			// Draw Line numbers
			for(int i = 1; i < textBox.Text.Split("\n").Length + 1; i++)
			{
				int y = i;
				int x = textBox.Position.X - 3 - (int)Math.Log10(i);
				if (ErrorLine == i)
				{
					Print(x, y, i.ToString(), Color.Red);
				}
				else
				{
					Print(x, y, i.ToString(), Color.White);
				}	
			}

			// Draw Status text
			Cursor.Move(2, this.textBox.Height + 1).Print(Status);

			// Disabled for now. I can't figure out a good way to make this work with labels and comments
			/*
			// Draw a pointer for the current instruction, if not halted
			if (cpu.IsHalted == false)
			{
				var pc = cpu.GetPCValue();
				SetGlyph(Width - 1, pc + 1, 17);
				SetGlyph(gutterWidth + 1, pc + 1, 16);
			}
			*/
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			return textBox.ProcessKeyboard(info);
		}
	}
}
