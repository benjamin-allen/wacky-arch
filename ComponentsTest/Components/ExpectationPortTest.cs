using WackyArch.Components;
using WackyArch.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Test.Components
{
    [TestClass]
	public class ExpectationPortTest
	{
		private ExpectationPort expectationPort;
		private List<Word> data = new List<Word>
		{
			new Word { Value = 1 },
			new Word { Value = 2 },
			new Word { Value = 3 },
		};

		[TestInitialize]
		public void TestInitialize()
		{
			expectationPort = new ExpectationPort(data, "TEST");
		}

		[TestMethod]
		public void CorrectWritesSucceed()
		{
			expectationPort.Pipe.Write(1);
			expectationPort.Cycle();
			expectationPort.Pipe.Write(2);
			expectationPort.Cycle();
			expectationPort.Pipe.Write(3);
			expectationPort.Cycle();
		}

		[TestMethod]
		public void WritesWithoutCyclingSucceed()
		{
			expectationPort.Pipe.Write(0);
			expectationPort.Pipe.Write(12);
			expectationPort.Pipe.Write(1);
			expectationPort.Cycle();
		}

		[TestMethod]
		public void WrongWritesFail()
		{
			try
			{
				expectationPort.Pipe.Write(0);
				expectationPort.Cycle();
				Assert.Fail();
			}
			catch (ComponentException cex)
			{
				Assert.AreEqual("TEST: Expected 1. Got 0", cex.ShortMessage);
			}
		}
	}
}
