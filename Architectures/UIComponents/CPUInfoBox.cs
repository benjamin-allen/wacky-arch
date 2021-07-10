using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emulator.UIComponents
{
	public class CPUInfoBox : ScrollingConsole
	{
		private int width = 16;
		private int height = 9;
		private CPU.CPU cpu;

		public CPUInfoBox(CPU.CPU cpu) : base(16, 9)
		{
			this.cpu = cpu;
		}

		public override void Draw(TimeSpan timeElapsed)
		{
			base.Draw(timeElapsed);
			Clear();
			DrawBox(new Microsoft.Xna.Framework.Rectangle(0, 0, width, height), new Cell(Color.White, Color.Black, '-'), null, CellSurface.ConnectedLineThin);

			Print(5, 1, "DECI | HEX");
			Print(1, 2, $"R0 {String.Format("{0,5:####0}", cpu.Registers[0].Data.Value)} | {cpu.Registers[0].Data.Value & 0xFFF:X3}");
			Print(1, 3, $"R1 {String.Format("{0,5:####0}", cpu.Registers[1].Data.Value)} | {cpu.Registers[1].Data.Value & 0xFFF:X3}");
			Print(1, 4, $"R2 {String.Format("{0,5:####0}", cpu.Registers[2].Data.Value)} | {cpu.Registers[2].Data.Value & 0xFFF:X3}");
			Print(1, 5, $"C  {String.Format("{0,5:####0}", cpu.Registers[3].Data.Value)} | {cpu.Registers[3].Data.Value & 0xFFF:X3}");
			Print(1, 7, $"CPU: {(cpu.IsHalted ? "  HALTED" : (cpu.IsErrored ? "   ERROR" : (cpu.PcLineMap.Count > 0 ? "   READY" : "")))}");
		}
	}
}
