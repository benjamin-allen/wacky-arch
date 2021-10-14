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
using Utilities;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Emulator.Architectures
{
	// The ALPHA architecture is the simplest available.
	// Just an interpreted CPU, 2 input ports, and an output port. 
	public class AlphaArchitecture : ControlsConsole
	{
		public AlphaComponents AC { get; set; }

		// SadConsole stuff - UI Components
		public CodeBox CodeBox { get; set; }
		public Button StepButton { get; set; }
		public Button RunButton { get; set; }
		public Button ResetButton { get; set; }
		public Button LoadButton { get; set; }
		public Button QuitButton { get; set; }
		public Button SubmitButton { get; set; }
		public CPUInfoBox CpuInfoBox { get; set; }
		public InputPort TopInputPort { get; set; }
		public InputPort BottomInputPort { get; set; }
		public OutputPort OutputPort { get; set; }
		public CodeInstruction runInstruction { get; set; }
		public ScrollingConsole CommentsConsole { get; set; }
		public bool Running { get; set; }

		private int challengeId;

		public AlphaArchitecture(AlphaChallenge challenge) : base((int)(Global.RenderWidth / Global.FontDefault.Size.X), (int)(Global.RenderHeight / Global.FontDefault.Size.Y))
		{
			AC = new AlphaComponents(challenge);
			challengeId = challenge.ChallengeId;
			InitGUI();
			setComments(challenge.StartingComments);
		}

		private void InitGUI()
		{
			ThemeColors = Emulator.EmulatorColors;

			CodeBox = new CodeBox(35, 25, AC.Cpu);
			CodeBox.Position = new Point((Width / 2) - (CodeBox.Width / 2), 0);
			CodeBox.Parent = this;

			CpuInfoBox = new CPUInfoBox(AC.Cpu);
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

			QuitButton = new Button(CpuInfoBox.Width - 2, 1) { Text = "BACK", Position = new Point(LoadButton.Position.X, LoadButton.Position.Y + 3) };
			Add(QuitButton);
			QuitButton.IsEnabled = true;
			QuitButton.MouseButtonClicked += Quit;

			SubmitButton = new Button(CpuInfoBox.Width - 2, 1) { Text = "SUBMIT", Position = new Point(QuitButton.Position.X, QuitButton.Position.Y + 1) };
			Add(SubmitButton);
			SubmitButton.IsEnabled = false;
			SubmitButton.MouseButtonClicked += Submit;

			TopInputPort = new InputPort(AC.Top, 10, 8);
			TopInputPort.Position = new Point(CodeBox.Position.X - TopInputPort.Width, 0);
			TopInputPort.Parent = this;

			BottomInputPort = new InputPort(AC.Bottom, 18, 16);
			BottomInputPort.Position = new Point(CodeBox.Position.X - BottomInputPort.Width, 0);
			BottomInputPort.Parent = this;

			OutputPort = new OutputPort(AC.Output, Width - CodeBox.Position.X - CodeBox.Width);
			OutputPort.Position = new Point(CodeBox.Position.X + CodeBox.Width, CodeBox.Position.Y + CodeBox.Height - 7);
			OutputPort.Parent = this;

			runInstruction = new CodeInstruction((hostObject, time) =>
			{
				var alphaArch = (AlphaArchitecture)hostObject;

				for (int i = 0; i < 10; i++)
				{
					try
					{
						AC.cyclables.ForEach(c => c.Cycle());
					}
					catch (ComponentException cex)
					{
						CodeBox.Status = cex.ShortMessage;
						AC.Cpu.IsErrored = true;
					}
				}
				if (AC.Output.ExpectedData.Count == 0)
				{
					CodeBox.Status = "Tests passed!";
					SubmitButton.IsEnabled = true;
				}
				else if (alphaArch.AC.Cpu.IsErrored == false)
				{
					CodeBox.Status = "Waiting for output...";
				}
				return alphaArch.AC.Cpu.IsHalted || alphaArch.AC.Cpu.IsErrored;
			});
			runInstruction.RemoveOnFinished = true;

			CommentsConsole = new ScrollingConsole(Width - 2, Height - 27);
			CommentsConsole.Position = new Point(1, Height - (Height - 27) - 1);
			CommentsConsole.Cursor.UseLinuxLineEndings = true;
			CommentsConsole.Cursor.PrintAppearance = new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark);
			CommentsConsole.Parent = this;
		}

		private void StepProgram(object sender, MouseEventArgs e)
		{
			try
			{
				Running = true;
				CodeBox.Status = "";
				AC.cyclables.ForEach(c => c.Cycle());
				if (AC.Cpu.IsHalted)
				{
					StepButton.IsEnabled = false;
					RunButton.IsEnabled = false;
					ResetButton.IsEnabled = true;
					Running = false;
				}
				else if (AC.Output.ExpectedData.Count == 0)
				{
					CodeBox.Status = "Tests passed!";
					SubmitButton.IsEnabled = true;
					Running = false;
				}
				else
				{
					CodeBox.Status = "waiting for output...";
				}
			}
			catch (ComponentException cex)
			{
				CodeBox.Status = cex.ShortMessage;
				AC.Cpu.IsErrored = true;
			}
		}

		private void RunProgram(object sender, MouseEventArgs e)
		{
			ResetButton.IsEnabled = true;
			StepButton.IsEnabled = false;
			this.Components.Add(runInstruction);
			SubmitButton.IsEnabled = false;
		}

		private void ResetCpu(object sender, MouseEventArgs e)
		{
			foreach(var cyclable in AC.cyclables)
			{
				cyclable.Reset();
			}
			StepButton.IsEnabled = false;
			RunButton.IsEnabled = false;
			LoadButton.IsEnabled = true;
			ResetButton.IsEnabled = false;
			this.Components.Remove(runInstruction);
			CodeBox.Status = "System Reset";
			Running = false;
		}

		private void LoadCode(object sender, MouseEventArgs e)
		{
			try
			{
				ResetCpu(sender, e);
				CodeBox.ErrorLine = -1;
				CodeBox.Status = "";
				AC.Cpu.Load(CodeBox.Text);
				StepButton.IsEnabled = true;
				RunButton.IsEnabled = true;
				LoadButton.IsEnabled = false;
				ResetButton.IsEnabled = true;
				CodeBox.Status = "Program Loaded";
				Running = false;
			}
			catch(CPU.AssemblerException ex)
			{
				CodeBox.Status = ex.ShortMessage;
				CodeBox.ErrorLine = ex.LineNumber + 1;
			}
		}

		private void Quit(object sender, MouseEventArgs e)
		{
			this.Components.RemoveAll();
			Global.CurrentScreen = Emulator.ChallengeSelector;
			Global.CurrentScreen.IsFocused = true;
		}

		private void Submit(object sender, MouseEventArgs e)
		{
			var dto = new ProgramRunDTO { ChallengeId = challengeId, Code = CodeBox.Text, };//Team = Emulator.Team };
			setComments("Submitting... wait a while for results");

			// send this dto and wait for a response
			var request = new HttpRequestMessage(HttpMethod.Post, "https://joltprogrunner.azurewebsites.net/api/emulator/alpha")
			{
				Content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json")
			};
			var task = Task.Run(() => Emulator.httpClient.SendAsync(request));
			var sendInstruction = new CodeInstruction((hostObject, time) =>
			{
				if (task.IsCompleted)
				{
					var response = task.Result;
					var task2 = Task.Run(() => response.Content.ReadAsStringAsync());
					var readInstruction = new CodeInstruction((hostObject, time) =>
					{
						if (task2.IsCompleted)
						{
							var content = task2.Result;
							setComments(content);
							return true;
						}
						else
							return false;
					});
					readInstruction.RemoveOnFinished = true;
					this.Components.Add(readInstruction);
					return true;
				}
				else
					return false;
			});
			sendInstruction.RemoveOnFinished = true;
			this.Components.Add(sendInstruction);
		}

		protected override void Invalidate()
		{
			base.Invalidate();
			Fill(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, 0);

			if (StepButton != null)
			{
				DrawBox(new Rectangle(StepButton.Position.X - 1, StepButton.Position.Y - 1, CpuInfoBox.Width, 6), new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, '-'), null, CellSurface.ConnectedLineThin);
				SetGlyph(StepButton.Position.X - 1, StepButton.Position.Y, 221);
			}
			if (CommentsConsole != null)
			{
				DrawBox(new Rectangle(0, CommentsConsole.Position.Y - 1, Width, CommentsConsole.Height + 2), new Cell(Emulator.EmulatorColors.Text, Emulator.EmulatorColors.ControlBackDark, '-'), null, ConnectedLineThin);
			}
		}

		public override bool ProcessKeyboard(Keyboard info)
		{
			if (!Running)
			{
				var canLoad = CodeBox.ProcessKeyboard(info);
				LoadButton.IsEnabled |= canLoad;
				return canLoad;
			}
			else
			{
				if (info.KeysPressed.Count == 1 && info.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F10))
				{
					StepProgram(this, null);
				}
			}
			return false;
		}

		private void setComments(string text)
		{
			CommentsConsole.Clear();
			CommentsConsole.Cursor.Position = new Point(0, 0);
			CommentsConsole.Cursor.Print(text);
		}
	}
}
