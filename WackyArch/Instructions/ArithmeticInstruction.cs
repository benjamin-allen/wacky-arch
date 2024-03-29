﻿using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;

namespace WackyArch.Instructions
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
						throw new ComponentException($"Division by zero: R{X}/R{Y}", "Division by zero");
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
					Cpu.IncrementPC = true;
					break; // No-op
				default:
					throw new ComponentException($"Invalid FuncCode {FuncCode}", $"Invalid FuncCode {FuncCode}");
			}
		}

        public override string Disassemble()
        {
			var opcodeMap = new Dictionary<int, Token> {
				{ 0, Tokens.Add }, { 1, Tokens.Subtract}, { 2, Tokens.Multiply }, { 3, Tokens.Divide },
				{ 4, Tokens.Modulus }, { 5, Tokens.Negate }, { 10, Tokens.And }, { 11, Tokens.Or }, { 12, Tokens.Xor },
				{ 13, Tokens.Not }, { 15, Tokens.NoOp}
			};
			var registerMap = new Dictionary<int, Token> { { 0, Tokens.R0 }, { 1, Tokens.R1 }, { 2, Tokens.R2 }, { 3, Tokens.Const } };

			switch (FuncCode)
			{
				case 0x0:
				case 0x1:
				case 0x2:
				case 0x3:
				case 0x4:
				case 0xA:
				case 0xB:
				case 0xC:
					return opcodeMap[FuncCode].Canonical + " " + registerMap[X].Canonical + " " + registerMap[Y].Canonical;
				case 0x5:
				case 0xD:
					return opcodeMap[FuncCode].Canonical + " " + registerMap[Y].Canonical;
				case 0xF:
					return opcodeMap[FuncCode].Canonical;
				default:
					throw new ComponentException($"Cannot disassemble arithmetic instruction with funccode {FuncCode}.", $"Invalid FuncCode: {FuncCode}");
            }
        }
    }
}
