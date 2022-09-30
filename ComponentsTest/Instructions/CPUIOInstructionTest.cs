using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WackyArch.Utilities;
using System;

namespace Test.Instructions
{
	[TestClass]
	public class CPUIOInstructionTest
	{
		public CPU cpu;
		public Pipe input;
		public Pipe output;

		[TestInitialize]
		public void TestInitialize()
		{
			input = new Pipe();
			output = new Pipe();

			Port port0 = new Port(input, "INPUT");
			Port port1 = new Port(output, "OUTPUT");

			cpu = new CPU(new Port[] { port0, port1 });
		}

		[TestMethod]
		public void TestReadInstruction()
		{
			input.Write(255);
			var readInsn = new IOInstruction(cpu, new Word { Value = 0b0100_0000_0000 });
			readInsn.Execute();
			Assert.AreEqual(255, cpu.Registers[0].Data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);

			input.Write(511);
			readInsn = new IOInstruction(cpu, new Word { Value = 0b0100_0101_0000 });
			readInsn.Execute();
			Assert.AreEqual(511, cpu.Registers[1].Data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);
		}


		[TestMethod]
		public void TestWriteInstruction()
		{
			cpu.Registers[3].Data.Value = -111;
			var writeInsn = new IOInstruction(cpu, new Word { Value = 0b0100_1011_0001 });
			writeInsn.Execute();
			bool readResult;
			Word data = output.Read(out readResult);
			Assert.AreEqual(true, readResult);
			Assert.AreEqual(-111, data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);

			cpu.Registers[2].Data.Value = Word.Max;
			writeInsn = new IOInstruction(cpu, new Word { Value = 0b0100_1010_0001 });
			writeInsn.Execute();
			data = output.Read(out readResult);
			Assert.AreEqual(true, readResult);
			Assert.AreEqual(Word.Max, data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);
		}


		[TestMethod]
		public void TestReadFailure()
		{
			cpu.Registers[0].Data.Value = 3;
			output.Write(1);
			output.Read(out _); // Output isn't ready for another read.
			var readInsn = new IOInstruction(cpu, new Word { Value = 0b0100_0000_0001 });
			readInsn.Execute();
			Assert.AreEqual(3, cpu.Registers[0].Data.Value);
			Assert.AreEqual(false, cpu.IncrementPC);

			// targeting a non-existant port should just silently continue
			readInsn = new IOInstruction(cpu, new Word { Value = 0b0100_0000_1111 });
			readInsn.Execute();
			Assert.AreEqual(3, cpu.Registers[0].Data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);
		}


		[TestMethod]
		public void TestWriteFailure()
		{
			cpu.Registers[1].Data.Value = 10;
			input.Write(-1); // Input isn't ready for another write. 
			var writeInsn = new IOInstruction(cpu, new Word { Value = 0b0100_1001_0000 });
			writeInsn.Execute();
			Assert.AreEqual(-1, input.Read(out _).Value);
			Assert.AreEqual(false, cpu.IncrementPC);

			// targeting a non-existant port should just silently continue
			writeInsn = new IOInstruction(cpu, new Word { Value = 0b0100_1001_0010 });
			writeInsn.Execute();
			Assert.AreEqual(true, cpu.IncrementPC);
		}

        [TestMethod]
		public void TestUnlockInterrupt()
        {
			var unlockInsn = new IOInstruction(cpu, new Word { Value = 0b0100_1111_0000 });
			try
            {
				unlockInsn.Execute();
            }
			catch (Interrupt i)
            {
				Assert.AreEqual(i.InterruptType, InterruptType.UNLOCK);
            }
        }

        [TestMethod]
		public void TestHaltInterrupt()
        {
			var haltInsn = new IOInstruction(cpu, new Word { Value = 0b0100_1111_0001 });
			try
            {
				haltInsn.Execute();
            }
			catch (Interrupt i)
            {
				Assert.AreEqual(i.InterruptType, InterruptType.HALT);
            }
        }

		[TestMethod]
		public void TestEndInterrupt()
		{
			var haltInsn = new IOInstruction(cpu, new Word { Value = 0b0100_1111_1111 });
			try
			{
				haltInsn.Execute();
			}
			catch (Interrupt i)
			{
				Assert.AreEqual(i.InterruptType, InterruptType.END);
			}
		}

		[TestMethod]
		public void TestInvalidInterrupts()
        {
			var baseVal = 0b0100_1111_0000;
			for (int i = 2; i < 0xF; i++)
            {
				var invalidInterruptInsn = new IOInstruction(cpu, new Word { Value = baseVal + i });
				try
                {
					invalidInterruptInsn.Execute();
					Assert.Fail();
                }
				catch (NotImplementedException)
                {
                }
            }
        }
	}
}
