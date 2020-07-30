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

		public JumpInstruction(CPU cpu, Word word) : base(cpu, word)
		{
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
					Cpu.OffsetPCValue(Offset);
					break;
				case 0b0110:
					// Jump if Eq0
					if(Cpu.Const.Data.Value == 0)
					{
						Cpu.OffsetPCValue(Offset);
					}
					break;
				case 0b0111:
					// Jump if Gr0
					if(Cpu.Const.Data.Value > 0)
					{
						Cpu.OffsetPCValue(Offset);
					}
					break;
				case 0b1000:
					// Jump if Le0
					if(Cpu.Const.Data.Value < 0)
					{
						Cpu.OffsetPCValue(Offset);
					}
					break;
				case 0b1001:
					// Jump address
					Cpu.SetPCValue(Cpu.Registers[X].Data.Value);
					break;
				default:
					throw new InvalidOperationException("Invalid Opcode!");
			}
		}
	}
}
