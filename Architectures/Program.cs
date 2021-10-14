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
using System.Net.Http;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Emulator
{
	// main amber color: cc9900
	// "good" green color: 99cc00
	// "bad" red color: cc3300

	class Program
	{
		public static void Main(string[] args)
		{
			SadConsole.Game.Create(Emulator.WindowWidth, Emulator.WindowHeight);
			SadConsole.Game.OnInitialize = Init;

			SadConsole.Game.Instance.Run();
			SadConsole.Game.Instance.Dispose();
		}

		static void Init()
		{
			SadConsole.Themes.Library.Default.SetControlTheme(typeof(Button), new SadConsole.Themes.ButtonTheme());

			//Global.CurrentScreen = new Architectures.AlphaArchitecture();
			var challenges = LoadAlphaChallenges();
			Emulator.ChallengeSelector = new ChallengeSelector(Emulator.WindowWidth, Emulator.WindowHeight, challenges);
			//var aarch = new AlphaArchitecture(challenges[1]);
			Global.CurrentScreen = Emulator.ChallengeSelector;
			Global.CurrentScreen.IsFocused = true;
		}

		static List<AlphaChallenge> LoadAlphaChallenges()
		{
			var matcher = new Matcher();
			matcher.AddInclude("Challenges/alpha_*.json");

			var challenges = new List<AlphaChallenge>();

			foreach (string file in matcher.GetResultsInFullPath(Directory.GetCurrentDirectory()))
			{
				var c = JsonConvert.DeserializeObject<List<AlphaChallenge>>(File.ReadAllText(file));
				challenges = challenges.Concat(c).ToList();
			}
			challenges = challenges.OrderBy(c => c.Order).ToList();

			return challenges;
		}
	}

	public static class Emulator
	{
		public static int WindowHeight = 35;
		public static int WindowWidth = 100;
		public static ChallengeSelector ChallengeSelector;
		public static readonly HttpClient httpClient = new HttpClient();

		public static Colors EmulatorColors;

		static Emulator() 
		{
			//https://mdigi.tools/color-shades/#ffab29
			EmulatorColors = Colors.CreateDefault();
			EmulatorColors.Text = new Color(255, 165, 26); // amber
			EmulatorColors.ControlBack = new Color(77, 46, 0); // brown
			EmulatorColors.TextLight = new Color(255, 205, 128); // light amber
			EmulatorColors.ControlBackDark = new Color(26, 15, 0); // black
			EmulatorColors.TextSelectedDark = new Color(77, 46, 0); // dark
			EmulatorColors.ControlBackSelected = new Color(EmulatorColors.Text.PackedValue); // amber
			EmulatorColors.TextSelected = new Color(EmulatorColors.TextSelectedDark.PackedValue); // near white
			EmulatorColors.TextFocused = new Color(EmulatorColors.TextSelectedDark.PackedValue);
			EmulatorColors.ControlBackLight = new Color(EmulatorColors.Text.PackedValue);
			EmulatorColors.RebuildAppearances();
			EmulatorColors.ControlHostBack = new Color(EmulatorColors.ControlBackDark.PackedValue);
		}
	}
}
