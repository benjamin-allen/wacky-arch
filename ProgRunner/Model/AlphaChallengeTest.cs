using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgRunner.Model
{
	public class AlphaChallengeTest
	{
		public int Id { get; set; }
		public AlphaChallenge AlphaChallenge { get; set; }
		public string TopInput { get; set; }
		public string BottomInput { get; set; }
		public string ExpectedOutput { get; set; }
	}
}
