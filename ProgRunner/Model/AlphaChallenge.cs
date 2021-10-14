using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgRunner.Model
{
	public class AlphaChallenge
	{
		public int Id { get; set; }
		public string Flag { get; set; }
		public ICollection<AlphaChallengeTest> AlphaChallengeTests { get; set; }
	}
}
