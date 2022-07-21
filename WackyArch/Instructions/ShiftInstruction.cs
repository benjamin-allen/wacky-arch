using WackyArch.Components;
using WackyArch.CPUs;

namespace WackyArch.Instructions
{
    public class ShiftInstruction : Instruction
	{
		protected int ShiftCode;
		protected int X;
		protected int ShiftAmt;

		public ShiftInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			ShiftCode = (word.Value & 0b0000_1100_0000) >> 6;
			X = (word.Value & 0b0000_0011_0000) >> 4;
			ShiftAmt = word.Value & 0b0000_0000_1111;
		}

		public override void Execute()
		{
			if(Opcode == 1)
			{
				// Shift Instruction
				if(ShiftCode < 2)
				{
					// Left shift
					Cpu.Registers[X].Data.Value <<= ShiftAmt;
				}
				else if(ShiftCode == 2)
				{
					// Right shift (logical)
					int intermediate = Cpu.Registers[X].Data.Value >> ShiftAmt;
					intermediate = intermediate & ((1 << Word.Size - ShiftAmt) - 1);
					Cpu.Registers[X].Data.Value = Utilities.Utilities.SignExtend(intermediate, Word.Size - 1);
				}
				else
				{
					// Right shift (arithmetic)
					Cpu.Registers[X].Data.Value >>= ShiftAmt;
				}
			}
			else
			{
				if(ShiftCode < 2)
				{
					// Rotate left
					// Get upper bits of the value
					int mask = Utilities.Utilities.SignExtend(0b1000_0000_0000, 11) >> (ShiftAmt - 1);
					int upper = Cpu.Registers[X].Data.Value & mask;
					upper >>= Word.Size - (ShiftAmt);
					upper &= ((1 << ShiftAmt) - 1);
					Cpu.Registers[X].Data.Value <<= ShiftAmt;
					Cpu.Registers[X].Data.Value += upper;
				}
				else
				{
					// Rotate right
					int lower = Cpu.Registers[X].Data.Value & ((1 << ShiftAmt) - 1);
					int newUpperBits = lower << Word.Size - ShiftAmt;
					int newValue = Cpu.Registers[X].Data.Value >> ShiftAmt;
					newValue &= ((1 << (Word.Size - ShiftAmt)) - 1); // Keep only the lower bits
					newValue |= newUpperBits;
					Cpu.Registers[X].Data.Value = newValue;
				}
			}
		}
	}
}
