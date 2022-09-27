using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Utilities;

namespace WackyArch.Components
{
	public class Stack : ICyclable
	{
		public static int MaxWords = 15;
		public Word[] Words;
		public int SP { get; set; }
		public Pipe StackInterface { get; set; }

		public Stack()
		{
			SP = 0;
			Words = new Word[MaxWords];
			StackInterface = new Pipe();
		}

		/// <summary>
		/// If a value was written, push it onto the stack.
		/// If the pipe was read from, pop it off the stack.
		/// </summary>
		public void Cycle()
		{
			if (StackInterface.Status == PipeStatus.AwaitingRead) // The stack was just written to
			{
				SP++;
				if (SP > MaxWords) { throw new ComponentException("Stack overflow", "Stack overflow"); }
				
			}
		}

		public void Reset()
		{
			throw new NotImplementedException();
		}
	}
}
