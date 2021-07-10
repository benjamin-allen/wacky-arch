using Components;
using System;
using System.Collections.Generic;

namespace CPU
{
	public class CPU : ICyclable
	{
		public Port[] Ports;
		public Register[] Registers;
		public readonly Register Const;
		public bool IncrementPC;
		public bool IsHalted;
		public bool IsErrored;
		public Dictionary<int, int> PcLineMap;

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
			PcLineMap = new Dictionary<int, int>();
		}

		public CPU(Port[] ports) : this()
		{
			Ports = ports;
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

		public virtual void Cycle()
		{
			if(IncrementPC)
			{
				PC.Value += 1;
			}
			if(PC.Value < 0)
			{
				PC.Value = 0;
			}
			IncrementPC = true;
		}

		public virtual void Reset()
		{
			// Reset registers
			Registers[0].Data.Value = 0;
			Registers[1].Data.Value = 0;
			Registers[2].Data.Value = 0;
			Registers[3].Data.Value = 0;

			// Reset PC
			PC.Value = 0;

			// Unset IsHalted
			IsHalted = false;
			IsErrored = false;
		}
	}
}
