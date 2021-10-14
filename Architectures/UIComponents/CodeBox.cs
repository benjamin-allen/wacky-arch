﻿using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emulator.UIComponents
{
	public class CodeBox : ScrollingConsole
	{
		private TextBox textBox;
		private int gutterWidth;
		private CPU.CPU cpu;
		private ScrollingConsole statusConsole;

		public string Status { get; set; }
		public int ErrorLine { get; set; } = -1;
		public bool CanType { get; set; }

		/// <summary>
		/// Create a codebox with the specified width and height. The typeable area will
		/// be slightly smaller.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		public CodeBox(int width, int height, CPU.CPU cpu) : base(width, height)
		{
			gutterWidth = 3;
			if (height - 2 >= Math.Pow(10, gutterWidth - 1)) 
				throw new ArgumentException("Too many lines to display line numbers");
			DefaultForeground = Emulator.EmulatorColors.Text;
			DefaultBackground = Emulator.EmulatorColors.ControlBackDark;

			textBox = new TextBox(width - 4 - gutterWidth, height - 4);
			textBox.Position = new Point(gutterWidth + 3, 1);
			textBox.Parent = this;
			Cursor.SetPrintAppearance(new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark));

			Status = "";

			this.cpu = cpu;

			statusConsole = new ScrollingConsole(width - 4, 2);
			statusConsole.Position = new Point(1, Height - 3);
			statusConsole.Parent = this;
			statusConsole.Cursor.SetPrintAppearance(new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark));
		}

		public string Text
		{
			get { return textBox.Text; }
		}

		public override void Draw(TimeSpan timeElapsed)
		{
			base.Draw(timeElapsed);
			Clear();
			// Draw a pointer for the next instruction instruction, if not halted
			if (cpu.IsHalted == false && cpu.PcLineMap.Count > 0)
			{
				var pc = cpu.GetPCValue();
				var lineIndex = cpu.PcLineMap.ContainsKey(pc) ? cpu.PcLineMap[pc] : Math.Max(cpu.PcLineMap.Max(kv => kv.Value) + 1, textBox.Text.Split("\n").Length);
				if (cpu.PcLineMap.ContainsKey(pc))
					textBox.ScrollTo(lineIndex);
				SetGlyph(Width - 2, lineIndex + 1 - textBox.ScrollOffset, 17);
				SetGlyph(gutterWidth + 2, lineIndex + 1 - textBox.ScrollOffset, 16);
			}
			textBox.Draw(timeElapsed);

			// Draw the gutter and surrounding box
			DrawBox(new Rectangle(0, 0, Width, Height), new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, '-'), null, CellSurface.ConnectedLineThin);
			DrawLine(new Point(gutterWidth + 1, 0), new Point(gutterWidth + 1, textBox.Height), Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, 179);
			DrawLine(new Point(0, textBox.Height), new Point(Width, textBox.Height), Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, 196);
			SetGlyph(gutterWidth + 1, 0, 194);
			SetGlyph(gutterWidth + 1, textBox.Height, 193);
			SetGlyph(0, textBox.Height, 195);
			SetGlyph(Width - 1, textBox.Height, 180);

			// Draw Line numbers
			for(int i = 1; i < textBox.Text.Split("\n").Length + 1 && i < textBox.Height; i++)
			{
				int y = i;
				int j = i + textBox.ScrollOffset;
				int x = textBox.Position.X - 3 - (int)Math.Log10(j);
				if (ErrorLine == j)
				{
					Print(x, y, j.ToString(), Color.Red);
				}
				else
				{
					Print(x, y, j.ToString(), Emulator.EmulatorColors.Text);
				}	
			}

			// draw pipe "connections"
			SetGlyph(0, 9, 221);
			SetGlyph(0, 10, 221);
			SetGlyph(0, 11, 221);
			SetGlyph(0, 17, 221);
			SetGlyph(0, 18, 221);
			SetGlyph(0, 19, 221);
			SetGlyph(Width - 1, 18, 222);
			SetGlyph(Width - 1, 19, 222);
			SetGlyph(Width - 1, 20, 222);

			// Draw cpubox "connectors"
			SetGlyph(Width - 1, 6, 222);
			SetGlyph(Width - 1, 10, 222);

			// Draw Status text
			statusConsole.Clear();
			statusConsole.Cursor.Move(2, this.textBox.Height + 1).Print(Status);
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			return textBox.ProcessKeyboard(info);
		}
	}
}
