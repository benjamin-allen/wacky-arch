using Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Components
{
	[TestClass]
	public class MemoryTest
	{
		[TestMethod]
		public void WordsAreIndependent()
		{
			Memory memory = new Memory(2);
			memory.Fill(22);
			Assert.AreEqual(22, memory.Words[0].Value);
			Assert.AreEqual(22, memory.Words[1].Value);

			memory.Words[0].Value = 3;
			Assert.AreEqual(3, memory.Words[0].Value);
			Assert.AreEqual(22, memory.Words[1].Value);
		}

		[TestMethod]
		public void CanReadFromMemory()
		{
			Memory memory = new Memory(4);
			memory.Words[0].Value = 1;
			memory.Words[1].Value = 4;
			memory.Words[2].Value = 16;
			memory.Words[3].Value = 64;

			// So the idea is that an address would get written, then the memory would cycle, then the data would get read.
			// We'll test that process first.
			for(int i = 3; i >= 0; i--)
			{
				memory.Address.Write(i);
				memory.Cycle();
				Word d = memory.Data.Read(out _);
				Assert.AreEqual((int)Math.Pow(4, i), d.Value);
			}
		}

		[TestMethod]
		public void CanReadFromMemoryWithDelays()
		{
			Memory memory = new Memory(4);
			memory.Words[0].Value = 1;
			memory.Words[1].Value = 4;
			memory.Words[2].Value = 16;
			memory.Words[3].Value = 64;

			for (int i = 3; i >= 0; i--)
			{
				memory.Address.Write(i);
				for(int j = 0; j < new Random().Next(2, 10); j++)
				{
					memory.Cycle();
				}
				Word d = memory.Data.Read(out _);
				Assert.AreEqual((int)Math.Pow(4, i), d.Value);
			}
		}

		[TestMethod]
		public void CanWriteToMemory()
		{
			Memory memory = new Memory(2);
			memory.Words[0].Value = 10;
			memory.Words[1].Value = 100;

			// Writes are expected to write to the address first, then the data
			memory.Address.Write(1);
			memory.Cycle();
			memory.Data.Write(5);
			memory.Cycle();
			Assert.AreEqual(5, memory.Words[1].Value);

			memory.Address.Write(0);
			memory.Cycle();
			memory.Data.Write(67);
			memory.Cycle();
			Assert.AreEqual(67, memory.Words[0].Value);
			memory.Data.Write(0);
			memory.Cycle();
			Assert.AreEqual(0, memory.Words[0].Value);
		}
	}
}
