using Components;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;

namespace CPU.CPUs
{
	public class InterpreterCPU : CPU
	{
		private string ProgramText { get; set; }

		private List<Word> ProgramBinary { get; set; }


		public InterpreterCPU() : base() { }

		public InterpreterCPU(Port[] ports) : base(ports) { }

		public void Load(string programText)
		{
			Reset();
			ProgramText = programText;
			ProgramBinary = Assembler.Assemble(this, ProgramText, out var pcLineMap);
			PcLineMap = pcLineMap;
		}

		public override void Cycle()
		{
			// Is our PC within executable program? If so, execute the next instruction.
			if (GetPCValue() < ProgramBinary.Count)
			{
				Instruction insn = InstructionFactory.CreateInstruction(this, ProgramBinary[GetPCValue()]);
				insn.Execute();

				base.Cycle();
			}

			// otherwise, do nothing. Set the halt flag
			else
			{
				IsHalted = true;
			}
		}
	}
}
