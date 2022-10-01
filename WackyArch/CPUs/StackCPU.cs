using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Assemblers;
using WackyArch.Components;
using WackyArch.Instructions;
using WackyArch.Utilities;

namespace WackyArch.CPUs
{
	public class StackCPU : CPU, ISupportsFunctionCall
	{
		public Stack Stack { get; set; }
		public Memory Memory { get; set; }
		private string programText { get; set; }
		private List<Word> programBinary { get; set; }

		public StackCPU() : base()
        {
			Stack = new Stack();
			Memory = new Memory(256);
			var addrPort = new Port(Memory.Address, "ADDR");
			var dataPort = new Port(Memory.Data, "DATA");
			Ports = new Port[] { new Port(Stack.StackInterface, "STACK"), addrPort, dataPort };
        }

		public StackCPU(Port[] ports) : base()
        {
			Stack = new Stack();
			Memory = new Memory(256);
			var addrPort = new Port(Memory.Address, "ADDR");
			var dataPort = new Port(Memory.Data, "DATA");

			var p = new Port[] { new Port(Stack.StackInterface, "STACK"), addrPort, dataPort }.ToList();
			p.AddRange(ports);
			Ports = p.ToArray();
        }

		public void AddToCallStack(int currentPC)
		{
			Stack.StackInterface.Write(currentPC);
		}

		public int GetReturnAddress()
		{
			return Stack.StackInterface.Read(out _).Value;
		}

		public int GetPCOfNthFunction(int n)
		{
			for (int i = 0; i < Memory.Words.Length; i++)
            {
				var word = Memory.Words[i];
				if ((word.Value & 0xF00) == 0x300) // the word is a function instruction 
				{
					var functionInsn = new FunctionInstruction(this, word);
					if (!functionInsn.IsCallInstruction && !functionInsn.IsReturnInstruction && (word.Value & 0x07F) != 0 )
                    {
						if (n == 0) // this is our function!
                        {
							return i;
                        }
						else
                        {
							n--;
                        }
                    }
                } 
            }
			throw new ComponentException($"The function numbered {n} could not be found.", $"No function numbered {n}");
		}

		public override void Cycle()
		{
			IsHalted = false;
			if (GetPCValue() < Memory.Words.Length && GetPCValue() < programBinary.Count) 
            {
				Instruction insn = InstructionFactory.CreateInstruction(this, Memory.Words[GetPCValue()]);
				insn.Execute();

				// Update the display program text and PCLineMap
				programText = Disassembler.Disassemble(this, Memory.Words.ToList(), out PcLineMap);

				base.Cycle();
            }
			else
            {
				IsHalted = true;
            }

			// Cycle my other components
			Stack.Cycle();
			Memory.Cycle();
		}

		public void Load(List<Word> binary)
        {
			if (binary.Count > Memory.Words.Length)
            {
				throw new ComponentException($"Can't load a binary of size {binary.Count} into memory of size {Memory.Words.Length}", "Binary too big!");
            }
			programBinary = binary;
			Reset();
			IsHalted = true;
        }

        public override void Reset()
        {
			base.Reset();
			programText = Disassembler.Disassemble(this, programBinary, out PcLineMap);
			for (int i = 0; i < programBinary.Count; i++)
			{
				Memory.Words[i] = programBinary[i];
			}
		}
    }
}
