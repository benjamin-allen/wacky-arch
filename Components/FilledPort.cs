using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
	public class FilledPort : Port, ICyclable
	{
		public List<Word> BacklogData { get; set; }
		public Word CurrentData { get; set; }

		public FilledPort(List<Word> data, Pipe pipe, string name) : base(pipe, name)
		{
			this.BacklogData = data;
			Cycle();
		}

		public override void Cycle()
		{
			if (BacklogData.Count > 0 && (Pipe.Status == PipeStatus.Idle || Pipe.Status == PipeStatus.AwaitingWrite))
			{
				// Write the next value from the list, then shrink the list by one
				CurrentData = BacklogData[0];
				Pipe.Write(BacklogData[0].Value);
				BacklogData = BacklogData.Skip(1).ToList();
			}
			else if (Pipe.Status == PipeStatus.Idle || Pipe.Status == PipeStatus.AwaitingWrite)
			{
				CurrentData = null;
				Pipe.Status = PipeStatus.Idle;
			}
		}
	}
}
