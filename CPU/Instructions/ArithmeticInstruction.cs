using Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPU.Instructions
{
	public abstract class ArithmeticInstruction : Instruction
	{
		protected int X;
		protected int Y;
		protected int FuncCode;

		public ArithmeticInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			X = (word.Value & 0b0000_1100_0000) >> 6;
			Y = (word.Value & 0b0000_0011_0000) >> 4;
			FuncCode = word.Value & 0xF;
		}
	}

	public class AddInstruction : ArithmeticInstruction
	{
		public AddInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			if(FuncCode != 0x0)
			{
				throw new InvalidOperationException("Created AddInstruction with invalid FuncCode");
			}
		}

		public override void Execute()
		{
			Cpu.Registers[X].Data.Value = Cpu.Registers[X].Data.Value + Cpu.Registers[Y].Data.Value;
		}
	}

	public class SubtractInstruction : ArithmeticInstruction
	{
		public SubtractInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			if(FuncCode != 0x1)
			{
				throw new InvalidOperationException("Created SubtractInstruction with invalid FuncCode");
			}
		}

		public override void Execute()
		{
			Cpu.Registers[X].Data.Value = Cpu.Registers[X].Data.Value - Cpu.Registers[Y].Data.Value;
		}
	}
}
