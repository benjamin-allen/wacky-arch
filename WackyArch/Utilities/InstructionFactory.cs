using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Instructions;

namespace WackyArch.Utilities
{
    public static class InstructionFactory
    {
        public static Instruction CreateInstruction(CPU cpu, Word word)
        {
            int opcode = (word.Value & 0xF00) >> 8;

            switch (opcode)
            {
                case 0:
                    return new ArithmeticInstruction(cpu, word);
                case 1:
                case 2:
                    return new ShiftInstruction(cpu, word);
                case 3:
                    return new RegisterInstruction(cpu, word);
                case 4:
                    return new PortInstruction(cpu, word);
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return new JumpInstruction(cpu, word);
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    return new ConstInstruction(cpu, word);
                default:
                    throw new ArgumentException("Invalid Opcode!");
            }
        }
    }
}
