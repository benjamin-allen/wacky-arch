using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Instructions;
using static WackyArch.Utilities.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Instructions
{
	[TestClass]
	public class CPUShiftInstructionTest
	{
		private CPU cpu;

		[TestInitialize]
		public void TestInitialize()
		{
			cpu = new CPU();
		}


		[TestMethod]
		public void TestShiftLeft()
		{
			var shl = new ShiftInstruction(cpu, new Word { Value = 0b0001_0001_0010 });
			cpu.Registers[1].Data.Value = 0b1000_1101_0010;
			shl.Execute();
			Assert.AreEqual(0b0011_0100_1000, cpu.Registers[1].Data.Value);

			shl = new ShiftInstruction(cpu, new Word { Value = 0b0001_0010_0111 });
			cpu.Registers[2].Data.Value = 0b0000_0000_0010;
			shl.Execute();
			Assert.AreEqual(0b0001_0000_0000, cpu.Registers[2].Data.Value);
		}


		[TestMethod]
		public void TestShiftRight()
		{
			var shr = new ShiftInstruction(cpu, new Word { Value = 0b0001_1000_0001 });
			cpu.Registers[0].Data.Value = 0b0000_1000_0001;
			shr.Execute();
			Assert.AreEqual(0b0000_0100_0000, cpu.Registers[0].Data.Value);

			shr = new ShiftInstruction(cpu, new Word { Value = 0b0001_1011_0011 });
			cpu.Registers[3].Data.Value = 0b1001_0111_0111;
			shr.Execute();
			Assert.AreEqual(0b0001_0010_1110, cpu.Registers[3].Data.Value);
		}


		[TestMethod]
		public void TestShiftRightArithmetic()
		{
			var shra = new ShiftInstruction(cpu, new Word { Value = 0b0001_1100_0001 });
			cpu.Registers[0].Data.Value = 0b0000_1000_0001;
			shra.Execute();
			Assert.AreEqual(0b0000_0100_0000, cpu.Registers[0].Data.Value);

			shra = new ShiftInstruction(cpu, new Word { Value = 0b0001_1111_0011 });
			cpu.Registers[3].Data.Value = 0b1001_0111_0111;
			shra.Execute();
			Assert.AreEqual(SignExtend(0b1111_0010_1110, 11), cpu.Registers[3].Data.Value);
		}
	}
}
