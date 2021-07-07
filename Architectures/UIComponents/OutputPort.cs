using Components;
using Microsoft.Xna.Framework;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emulator.UIComponents
{
	public class OutputPort : ScrollingConsole
	{
		public ExpectationPort Port { get; set; }

		public OutputPort(ExpectationPort port, int width) : base(width, 3)
		{
			Port = port;
		}

		public override void Draw(TimeSpan timeElapsed)
		{
			base.Draw(timeElapsed);
			Clear();

			// Draw shape outline
			DrawLine(new Point(0, 0), new Point(Width - 1, 0), Color.White, Color.Black, 205);
			DrawLine(new Point(0, Height - 1), new Point(Width - 1, Height - 1), Color.White, Color.Black, 205);

			// Draw expected number and arrow
			if (Port.ExpectedData != null && Port.ExpectedData.Count > 0)
			{
				Print(2, Height - 2, String.Format("{0,4:###0}", Port.ExpectedData[0].Value));
				SetGlyph(0, Height - 2, 26);
			}

			// Draw port name
			SetGlyph(1, Height - 1, 181);
			Print(2, Height - 1, Port.Name);
			SetGlyph(2 + Port.Name.Length, Height - 1, 198);
		}
	}
}
