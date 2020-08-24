using Components;
using CPU.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Instructions
{
	[TestClass]
	public class CPUPortInstructionTest
	{
		public CPU.CPU cpu;
		public Pipe input;
		public Pipe output;

		[TestInitialize]
		public void TestInitialize()
		{
			input = new Pipe();
			output = new Pipe();

			Port port0 = new Port(input, "INPUT");
			Port port1 = new Port(output, "OUTPUT");

			cpu = new CPU.CPU(new Port[] { port0, port1 });
		}

		[TestMethod]
		public void TestReadInstruction()
		{
			input.Write(255);
			var readInsn = new PortInstruction(cpu, new Word { Value = 0b0100_0000_0000 });
			readInsn.Execute();
			Assert.AreEqual(255, cpu.Registers[0].Data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);

			input.Write(511);
			readInsn = new PortInstruction(cpu, new Word { Value = 0b0100_0101_0000 });
			readInsn.Execute();
			Assert.AreEqual(511, cpu.Registers[1].Data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);
		}


		[TestMethod]
		public void TestWriteInstruction()
		{
			cpu.Registers[3].Data.Value = -111;
			var writeInsn = new PortInstruction(cpu, new Word { Value = 0b0100_1011_0001 });
			writeInsn.Execute();
			bool readResult;
			Word data = output.Read(out readResult);
			Assert.AreEqual(true, readResult);
			Assert.AreEqual(-111, data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);

			cpu.Registers[2].Data.Value = Word.Max;
			writeInsn = new PortInstruction(cpu, new Word { Value = 0b0100_1110_0001 });
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
			var readInsn = new PortInstruction(cpu, new Word { Value = 0b0100_0000_0001 });
			readInsn.Execute();
			Assert.AreEqual(3, cpu.Registers[0].Data.Value);
			Assert.AreEqual(false, cpu.IncrementPC);

			// targeting a non-existant port should just silently continue
			readInsn = new PortInstruction(cpu, new Word { Value = 0b0100_0000_1111 });
			readInsn.Execute();
			Assert.AreEqual(3, cpu.Registers[0].Data.Value);
			Assert.AreEqual(true, cpu.IncrementPC);
		}


		[TestMethod]
		public void TestWriteFailure()
		{
			cpu.Registers[1].Data.Value = 10;
			input.Write(-1); // Input isn't ready for another write. 
			var writeInsn = new PortInstruction(cpu, new Word { Value = 0b0100_1001_0000 });
			writeInsn.Execute();
			Assert.AreEqual(-1, input.Read(out _).Value);
			Assert.AreEqual(false, cpu.IncrementPC);

			// targeting a non-existant port should just silently continue
			writeInsn = new PortInstruction(cpu, new Word { Value = 0b0100_1001_0010 });
			writeInsn.Execute();
			Assert.AreEqual(true, cpu.IncrementPC);
		}
	}
}
