using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;

namespace WackyArch.Instructions
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
					Cpu.IncrementPC = false; // Don't increment the PC until the end next cycle
					break;
				case 0b0110:
					// Jump if Eq0
					if(Cpu.Const.Data.Value == 0)
					{
						goto case 0b0101;
					}
					break;
				case 0b0111:
					// Jump if Gr0
					if(Cpu.Const.Data.Value > 0)
					{
						goto case 0b0101;
					}
					break;
				case 0b1000:
					// Jump if Le0
					if(Cpu.Const.Data.Value < 0)
					{
						goto case 0b0101;
					}
					break;
				case 0b1001:
					// Jump address
					Cpu.SetPCValue(Cpu.Registers[X].Data.Value);
					Cpu.IncrementPC = false;
					break;
				default:
					throw new ComponentException($"Invalid Opcode {Opcode}!", $"Invalid Opcode {Opcode}!");
			}
		}

		public override string Disassemble()
		{
			var opcodeMap = new Dictionary<int, Token> {
				{ 5, Tokens.Jump }, { 6, Tokens.JumpIfZero }, { 7, Tokens.JumpIfGreater },
				{ 8, Tokens.JumpIfLesser }, { 9, Tokens.JumpAddress }
			};

			if (Opcode != 9)
            {
				return opcodeMap[Opcode].Canonical + " " + Offset;
            }
			else
            {
				return opcodeMap[Opcode].Canonical + " " + Cpu.Registers[X].Name;
            }
		}
	}
}
