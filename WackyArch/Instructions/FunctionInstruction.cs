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
    /// There are 4 function instructions: DEFFUNC, ENDFUNC, CALL, and RETURN.
    /// </summary>
    public class FunctionInstruction : Instruction
    {
        public int FunctionNameLength;
        public bool IsCallInstruction;
        public bool IsReturnInstruction;
        public int FunctionNumber;

        public FunctionInstruction(CPU cpu, Word word) : base(cpu, word)
        {
            FunctionNameLength = word.Value & 0x07F;
            IsCallInstruction = ((word.Value & 0x0F0) >> 4) == 0xC;
            IsReturnInstruction = ((word.Value & 0x0F0) >> 4) == 0xF;
            FunctionNumber = word.Value & 0x00F;
        }

        public override void Execute()
        {
            if (Cpu is ISupportsFunctionCall)
            {
                var cpu = Cpu as ISupportsFunctionCall;
                if (!IsCallInstruction && !IsReturnInstruction)
                {
                    // When this instruction is executed, scan forward to the end of the function name and
                    // resume execution. A value of 300, which indicates the "ENDFUNCTION"
                    var startPointer = Cpu.GetPCValue() + FunctionNameLength;
                    Cpu.SetPCValue(startPointer);
                    Cpu.IncrementPC = true;
                }
                else if (IsCallInstruction)
                {
                    cpu.AddToCallStack(Cpu.GetPCValue());
                    // The function to call is the function number
                    Cpu.SetPCValue(cpu.GetPCOfNthFunction(FunctionNumber));
                    // After this point the CPU will basically execute the "definefunc" instruction and then go to execute the function's code.
                    Cpu.IncrementPC = false;
                }
                else if (IsReturnInstruction)
                {
                    Cpu.SetPCValue(cpu.GetReturnAddress());
                    Cpu.IncrementPC = true;
                }
            }
        }

        public override string Disassemble()
        {
            if (!IsCallInstruction && !IsReturnInstruction)
            {
                return FunctionNameLength > 0 ? Tokens.DefineFunction.Canonical : Tokens.EndFunction.Canonical;
            }
            else if (IsCallInstruction)
            {
                return Tokens.Call.Canonical + " " + FunctionNumber;
            }
            else
            {
                return Tokens.Return.Canonical;
            }
        }
    }
}
