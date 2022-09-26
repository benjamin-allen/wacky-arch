using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;

namespace WackyArch.Instructions
{
    /// <summary>
    /// FUNCTION is a marker more than an instruction. The data following the function is its name
    /// and the number of words it puts on the stack.
    /// </summary>
    public class FunctionInstruction : Instruction
    {
        protected int FunctionNameLength;

        public FunctionInstruction(CPU cpu, Word word) : base(cpu, word)
        {
            FunctionNameLength = word.Value & 0x0FF;
        }

        public override void Execute()
        {
            // When this instruction is executed, scan forward to the end of the function name and
            // resume execution. A value of 300, which indicates the "ENDFUNCTION"
            var startPointer = Cpu.GetPCValue() + FunctionNameLength;
            Cpu.SetPCValue(startPointer);
            Cpu.IncrementPC = true;
        }

    }
}
