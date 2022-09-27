using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;

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

		public override string Disassemble()
		{
			switch (ShiftCode)
            {
				case 0:
				case 1:
					return Tokens.ShiftLeft.Canonical + " " + Cpu.Registers[X].Name + " " + ShiftAmt.ToString();
				case 2:
					return Tokens.ShiftRight.Canonical + " " + Cpu.Registers[X].Name + " " + ShiftAmt.ToString();
				case 3:
					return Tokens.ShiftRightArithmetic.Canonical + " " + Cpu.Registers[X].Name + " " + ShiftAmt.ToString();
				default:
					throw new ComponentException($"Cannot disassemble shift instruction with shiftcode {ShiftCode}.", $"Invalid ShiftCode: {ShiftCode}");
			}
		}
	}
}
