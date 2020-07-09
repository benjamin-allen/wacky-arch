using Components;
using CPU.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Instructions
{
	[TestClass]
	public class CPURegisterInstructionTest
	{
		private CPU.CPU cpu;

		[TestInitialize]
		public void TestInitialize()
		{
			cpu = new CPU.CPU();
		}


		[TestMethod]
		public void TestMoveInstruction()
		{
			cpu.Registers[0].Data.Value = 9;
			cpu.Registers[1].Data.Value = 1;
			cpu.Registers[2].Data.Value = -9;
			cpu.Registers[3].Data.Value = -10;

			var mov1 = new RegisterInstruction(cpu, new Word { Value = 0b0011_0001_0000 });
			var mov2 = new RegisterInstruction(cpu, new Word { Value = 0b0011_1110_0000 });

			mov1.Execute();
			mov2.Execute();

			Assert.AreEqual(1, cpu.Registers[0].Data.Value);
			Assert.AreEqual(1, cpu.Registers[1].Data.Value);
			Assert.AreEqual(-9, cpu.Registers[2].Data.Value);
			Assert.AreEqual(-9, cpu.Registers[3].Data.Value);
		}


		[TestMethod]
		public void TestSwapInstruction()
		{
			cpu.Registers[0].Data.Value = 0;
			cpu.Registers[1].Data.Value = 1;
			cpu.Registers[2].Data.Value = Word.Max;
			cpu.Registers[3].Data.Value = Word.Min;

			var swp1 = new RegisterInstruction(cpu, new Word { Value = 0b0011_0001_0001 });
			var swp1a = new RegisterInstruction(cpu, new Word { Value = 0b0011_0100_0001 });
			var swp2 = new RegisterInstruction(cpu, new Word { Value = 0b0011_1011_0001 });
			var swp2a = new RegisterInstruction(cpu, new Word { Value = 0b0011_1110_0001 });

			swp1.Execute();
			swp2.Execute();

			Assert.AreEqual(1, cpu.Registers[0].Data.Value);
			Assert.AreEqual(0, cpu.Registers[1].Data.Value);
			Assert.AreEqual(Word.Min, cpu.Registers[2].Data.Value);
			Assert.AreEqual(Word.Max, cpu.Registers[3].Data.Value);

			swp1a.Execute();
			swp2a.Execute();

			Assert.AreEqual(0, cpu.Registers[0].Data.Value);
			Assert.AreEqual(1, cpu.Registers[1].Data.Value);
			Assert.AreEqual(Word.Max, cpu.Registers[2].Data.Value);
			Assert.AreEqual(Word.Min, cpu.Registers[3].Data.Value);
		}

		[TestMethod]
		public void TestCompareInstruction()
		{
			cpu.Registers[0].Data.Value = 0;
			cpu.Registers[1].Data.Value = Word.Max;
			cpu.Registers[2].Data.Value = Word.Min;

			var cmpEq = new RegisterInstruction(cpu, new Word { Value = 0b0011_0000_0010 });
			var cmpLesser = new RegisterInstruction(cpu, new Word { Value = 0b0011_0001_0010 });
			var cmpGreater = new RegisterInstruction(cpu, new Word { Value = 0b0011_0010_0010 });

			cmpEq.Execute();
			Assert.AreEqual(0, cpu.Const.Data.Value);

			cmpLesser.Execute();
			Assert.AreEqual(-1, cpu.Const.Data.Value);

			cmpGreater.Execute();
			Assert.AreEqual(1, cpu.Const.Data.Value);
		}
	}
}
