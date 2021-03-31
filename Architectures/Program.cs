using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using Microsoft.Xna.Framework;
using Console = SadConsole.Console;
using Emulator.UIComponents;

namespace Emulator
{
	// main amber color: cc9900
	// "good" green color: 99cc00
	// "bad" red color: cc3300

	class Program
	{
		static int x = 5;
		static int y = 5;

		public static void Main(string[] args)
		{
			SadConsole.Game.Create(120, 40);
			SadConsole.Game.OnInitialize = Init;

			SadConsole.Game.Instance.Run();
			SadConsole.Game.Instance.Dispose();
		}

		static void Init()
		{
			//Global.CurrentScreen = new Architectures.AlphaArchitecture();
			Global.CurrentScreen = new CodeBox(120, 40);
			Global.CurrentScreen.IsFocused = true;
		}
	}
}
