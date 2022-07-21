using WackyArch.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Test.Components
{
    [TestClass]
	public class FilledPortTest
	{
		private FilledPort filledPort;
		private List<Word> data = new List<Word>
		{
			new Word { Value = 1 },
			new Word { Value = 2 },
			new Word { Value = 3 }
		};

		[TestInitialize]
		public void TestInitialize()
		{
			filledPort = new FilledPort(data, new Pipe(), "TEST");
		}

		[TestMethod]
		public void ConstructorCycles()
		{
			Assert.IsNotNull(filledPort.CurrentData);
			Assert.AreEqual(1, filledPort.CurrentData.Value);
		}

		[TestMethod]
		public void CurrentValueIsSetAfterCycling()
		{
			filledPort.Cycle();
			Assert.AreEqual(2, filledPort.BacklogData.Count());
			Assert.AreEqual(1, filledPort.CurrentData.Value);
			Assert.AreEqual(2, filledPort.BacklogData[0].Value);
			Assert.AreEqual(3, filledPort.BacklogData[1].Value);
		}

		[TestMethod]
		public void CanReadFromFilledPort()
		{
			filledPort.Cycle();
			Assert.AreEqual(1, filledPort.Pipe.Read(out _).Value);
			filledPort.Cycle();
			Assert.AreEqual(2, filledPort.Pipe.Read(out _).Value);
			filledPort.Cycle();
			Assert.AreEqual(3, filledPort.Pipe.Read(out _).Value);
		}
	}
}
