using System;
using System.ComponentModel.DataAnnotations;

namespace ProgRunner.Model
{
	public class RunLog
	{
		public int Id { get; set; }
		public int ChallengeId { get; set; }
		public DateTime SubmittedTime { get; set; }
		public DateTime CompletedTime { get; set; }
		public string Team { get; set; }
		public string Code { get; set; }
		public string Result { get; set; }
	}
}
