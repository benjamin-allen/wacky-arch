﻿using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;

namespace WackyArch.Instructions
{
    public class RegisterInstruction : Instruction
	{
		protected int X;
		protected int Y;
		protected int FuncCode;

		public RegisterInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			X = (word.Value & 0b0000_1100_0000) >> 6;
			Y = (word.Value & 0b0000_0011_0000) >> 4;
			FuncCode = word.Value & 0xF;
		}

		public override void Execute()
		{
			switch(FuncCode)
			{
				case 0x0:
					// Mov from Y to X
					Cpu.Registers[X].Data.Value = Cpu.Registers[Y].Data.Value;
					break;
				case 0x1:
					// Swap X and Y
					int bak = Cpu.Registers[X].Data.Value;
					Cpu.Registers[X].Data.Value = Cpu.Registers[Y].Data.Value;
					Cpu.Registers[Y].Data.Value = bak;
					break;
				case 0x2:
					// Compare X and Y
					if(Cpu.Registers[X].Data.Value > Cpu.Registers[Y].Data.Value)
					{
						Cpu.Const.Data.Value = 1;
					}
					else if(Cpu.Registers[X].Data.Value < Cpu.Registers[Y].Data.Value)
					{
						Cpu.Const.Data.Value = -1;
					}
					else
					{
						Cpu.Const.Data.Value = 0;
					}
					break;
				default:
					throw new ComponentException($"Invalid FuncCode {FuncCode}!", $"Invalid FuncCode {FuncCode}!");
			}
		}

		public override string Disassemble()
		{
			switch (FuncCode)
			{
				case 0x0:
					return Tokens.Move.Canonical + " " + Cpu.Registers[X].Name + " " + Cpu.Registers[Y].Name;
				case 0x1:
					return Tokens.Swap.Canonical + " " + Cpu.Registers[X].Name + " " + Cpu.Registers[Y].Name;
				case 0x2:
					return Tokens.Compare.Canonical + " " + Cpu.Registers[X].Name + " " + Cpu.Registers[Y].Name;
				default:
					throw new ComponentException($"Cannot disassemble register instruction with funccode {FuncCode}.", $"Invalid FuncCode: {FuncCode}");
			}
		}
	}
}
