using Components;
using CPU.CPUs;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using SadConsole.Input;
using Emulator.UIComponents;
using SadConsole.Controls;
using SadConsole.Instructions;
using Emulator.Challenges;
using System.Linq;

namespace Emulator.Architectures
{
	// The ALPHA architecture is the simplest available.
	// Just an interpreted CPU, 2 input ports, and an output port. 
	public class AlphaArchitecture : ControlsConsole
	{
		public InterpreterCPU Cpu { get; set; }
		public FilledPort Top;
		public FilledPort Bottom;
		public ExpectationPort Output;

		// SadConsole stuff - UI Components
		public CodeBox CodeBox { get; set; }
		public Button StepButton { get; set; }
		public Button RunButton { get; set; }
		public Button ResetButton { get; set; }
		public Button LoadButton { get; set; }
		public CPUInfoBox CpuInfoBox { get; set; }
		public InputPort TopInputPort { get; set; }
		public InputPort BottomInputPort { get; set; }
		public OutputPort OutputPort { get; set; }
		public CodeInstruction runInstruction { get; set; }

		private List<ICyclable> cyclables { get; set; }

		public AlphaArchitecture(AlphaChallenge challenge) : base((int)(Global.RenderWidth / Global.FontDefault.Size.X), (int)(Global.RenderHeight / Global.FontDefault.Size.Y))
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

			InitGUI();
		}

		private void InitGUI()
		{
			ThemeColors = SadConsole.Themes.Colors.CreateAnsi();

			CodeBox = new CodeBox(35, 25, Cpu);
			CodeBox.Position = new Point((Width / 2) - (CodeBox.Width / 2), 0);
			CodeBox.Parent = this;

			CpuInfoBox = new CPUInfoBox(Cpu);
			CpuInfoBox.Position = new Point(CodeBox.Width + CodeBox.Position.X, 0);
			CpuInfoBox.Parent = this;

			StepButton = new Button(CpuInfoBox.Width - 2, 1) { Text = "STEP", Position = new Point(CpuInfoBox.Position.X + 1, CpuInfoBox.Position.Y + CpuInfoBox.Height + 1) };
			Add(StepButton);
			StepButton.IsEnabled = false;
			StepButton.MouseButtonClicked += StepProgram;

			RunButton = new Button(CpuInfoBox.Width - 2, 1) { Text = "RUN ", Position = new Point(StepButton.Position.X, StepButton.Position.Y + 1) };
			Add(RunButton);
			RunButton.IsEnabled = false;
			RunButton.MouseButtonClicked += RunProgram;

			ResetButton = new Button(CpuInfoBox.Width - 2, 1) { Text = "RESET", Position = new Point(RunButton.Position.X, RunButton.Position.Y + 1) };
			Add(ResetButton);
			ResetButton.IsEnabled = true;
			ResetButton.MouseButtonClicked += ResetCpu;

			LoadButton = new Button(CpuInfoBox.Width - 2, 1) { Text = "LOAD", Position = new Point(ResetButton.Position.X, ResetButton.Position.Y + 1) };
			Add(LoadButton);
			LoadButton.IsEnabled = false;
			LoadButton.MouseButtonClicked += LoadCode;

			TopInputPort = new InputPort(Top, 10, 8);
			TopInputPort.Position = new Point(CodeBox.Position.X - TopInputPort.Width, 0);
			TopInputPort.Parent = this;

			BottomInputPort = new InputPort(Bottom, 18, 16);
			BottomInputPort.Position = new Point(CodeBox.Position.X - BottomInputPort.Width, 0);
			BottomInputPort.Parent = this;

			OutputPort = new OutputPort(Output, Width - CodeBox.Position.X - CodeBox.Width);
			OutputPort.Position = new Point(CodeBox.Position.X + CodeBox.Width, CodeBox.Position.Y + CodeBox.Height - 7);
			OutputPort.Parent = this;

			runInstruction = new CodeInstruction((hostObject, time) =>
			{
				var alphaArch = (AlphaArchitecture)hostObject;

				if (alphaArch.Cpu.IsHalted == false)
				{
					for (int i = 0; i < 10; i++)
					{
						cyclables.ForEach(c => c.Cycle());
					}
				}

				return alphaArch.Cpu.IsHalted;
			});
		}

		private void StepProgram(object sender, MouseEventArgs e)
		{
			try
			{
				CodeBox.Status = "";
				cyclables.ForEach(c => c.Cycle());
				if (Cpu.IsHalted)
				{
					StepButton.IsEnabled = false;
					RunButton.IsEnabled = false;
					ResetButton.IsEnabled = true;
				}
			}
			catch (ComponentException cex)
			{
				CodeBox.Status = cex.ShortMessage;
				Cpu.IsErrored = true;
			}
		}

		private void RunProgram(object sender, MouseEventArgs e)
		{
			CodeBox.Status = "";
			ResetButton.IsEnabled = true;
			StepButton.IsEnabled = false;
			this.Components.Add(runInstruction);
		}

		private void ResetCpu(object sender, MouseEventArgs e)
		{
			foreach(var cyclable in cyclables)
			{
				cyclable.Reset();
			}
			StepButton.IsEnabled = true;
			RunButton.IsEnabled = true;
			LoadButton.IsEnabled = true;
			ResetButton.IsEnabled = true;
			this.Components.Remove(runInstruction);
			CodeBox.Status = "System Reset";
		}

		private void LoadCode(object sender, MouseEventArgs e)
		{
			try
			{
				ResetCpu(sender, e);
				CodeBox.ErrorLine = -1;
				CodeBox.Status = "";
				Cpu.Load(CodeBox.Text);
				StepButton.IsEnabled = true;
				RunButton.IsEnabled = true;
				LoadButton.IsEnabled = false;
				CodeBox.Status = "Program Loaded";
			}
			catch(CPU.AssemblerException ex)
			{
				CodeBox.Status = ex.ShortMessage;
				CodeBox.ErrorLine = ex.LineNumber + 1;
			}
		}

		protected override void Invalidate()
		{
			base.Invalidate();

			if (StepButton != null)
			{
				DrawBox(new Rectangle(StepButton.Position.X - 1, StepButton.Position.Y - 1, CpuInfoBox.Width, 6), new Cell(Color.White, Color.Black, '-'), null, CellSurface.ConnectedLineThin);
				SetGlyph(StepButton.Position.X - 1, StepButton.Position.Y, 221);
			}
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			var canLoad = CodeBox.ProcessKeyboard(info);
			LoadButton.IsEnabled |= canLoad;
			return canLoad;
		}
	}
}
