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

		private Word PC;

		public CPU()
		{
			Registers = new Register[]
			{
				new Register("R0"),
				new Register("R1"),
				new Register("R2"),
				new Register("CONST"),
			};
			PC = new Word();
			Const = Registers[3];
		}

		public CPU(Pipe[] pipes) : this()
		{
			PipeReferences = pipes;
		}

		public void SetPCValue(int value)
		{
			value = Math.Max(0, Math.Min(value, Word.Max));
			PC.Value = value;
		}

		public void OffsetPCValue(int value)
		{
			PC.Value += value;
			PC.Value = Math.Max(0, Math.Min(PC.Value, Word.Max));
		}

		public int GetPCValue()
		{
			return PC.Value;
		}

		public void Cycle()
		{
			// Load the word from memory and create an instruction.

			if(IncrementPC)
			{
				PC.Value += 1;
			}
			IncrementPC = true;
		}
	}
}
