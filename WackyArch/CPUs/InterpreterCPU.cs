using WackyArch.Components;
using WackyArch.Instructions;
using WackyArch.Assemblers;
using WackyArch.Utilities;

namespace WackyArch.CPUs
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
			IsHalted = true;
		}

		public override void Cycle()
		{
			IsHalted = false;
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

		public override void Reset()
		{
			base.Reset();
			ProgramText = "";
			ProgramBinary = new List<Word>();
			PcLineMap = new Dictionary<int, int>();
		}
	}
}
