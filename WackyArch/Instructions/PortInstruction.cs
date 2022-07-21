using WackyArch.Components;
using WackyArch.CPUs;

namespace WackyArch.Instructions
{
	public class PortInstruction : Instruction
	{
		protected int FuncCode;
		protected int X;
		protected int PortNumber;

		public PortInstruction(CPU cpu, Word word) : base(cpu, word)
		{
			FuncCode = (word.Value & 0b0000_1000_0000) >> 7;
			X = (word.Value & 0b0000_0011_0000) >> 4;
			PortNumber = (word.Value & 0b0000_0000_1111);
		}

		public override void Execute()
		{
			if(FuncCode == 0)
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
				catch (IndexOutOfRangeException ex)
				{
					Cpu.IncrementPC = true;
				}
			}
			else
			{
				try
				{
					bool writeDidSucceed;
					Pipe pipe = Cpu.Ports[PortNumber].Pipe;
					writeDidSucceed = pipe.Write(Cpu.Registers[X].Data.Value);
					Cpu.IncrementPC = writeDidSucceed;
				}
				catch (IndexOutOfRangeException ex)
				{
					Cpu.IncrementPC = true;
				}
			}
		}
	}
}
