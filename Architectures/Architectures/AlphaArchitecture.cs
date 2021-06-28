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
		public CPUInfoBox CpuInfoBox { get; set; }

		public AlphaArchitecture() : base((int)(Global.RenderWidth / Global.FontDefault.Size.X), (int)(Global.RenderHeight / Global.FontDefault.Size.Y))
		{
			Cpu = new InterpreterCPU(new Port[] { Top, Bottom, Output });
			ThemeColors = SadConsole.Themes.Colors.CreateAnsi();

			CodeBox = new CodeBox(30, 20);
			CodeBox.Position = new Point((Width / 2) - (CodeBox.Width / 2), 1);
			CodeBox.Parent = this;

			StepButton = new Button(8, 1) { Text = "Step", Position = new Point(40, 23) };
			Add(StepButton);

			CpuInfoBox = new CPUInfoBox(Cpu);
			CpuInfoBox.Position = new Point(CodeBox.Width + CodeBox.Position.X + 1, 0);
			CpuInfoBox.Parent = this;
		}

		protected override void Invalidate()
		{
			base.Invalidate();
			
			if(CodeBox != null)
			{
				DrawBox(new Rectangle(CodeBox.Position.X - 3, CodeBox.Position.Y - 1, CodeBox.Width + 4, CodeBox.Height + 1), new Cell(Color.White, Color.Black, '-'), null, CellSurface.ConnectedLineThin);
				DrawLine(new Point(CodeBox.Position.X - 1, CodeBox.Position.Y - 1), new Point(CodeBox.Position.X - 1, CodeBox.Position.Y + CodeBox.Height - 1), Color.White, Color.Black, 179);
				SetGlyph(CodeBox.Position.X - 1, CodeBox.Position.Y - 1, 194);
				SetGlyph(CodeBox.Position.X - 1, CodeBox.Position.Y + CodeBox.Height - 1, 193);
				
			}
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			return CodeBox.ProcessKeyboard(info);
		}
	}
}
