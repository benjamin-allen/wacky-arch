using Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPU.Instructions
{
	public class JumpInstruction : Instruction
	{
		protected int Offset;
		protected int X;
		CPU cpu;

		public JumpInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			this.cpu = cpu;
			Offset = word.Value & 0xFF;
			Offset = unchecked((sbyte)Offset);
			X = word.Value & (0b0000_0000_0011);
		}

		public override void Execute()
		{
			switch(Opcode)
			{
				case 0b0101:
					// Jump
					cpu.OffsetPCValue(Offset);
					break;
				case 0b0110:
					// Jump if Eq0
					if(cpu.Const.Data.Value == 0)
					{
						cpu.OffsetPCValue(Offset);
					}
					break;
				case 0b0111:
					// Jump if Gr0
					if(cpu.Const.Data.Value > 0)
					{
						cpu.OffsetPCValue(Offset);
					}
					break;
				case 0b1000:
					// Jump if Le0
					if(cpu.Const.Data.Value < 0)
					{
						cpu.OffsetPCValue(Offset);
					}
					break;
				case 0b1001:
					// Jump address
					cpu.SetPCValue(cpu.Registers[X].Data.Value);
					break;
				default:
					throw new InvalidOperationException("Invalid Opcode!");
			}
		}
	}
}
