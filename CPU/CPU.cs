using Components;
using System;

namespace CPU
{
	public class CPU
	{
		public Pipe[] PipeReferences;
		public Register[] Registers;
		public readonly Register Const;
		public bool IncrementPC;
		public Word PC;

		public CPU()
		{
			Registers = new Register[]
			{
				new Register("R0"),
				new Register("R1"),
				new Register("R2"),
				new Register("CONST"),
			};

			Const = Registers[3];
		}

		public CPU(Pipe[] pipes) : base()
		{
			PipeReferences = pipes;
		}
	}
}
