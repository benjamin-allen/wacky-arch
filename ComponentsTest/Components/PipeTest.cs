using WackyArch.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Components
{
    [TestClass]
	public class PipeTest
	{
		[TestMethod]
		public void Write()
		{
			Pipe pipe = new Pipe();

			pipe.Write(1);
			Assert.AreEqual(PipeStatus.AwaitingRead, pipe.Status);
			Assert.AreEqual(1, pipe.Read(out _).Value);
		}

		[TestMethod] // WR
		public void ReadAfterWrite()
		{
			Pipe pipe = new Pipe();

			// Reads should work fine assuming that a value has been written
			pipe.Write(29);

			bool result;
			Word pipeWord = pipe.Read(out result);
			Assert.AreEqual(true, result);
			Assert.AreEqual(29, pipeWord.Value);
			Assert.AreEqual(PipeStatus.Idle, pipe.Status);
		}

		[TestMethod] // RW
		public void ReadBeforeWriteReturnsFalse()
		{
			Pipe pipe = new Pipe();
			bool didRead;

			pipe.Read(out didRead);
			Assert.AreEqual(false, didRead);

			pipe.Write(1);
			pipe.Read(out _);

			pipe.Read(out didRead);
			Assert.AreEqual(false, didRead);
		}

		[TestMethod] // WW
		public void WriteAfterWriteReturnsFalseAndAbortsWrite()
		{
			Pipe pipe = new Pipe();

			Assert.AreEqual(true, pipe.Write(1));
			Assert.AreEqual(false, pipe.Write(2));
			Assert.AreEqual(1, pipe.Read(out _).Value);
		}

		[TestMethod]
		public void ReadAfterReadReturnsFalse()
		{
			Pipe pipe = new Pipe();
			bool didRead;

			pipe.Write(5);

			Word pipeWord = pipe.Read(out didRead);
			Assert.AreEqual(true, didRead);
			Assert.AreEqual(5, pipeWord.Value);

			pipeWord = pipe.Read(out didRead);
			Assert.AreEqual(false, didRead);
			Assert.AreEqual(null, pipeWord);
		}
	}
}
