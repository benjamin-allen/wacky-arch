using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using Microsoft.Xna.Framework;
using Console = SadConsole.Console;
using Emulator.UIComponents;
using Emulator.Architectures;
using SadConsole.Themes;
using SadConsole.Controls;
using Newtonsoft.Json;
using Emulator.Challenges;
using System.IO;

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
			SadConsole.Game.Create(80, 25);
			SadConsole.Game.OnInitialize = Init;

			SadConsole.Game.Instance.Run();
			SadConsole.Game.Instance.Dispose();
		}

		static void Init()
		{
			SadConsole.Themes.Library.Default.SetControlTheme(typeof(Button), new SadConsole.Themes.ButtonTheme());

			//Global.CurrentScreen = new Architectures.AlphaArchitecture();
			var challenges = JsonConvert.DeserializeObject<List<AlphaChallenge>>(File.ReadAllText("Challenges/BootstrapChallenges.json"));
			var aarch = new AlphaArchitecture(challenges[0]);
			Global.CurrentScreen = aarch;
			Global.CurrentScreen.IsFocused = true;
		}
	}
}
