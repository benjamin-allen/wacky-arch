using Components;
using CPU.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Instructions
{
	[TestClass]
	public class CPUArithmeticTest
	{
		[TestMethod]
		public void TestAddBasic()
		{
			CPU.CPU cpu = new CPU.CPU();
			cpu.Registers[0].Data.Value = 45;
			cpu.Registers[1].Data.Value = 47;
			cpu.Registers[2].Data.Value = -1;
			cpu.Registers[3].Data.Value = -2;

			Word insn = new Word { Value = 0b0000_0001_0000 };
			var addInsn = new AddInstruction(cpu, insn);

			addInsn.Execute();
			Assert.AreEqual(92, cpu.Registers[0].Data.Value);
			Assert.AreEqual(47, cpu.Registers[1].Data.Value);
			Assert.AreEqual(-1, cpu.Registers[2].Data.Value);
			Assert.AreEqual(-2, cpu.Registers[3].Data.Value);
		}
	}
}
