using Components;
using CPU.CPUs;
using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using Console = SadConsole.Console;
using Microsoft.Xna.Framework;
using SadConsole.Input;

namespace Emulator.Architectures
{
	// The ALPHA architecture is the simplest available.
	// Just an interpreted CPU, 2 input ports, and an output port. 
	public class AlphaArchitecture : ContainerConsole
	{
		public InterpreterCPU Cpu { get; set; }
		public Port Top = new Port(new Pipe(), "TOP");
		public Port Bottom = new Port(new Pipe(), "BOTTOM");
		public Port Output = new Port(new Pipe(), "OUTPUT");

		public Cell Player { get; set; }

		private Point _position;  public Point Position
		{
			get => _position;
			private set
			{
				AAConsole.Clear(_position.X, _position.Y);
				_position = value;
				Player.CopyAppearanceTo(AAConsole[_position.X, _position.Y]);
			}
		}

		// SadConsole stuff
		public Console AAConsole { get; }

		public AlphaArchitecture()
		{
			Cpu = new InterpreterCPU(new Port[] { Top, Bottom, Output });

			var width = (int)(Global.RenderWidth / Global.FontDefault.Size.X);
			var height = (int)(Global.RenderHeight / Global.FontDefault.Size.Y);
			AAConsole = new Console(width, height);
			AAConsole.Parent = this;

			Player = new Cell(Color.White, Color.Black, 1);
			_position = new Point(4, 4);
			Player.CopyAppearanceTo(AAConsole[_position.X, _position.Y]);
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			Point newPlayerPosition = Position;

			if (info.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Up))
				newPlayerPosition += SadConsole.Directions.North;
			else if (info.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Down))
				newPlayerPosition += SadConsole.Directions.South;

			if (info.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Left))
				newPlayerPosition += SadConsole.Directions.West;
			else if (info.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Right))
				newPlayerPosition += SadConsole.Directions.East;

			if (newPlayerPosition != Position)
			{
				Position = newPlayerPosition;
				return true;
			}

			return false;
		}
	}
}
