using WackyArch.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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

		[TestMethod]
		public void CanWriteToMemoryWithDelays()
		{
			Memory memory = new Memory(1);
			Assert.AreEqual(0, memory.Words[0].Value);

			memory.Address.Write(0);
			memory.Cycle();
			for(int i = 0; i < new Random().Next(3, 20); i++)
			{
				memory.Cycle();
			}
			memory.Data.Write(85);
			for (int i = 0; i < new Random().Next(3, 20); i++)
			{
				memory.Cycle();
			}
			Assert.AreEqual(85, memory.Words[0].Value);
		}

		[TestMethod]
		public void RandomDelayRandomWrite()
		{
			Random r = new Random();
			int n = 1 << r.Next(3, Word.Size - 1);
			int[] addresses = new int[n];
			int[] data = new int[n];
			Memory memory = new Memory(n);

			for(int i = 0; i < n; i++)
			{
				addresses[i] = i;
				data[i] = r.Next(Word.Min, Word.Max + 1);
			}
			addresses = addresses.OrderBy(x => r.Next()).ToArray(); // shuffle addresses

			for(int i = 0; i < n; i++)
			{
				// pull out the address and data.
				int addr = addresses[i];
				int dat = data[i];

				StressCycle();
				memory.Address.Write(addr);

				StressCycle();
				memory.Data.Write(dat);

				StressCycle();
				Assert.AreEqual(dat, memory.Data.Read(out _).Value);
			}

			// Now that the writes are complete, iterate over memory and verify that each address contains the data associated
			for(int i = 0; i < n; i++)
			{
				Assert.AreEqual(data[i], memory.Words[addresses[i]].Value);
			}

			// Wait a random amount of cycles (at least one), then read data and verify it matches dat
			void StressCycle()
			{
				for(int j = 0; j < r.Next(1, 50); j++)
				{
					memory.Cycle();
				}
			}
		}
	}
}
