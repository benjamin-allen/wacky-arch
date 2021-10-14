using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{
	public class ExpectationPort : Port, ICyclable
	{
		public List<Word> ExpectedData { get; set; }
		private List<Word> loadedData { get; set; }

		public ExpectationPort(List<Word> expectedData, string name) : base(new AlwaysWriteablePipe(), name)
		{
			ExpectedData = expectedData;
			loadedData = expectedData.Select(w => new Word { Value = w.Value }).ToList();
			Cycle();
		}

		public override void Cycle()
		{

			if (ExpectedData.Count > 0)
			{
				var word = Pipe.Read(out bool didRead);
				if (didRead)
				{
					// Compare the read value to what we want to see in the expected data.
					if (word.Value != ExpectedData[0].Value)
					{
						// Halt the emulator somehow.
						string errMessage = $"{Name}: Expected {ExpectedData[0].Value}. Got {word.Value}";
						throw new ComponentException(errMessage, errMessage);
					}

					ExpectedData = ExpectedData.Skip(1).ToList();
				} 
			}
		}

		public override void Reset()
		{
			ExpectedData = loadedData.Select(w => new Word { Value = w.Value }).ToList();
			this.Pipe.Status = PipeStatus.Idle;
			Cycle();
		}
	}
}
