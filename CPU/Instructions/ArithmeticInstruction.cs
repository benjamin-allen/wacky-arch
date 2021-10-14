using Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace CPU.Instructions
{
	public class ArithmeticInstruction : Instruction
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

		public override void Execute()
		{
			switch (FuncCode)
			{
				case 0x0:
					// Add
					Cpu.Registers[X].Data.Value = Cpu.Registers[X].Data.Value + Cpu.Registers[Y].Data.Value;
					break;
				case 0x1:
					// Subtract
					Cpu.Registers[X].Data.Value = Cpu.Registers[X].Data.Value - Cpu.Registers[Y].Data.Value;
					break;
				case 0x2:
					// Multiply
					int xy = Cpu.Registers[X].Data.Value * Cpu.Registers[Y].Data.Value;
					Cpu.Registers[X].Data.Value = xy;
					break;
				case 0x3:
					// Divide
					if(Cpu.Registers[Y].Data.Value == 0)
					{
						throw new DivideByZeroException();
					}

					int y = Cpu.Registers[Y].Data.Value; 
					Cpu.Registers[X].Data.Value = Cpu.Registers[X].Data.Value / Cpu.Registers[Y].Data.Value;
					break;
				case 0x4:
					// Modulus
					if(Cpu.Registers[Y].Data.Value == 0)
					{
						throw new ComponentException("Division by 0", "Division by 0");
					}

					Cpu.Registers[X].Data.Value = ((Cpu.Registers[X].Data.Value % Cpu.Registers[Y].Data.Value) + Cpu.Registers[Y].Data.Value) % Cpu.Registers[Y].Data.Value; // Smart mod, dammit
					break;
				case 0x5:
					// Negate
					Cpu.Registers[Y].Data.Value = -Cpu.Registers[Y].Data.Value;
					break;
				case 0xA:
					Cpu.Registers[X].Data.AssignBitwise(Cpu.Registers[X].Data.Value & Cpu.Registers[Y].Data.Value);
					break;
				case 0xB:
					Cpu.Registers[X].Data.AssignBitwise(Cpu.Registers[X].Data.Value | Cpu.Registers[Y].Data.Value);
					break;
				case 0xC:
					Cpu.Registers[X].Data.AssignBitwise(Cpu.Registers[X].Data.Value ^ Cpu.Registers[Y].Data.Value);
					break;
				case 0xD:
					Cpu.Registers[Y].Data.AssignBitwise(~Cpu.Registers[Y].Data.Value);
					break;
				case 0xF:
					break; // No-op
				default:
					throw new InvalidOperationException("Invalid FuncCode!");
			}
		}
	}
}
