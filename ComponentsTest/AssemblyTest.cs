using Assembler;
using Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test
{
	[TestClass]
	public class AssemblyTest
	{
		private CPU.CPU cpu;

		[TestInitialize]
		public void TestInitialize()
		{
			cpu = new CPU.CPU();
		}


		[TestMethod]
		public void FibonacciProgram()
		{
			string program = String.Join(Environment.NewLine,
				"MOVC 1",
				"MOV R0 CONST",
				"MOV R1 CONST",
				"# SET UP THA LOOP",
				"MOVC 10",
				"",
				"# COMPUTE THE NEXT FIBONACCI NUMBER AND STORE IT INTO R0",
				"@FIB",
				"ADD R0 R1",
				"SWP R0 R1",
				"SUBC 1",
				"JGZ @FIB");

			List<Word> binary = Assembler.Assembler.Assemble(cpu, program);

			Assert.AreEqual(0b1111_0000_0001, binary[0].Value & 0xFFF);
			Assert.AreEqual(0b0011_0011_0000, binary[1].Value & 0xFFF);
			Assert.AreEqual(0b0011_0111_0000, binary[2].Value & 0xFFF);
			Assert.AreEqual(0b1111_0000_1010, binary[3].Value & 0xFFF);
			Assert.AreEqual(0b0000_0001_0000, binary[4].Value & 0xFFF);
			Assert.AreEqual(0b0011_0001_0001, binary[5].Value & 0xFFF);
			Assert.AreEqual(0b1011_0000_0001, binary[6].Value & 0xFFF);
			Assert.AreEqual(0b0111_1111_1101, binary[7].Value & 0xFFF);
		}
	}
}
