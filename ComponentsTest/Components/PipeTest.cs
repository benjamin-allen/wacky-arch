using Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

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
			Assert.AreEqual(1, pipe.Data.Value);
		}

		[TestMethod] // WR
		public void ReadAfterWrite()
		{
			Pipe pipe = new Pipe();

			// Reads should work fine assuming that a value has been written
			pipe.Write(29);

			int pipeVal;
			bool result = pipe.Read(out pipeVal);
			Assert.AreEqual(true, result);
			Assert.AreEqual(29, pipeVal);
			Assert.AreEqual(PipeStatus.Idle, pipe.Status);
		}

		[TestMethod] // RW
		public void ReadBeforeWriteReturnsFalse()
		{
			Pipe pipe = new Pipe();

			int _;
			bool didRead = pipe.Read(out _);
			Assert.AreEqual(false, didRead);

			pipe.Write(1);
			didRead = pipe.Read(out _);

			didRead = pipe.Read(out _);
			Assert.AreEqual(false, didRead);
		}

		[TestMethod] // WW
		public void WriteAfterWriteReturnsFalse()
		{
			Pipe pipe = new Pipe();

			Assert.AreEqual(true, pipe.Write(1));
			Assert.AreEqual(false, pipe.Write(2));
			int x;
			Assert.AreEqual(true, pipe.Read(out x));
			Assert.AreEqual(1, x);
		}

		[TestMethod]
		public void ReadAfterReadReturnsFalse()
		{
			Pipe pipe = new Pipe();

			pipe.Write(5);
			int x;
			Assert.AreEqual(true, pipe.Read(out x));
			Assert.AreEqual(5, x);
			Assert.AreEqual(false, pipe.Read(out x));
			Assert.AreEqual(0, x);
		}
	}
}
