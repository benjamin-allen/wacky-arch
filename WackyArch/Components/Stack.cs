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
		public static int MaxWords = 14;
		public Word[] Words;
		public int SP { get; set; }
		public StackPipe StackInterface { get; set; }

		public Stack()
		{
			SP = 0;
			Words = new Word[MaxWords];
			StackInterface = new StackPipe() { Name = "Stack" };
		}

		/// <summary>
		/// If a value was written, push it onto the stack.
		/// If the pipe was read from, pop it off the stack.
		/// </summary>
		public void Cycle()
		{
			if (StackInterface.Status == PipeStatus.AwaitingRead) // The stack was just written to
			{
				var newVal = StackInterface.Read(out _);
				StackInterface.Status = PipeStatus.Idle;
				if (SP == 15) {
					SP = newVal.Value & 0x00F; // Sets a new stack pointer (!!!)
				}
				else
                {
					Words[SP] = new Word { Value = newVal.Value };
					SP++;
                }
			}
			else if (StackInterface.Status == PipeStatus.ForceRead) // The stack was just read from. Adjust SP
            {
				SP--;
				if (SP < 0) { throw new ComponentException("Stack underflow", "Stack underflow"); }
				if (SP > 0)
				{
					StackInterface.Write(Words[SP - 1].Value);
				}
				StackInterface.Status = PipeStatus.Idle;
            }
		}

		public void Reset()
		{
			SP = -1;
			Words = new Word[MaxWords];
		}
	}
}
