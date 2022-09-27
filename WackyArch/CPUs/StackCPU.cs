using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Components;
using WackyArch.Instructions;
using WackyArch.Utilities;

namespace WackyArch.CPUs
{
	public class StackCPU : InterpreterCPU, ISupportsFunctionCall
	{
		public Stack Stack { get; set; }

		public StackCPU() : base()
        {
			Stack = new Stack();
			Ports = new Port[] { new Port(Stack.StackInterface, "STACK") };
        }

		public StackCPU(Port[] ports) : base()
        {
			Stack = new Stack();
			Ports = ports.Append(new Port(Stack.StackInterface, "STACK")).ToArray();
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
			for (int i = 0; i < ProgramBinary.Count; i++)
            {
				var word = ProgramBinary[i];
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
			base.Cycle();
			Stack.Cycle();
		}
	}
}
