using Components;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace Emulator.UIComponents
{
	public class InputPort : ScrollingConsole
	{
		public FilledPort Port { get; set; }
		int stackHeight { get; set; }

		public InputPort(FilledPort port, int stackHeight, int width) : base(width, stackHeight + 2)
		{
			Port = port;
			this.stackHeight = stackHeight;

			//DefaultForeground = Emulator.EmulatorColors.Text;
			//DefaultBackground = Emulator.EmulatorColors.ControlBackDark;
		}

		public override void Draw(TimeSpan update)
		{
			base.Draw(update);
			Clear();

			// Draw shape outline
			DrawLine(new Point(0, 0), new Point(0, Height - 1), Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, 186);
			DrawLine(new Point(5, 0), new Point(5, stackHeight - 1), Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, 186);
			DrawLine(new Point(0, Height - 1), new Point(Width - 1, Height - 1), Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, 205);
			DrawLine(new Point(5, stackHeight - 1), new Point(Width - 1, stackHeight - 1), Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, 205);
			SetGlyph(0, Height - 1, 200);
			SetGlyph(5, stackHeight - 1, 200);

			// Draw current number
			if (Port.CurrentData != null)
			{
				Print(Width - 1 - 3, Height - 2, String.Format("{0,4:###0}", Port.CurrentData.Value));
				SetGlyph(2, Height - 2, 26);
			}
			else
			{
				// if there's a written value, show it in the input
				var data = Port.Pipe.Read(out bool didRead);
				if (didRead)
				{
					// write it back real quick, also display it
					Port.Pipe.Write(data.Value);
					Print(Width - 1 - 3, Height - 2, String.Format("{0,4:###0}", data.Value));
					SetGlyph(2, Height - 2, 26);
				}
			}

			// Draw other numbers
			for (int i = 0; i < stackHeight - 1 && i < Port.BacklogData.Count; i++)
			{
				Print(1, stackHeight - i - 2, String.Format("{0,4:###0}", Port.BacklogData[i].Value));
			}

			// Draw Port name
			SetGlyph(1, Height - 1, 181);
			Print(2, Height - 1, Port.Name);
			SetGlyph(2 + Port.Name.Length, Height - 1, 198);
		}
	}
}
