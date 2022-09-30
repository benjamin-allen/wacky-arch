using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;

namespace WackyArch.Instructions
{
	public class IOInstruction : Instruction
	{
		protected int FuncCode;
		protected int X;
		protected int PortNumber;

		public IOInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			FuncCode = (word.Value & 0b0000_1100_0000) >> 6;
			X = (word.Value & 0b0000_0011_0000) >> 4;
			PortNumber = (word.Value & 0b0000_0000_1111);
		}

		public override void Execute()
		{
			if(FuncCode == 0 || FuncCode == 1) // read case
			{
				try
				{
					bool readDidSucceed;
					Pipe pipe = Cpu.Ports[PortNumber].Pipe;
					Word word = pipe.Read(out readDidSucceed);
					if(readDidSucceed)
					{
						Cpu.Registers[X].Data.Value = word.Value;
					}
					Cpu.IncrementPC = readDidSucceed;
				}
				catch (IndexOutOfRangeException)
				{
					Cpu.IncrementPC = true;
				}
			}
			else if (FuncCode == 2) // write case
			{
				try
				{
					bool writeDidSucceed;
					Pipe pipe = Cpu.Ports[PortNumber].Pipe;
					writeDidSucceed = pipe.Write(Cpu.Registers[X].Data.Value);
					Cpu.IncrementPC = writeDidSucceed;
				}
				catch (IndexOutOfRangeException)
				{
					Cpu.IncrementPC = true;
				}
			}
			else if (FuncCode == 3 && X == 3 && PortNumber == 0) // special "INTERRUPT/UNLOCK" instruction
            {
				throw new Interrupt(InterruptType.UNLOCK);
            }
			else if (FuncCode == 3 && X == 3 && PortNumber == 0x1) // special "INTERRUPT/HALT" instruction
            {
				throw new Interrupt(InterruptType.HALT);
            }
			else if (FuncCode == 3 && X == 3 && PortNumber == 0xF) // special "INTERRUPT/HALT" instruction
			{
				throw new Interrupt(InterruptType.END);
			}
			else
            {
				throw new NotImplementedException("Invalid Interrupt.");
            }
		}

		public override string Disassemble()
		{
			switch(FuncCode)
			{
				case 0:
				case 1:
					return Tokens.Read.Canonical + " " + Cpu.Registers[X].Name + " " + Cpu.Ports[PortNumber].Name;
				case 2:
					return Tokens.Write.Canonical + " " + Cpu.Registers[X].Name + " " + Cpu.Ports[PortNumber].Name;
				case 3:
					if (PortNumber == 0 || PortNumber == 0xF || PortNumber == 0x1)
                    {
						return Tokens.Interrupt.Canonical + " " + Enum.GetName((InterruptType)PortNumber);
                    }
					goto default;
				default:
					throw new ComponentException($"Cannot disassemble IO instruction with funccode {FuncCode} and portnumber {PortNumber}.", $"Invalid FuncCode/PortNumber: {FuncCode}/{PortNumber}");

			}
		}
	}
}
