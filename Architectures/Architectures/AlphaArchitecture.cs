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

namespace Emulator.Architectures
{
	// The ALPHA architecture is the simplest available.
	// Just an interpreted CPU, 2 input ports, and an output port. 
	public class AlphaArchitecture : ControlsConsole
	{
		public InterpreterCPU Cpu { get; set; }
		public Port Top = new Port(new Pipe(), "TOP");
		public Port Bottom = new Port(new Pipe(), "BOTTOM");
		public Port Output = new Port(new Pipe(), "OUTPUT");

		// SadConsole stuff
		public CodeBox CodeBox { get; set; }
		public Button StepButton { get; set; }
		public Button RunButton { get; set; }
		public Button ResetButton { get; set; }
		public Button LoadButton { get; set; }
		public CPUInfoBox CpuInfoBox { get; set; }

		private string statusText { get; set; }

		public CodeInstruction runInstruction { get; set; }

		public AlphaArchitecture() : base((int)(Global.RenderWidth / Global.FontDefault.Size.X), (int)(Global.RenderHeight / Global.FontDefault.Size.Y))
		{
			Cpu = new InterpreterCPU(new Port[] { Top, Bottom, Output });
			ThemeColors = SadConsole.Themes.Colors.CreateAnsi();

			CodeBox = new CodeBox(35, 25);
			CodeBox.Position = new Point((Width / 2) - (CodeBox.Width / 2), 0);
			CodeBox.Parent = this;

			CpuInfoBox = new CPUInfoBox(Cpu);
			CpuInfoBox.Position = new Point(CodeBox.Width + CodeBox.Position.X + 1, 0);
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

			runInstruction = new CodeInstruction((hostObject, time) =>
			{
				var alphaArch = (AlphaArchitecture)hostObject;

				if (alphaArch.Cpu.IsHalted == false)
				{
					for (int i = 0; i < 10; i++)
					{
						alphaArch.Cpu.Cycle();
					}
				}

				return alphaArch.Cpu.IsHalted;
			});
		}

		private void StepProgram(object sender, MouseEventArgs e)
		{
			Cpu.Cycle();
			if (Cpu.IsHalted)
			{
				StepButton.IsEnabled = false;
				RunButton.IsEnabled = false;
				ResetButton.IsEnabled = true;
			}
		}

		private void RunProgram(object sender, MouseEventArgs e)
		{
			ResetButton.IsEnabled = true;
			StepButton.IsEnabled = false;
			this.Components.Add(runInstruction);
		}

		private void ResetCpu(object sender, MouseEventArgs e)
		{
			Cpu.Reset();
			StepButton.IsEnabled = true;
			RunButton.IsEnabled = true;
			LoadButton.IsEnabled = true;
			ResetButton.IsEnabled = true;
			this.Components.Remove(runInstruction);
		}

		private void LoadCode(object sender, MouseEventArgs e)
		{
			try
			{
				CodeBox.ErrorLine = -1;
				CodeBox.Status = "";
				Cpu.Load(CodeBox.Text);
				StepButton.IsEnabled = true;
				RunButton.IsEnabled = true;
				LoadButton.IsEnabled = false;
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
