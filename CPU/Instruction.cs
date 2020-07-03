using Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPU
{
	/// <summary>
	/// For this project, an instruction is just something that can be executed by the CPU by calling Execute()
	/// It has a reference to the CPU that built it.
	/// </summary>
	public abstract class Instruction
	{
		public CPU Cpu { get; private set; }

		public int Opcode { get; private set; }

		public Instruction(CPU cpu, Word word)
		{
			Cpu = cpu;
			Opcode = (word.Value & 0xF00) >> 8;
		}

		public abstract void Execute();
	}
}
