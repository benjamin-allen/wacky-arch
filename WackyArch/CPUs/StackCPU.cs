using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Components;

namespace WackyArch.CPUs
{
	class StackCPU : InterpreterCPU, ISupportsFunctionCall
	{
		public Stack<Word> Stack { get; set; }

		public void AddToCallStack(int currentPC)
		{
			throw new NotImplementedException();
		}

		public int GetReturnAddress()
		{
			throw new NotImplementedException();
		}

		int ISupportsFunctionCall.GetPCOfNthFunction(int n)
		{
			throw new NotImplementedException();
		}

		public override void Cycle()
		{
			base.Cycle();

		}
	}
}
