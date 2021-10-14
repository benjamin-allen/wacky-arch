using Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Emulator.Challenges
{
	public class AlphaChallenge
	{
		public int ChallengeId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string StartingComments { get; set; }
		public List<int> TopInputData { get; set; }
		public List<int> BottomInputData { get; set; }
		public List<int> OutputData { get; set; }
		public int Order { get; set; }
	}
}
