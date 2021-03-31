using Microsoft.Xna.Framework;
using SadConsole;
using Console = SadConsole.Console;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole.Input;

namespace Emulator.UIComponents
{
	public class CodeBox : ScrollingConsole
	{
		private int width, height;
		private string[] text; 
		public string[] Text { get; }

		private TimeSpan blinkTime;
		private ColoredGlyph cursorGlyph;
		public Point cursor;
		

		public CodeBox(int width, int height) : base(width, height)
		{
			this.width = width;
			this.height = height;

			cursor = new Point(0, 0);
			cursorGlyph = new ColoredGlyph(0, Color.Green, Color.Green);
			blinkTime = new TimeSpan(0, 0, 0, 0, 500);

			text = new string[] { "Hello, world!", "Test text!" };
		}

		public override void Draw(TimeSpan timeElapsed)
		{
			base.Draw(timeElapsed);

			for(int i = 0; i < text.Length; i++)
			{
				Print(0, i, text[i], Color.Red, Color.Black);
			}

			Print(cursor.X, cursor.Y, cursorGlyph);
		}
	}
}
