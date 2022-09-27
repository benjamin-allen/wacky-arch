using WackyArch.Components;
using WackyArch.CPUs;

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
					throw new InvalidOperationException("Invalid FuncCode!");
			}
		}

        public override string Disassemble()
        {
            throw new NotImplementedException();
        }
    }
}
