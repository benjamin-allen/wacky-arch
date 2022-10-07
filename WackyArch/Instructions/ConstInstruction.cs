using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;

namespace WackyArch.Instructions
{
    public class ConstInstruction : Instruction
	{
		protected int Value;

		public ConstInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			Value = word.Value & 0xFF; // V is assumed to be unsigned
		}

		public override void Execute()
		{
			switch(Opcode)
			{
				case 0b1010:
					// Add const
					Cpu.Const.Data.Value += Value;
					break;
				case 0b1011:
					// Sub const
					Cpu.Const.Data.Value -= Value;
					break;
				case 0b1100:
					// Mod const
					Cpu.Const.Data.Value %= Value;
					break;
				case 0b1101:
					// And const
					Cpu.Const.Data.Value &= Utilities.Utilities.SignExtend(Value, 7);
					break;
				case 0b1110:
					// Or const
					Cpu.Const.Data.Value |= Utilities.Utilities.SignExtend(Value, 7);
					break;
				case 0b1111:
					// Move const
					Cpu.Const.Data.Value = Utilities.Utilities.SignExtend(Value, 7);
					break;
				default:
					throw new ComponentException($"Invalid Opcode {Opcode}", $"Invalid Opcode {Opcode}");
			}
		}

        public override string Disassemble()
        {
			var opcodeMap = new Dictionary<int, Token> {
				{ 10, Tokens.AddConstant }, { 11, Tokens.SubConstant }, { 12, Tokens.ModConstant },
				{ 13, Tokens.AndConstant }, { 14, Tokens.OrConstant }, { 15, Tokens.MoveConstant }
			};

			return opcodeMap[Opcode].Canonical + " " + (Opcode > 0b1100 ? Utilities.Utilities.SignExtend(Value, 7) : Value);
		}
    }
}
