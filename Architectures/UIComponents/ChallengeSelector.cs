using SadConsole;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;
using Emulator.Challenges;
using SadConsole.Controls;
using Emulator.Architectures;

namespace Emulator.UIComponents
{
	public class ChallengeSelector : ScrollingConsole
	{
		ControlsConsole SelectConsole;
		ControlsConsole StartConsole;
		ScrollingConsole InfoConsole;
		List<AlphaChallenge> Challenges;

		int SelectedChallenge;

		public ChallengeSelector(int width, int height, List<AlphaChallenge> challenges) : base(width, height)
		{
			Fill(Emulator.EmulatorColors.ControlBackDark, Emulator.EmulatorColors.ControlBackDark, 0);
			if (challenges.Count < 1)
			{
				throw new ArgumentException("No challenges provided");
			}
			SelectedChallenge = 0;

			SelectConsole = new ControlsConsole((int)(width * 0.4), height);
			SelectConsole.DrawBox(new Rectangle(0, 0, SelectConsole.Width, SelectConsole.Height), new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, '-'), null, ConnectedLineThin);
			SelectConsole.Parent = this;
			SelectConsole.ThemeColors = Emulator.EmulatorColors;
			SelectConsole.Invalidated += SelectConsole_Invalidated;

			InfoConsole = new ScrollingConsole(width - SelectConsole.Width - 2, height - 5);
			InfoConsole.Position = new Point(SelectConsole.Width + 1, 1);
			DrawBox(new Rectangle(SelectConsole.Width, 0, InfoConsole.Width + 2, InfoConsole.Height+2), new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, '-'), null, ConnectedLineThin);
			InfoConsole.Parent = this;
			InfoConsole.Cursor.UseLinuxLineEndings = true;
			InfoConsole.Cursor.SetPrintAppearance(new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark));

			Challenges = challenges;

			AddChallengeButtons();

			var startButton = new Button(19, 1);
			startButton.Text = "START CHALLENGE";
			startButton.Click += (sender, eventArgs) =>
			{
				Global.CurrentScreen = new AlphaArchitecture(Challenges[SelectedChallenge]);
				Global.CurrentScreen.IsFocused = true;
			};

			StartConsole = new ControlsConsole(19, 1);
			StartConsole.Position = new Point(SelectConsole.Width + 20, height - 2);
			StartConsole.Add(startButton);
			StartConsole.Parent = this;
			StartConsole.ThemeColors = Emulator.EmulatorColors;
		}

		private void SelectConsole_Invalidated(object sender, EventArgs e)
		{
			SelectConsole.Fill(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, 0);
		}

		private void AddChallengeButtons()
		{
			for (int i = 0; i < Challenges.Count; i++)
			{
				var challenge = Challenges[i];

				// Add a button for the challenge
				var button = new Button(SelectConsole.Width - 2, 1) { Text = challenge.Name, Position = new Point(1, 1 + 2*i) };
				button.Click += createButtonHandler(i);

				SelectConsole.Add(button);
			}

			EventHandler createButtonHandler(int index) {
				return (sender, eventArgs) =>
				{
					SelectedChallenge = index;
				};
			}
		}

		public override void Draw(TimeSpan timeElapsed)
		{
			base.Draw(timeElapsed);

			InfoConsole.Clear();
			var challenge = Challenges[SelectedChallenge];
			InfoConsole.Cursor.Position = new Point(0, 0);
			InfoConsole.Cursor.Print(challenge.Name);
			InfoConsole.DrawLine(new Point(0, 2), new Point(InfoConsole.Width, 2), Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, '-');
			InfoConsole.Cursor.Position = new Point(0, 3);
			InfoConsole.Cursor.Print(challenge.Description);
			InfoConsole.DrawLine(new Point(0, 7), new Point(InfoConsole.Width, 7), Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, '-');
			InfoConsole.Cursor.Position = new Point(0, 8);
			InfoConsole.Cursor.Print(challenge.StartingComments);
		}
	}
}
