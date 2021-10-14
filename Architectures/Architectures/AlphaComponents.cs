using Components;
using CPU.CPUs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emulator.Architectures
{
	public class AlphaComponents
	{
		public InterpreterCPU Cpu { get; set; }
		public FilledPort Top;
		public FilledPort Bottom;
		public ExpectationPort Output;

		public List<ICyclable> cyclables { get; set; }

		public AlphaComponents(Challenges.AlphaChallenge challenge)
		{
			Top = new FilledPort(challenge.TopInputData.Select(i => new Word { Value = i }).ToList(), new Pipe(), "top");
			Bottom = new FilledPort(challenge.BottomInputData.Select(i => new Word { Value = i }).ToList(), new Pipe(), "bottom");
			Output = new ExpectationPort(challenge.OutputData.Select(i => new Word { Value = i }).ToList(), "output");

			// Init cyclables
			cyclables = new List<ICyclable>();
			Cpu = new InterpreterCPU(new Port[] { Top, Bottom, Output });
			cyclables.Add(Cpu);
			cyclables.Add(Top);
			cyclables.Add(Bottom);
			cyclables.Add(Output);
		}
	}
}
