using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emulator.UIComponents
{
	public class CPUInfoBox : ScrollingConsole
	{
		private int width = 19;
		private int height = 9;
		private CPU.CPU cpu;

		public CPUInfoBox(CPU.CPU cpu) : base(19, 9)
		{
			this.cpu = cpu;
			DefaultForeground = Emulator.EmulatorColors.Text;
			DefaultBackground = Emulator.EmulatorColors.ControlBackDark;
			Cursor.SetPrintAppearance(new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark));
		}

		public override void Draw(TimeSpan timeElapsed)
		{
			base.Draw(timeElapsed);
			Clear();
			DrawBox(new Microsoft.Xna.Framework.Rectangle(0, 0, width, height), new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, '-'), null, CellSurface.ConnectedLineThin);

			Print(1, 1, "       DECI | HEX", Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);
			Print(1, 2, $"r0    {String.Format("{0,5:####0}", cpu.Registers[0].Data.Value)} | {cpu.Registers[0].Data.Value & 0xFFF:X3}", Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);
			Print(1, 3, $"r1    {String.Format("{0,5:####0}", cpu.Registers[1].Data.Value)} | {cpu.Registers[1].Data.Value & 0xFFF:X3}", Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);
			Print(1, 4, $"r2    {String.Format("{0,5:####0}", cpu.Registers[2].Data.Value)} | {cpu.Registers[2].Data.Value & 0xFFF:X3}", Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);
			Print(1, 5, $"const {String.Format("{0,5:####0}", cpu.Registers[3].Data.Value)} | {cpu.Registers[3].Data.Value & 0xFFF:X3}", Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);
			Print(1, 7, $"CPU: {(cpu.IsHalted ? "  HALTED" : (cpu.IsErrored ? "   ERROR" : (cpu.PcLineMap.Count > 0 ? "   READY" : "")))}", Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);

			// Codebox "connector"
			SetGlyph(0, 6, 221, Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);
		}
	}
}
