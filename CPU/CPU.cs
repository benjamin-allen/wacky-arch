using Components;
using System;

namespace CPU
{
	public class CPU
	{
		public Register[] Registers;

		public CPU()
		{
			Registers = new Register[]
			{
				new Register("R0"),
				new Register("R1"),
				new Register("R2"),
				new Register("CONST"),
			};
		}
	}
}
