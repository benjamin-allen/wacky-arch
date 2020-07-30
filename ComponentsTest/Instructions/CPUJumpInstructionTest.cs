using Components;
using CPU.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Utilities.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace Test.Instructions
{
	[TestClass]
	public class CPUJumpInstructionTest
	{
		private CPU.CPU cpu;

		[TestInitialize]
		public void TestInitialize()
		{
			cpu = new CPU.CPU();
		}

		[TestMethod]
		public void TestJumpInstruction()
		{
			var jmp = new JumpInstruction(cpu, new Word { Value = 0b0101_01111111 });
			jmp.Execute();
			Assert.AreEqual(127, cpu.GetPCValue());

			jmp = new JumpInstruction(cpu, new Word { Value = 0b0101_11111001 });
			jmp.Execute();
			Assert.AreEqual(120, cpu.GetPCValue());

			jmp = new JumpInstruction(cpu, new Word { Value = 0b0101_10000000 });
			jmp.Execute();
			Assert.AreEqual(0, cpu.GetPCValue());

			cpu.SetPCValue(Word.Max - 1);
			jmp = new JumpInstruction(cpu, new Word { Value = 0b0101_00001000 });
			jmp.Execute();
			Assert.AreEqual(Word.Max, cpu.GetPCValue());
		}

		[TestMethod]
		public void TestJumpIfZero()
		{
			var jez = new JumpInstruction(cpu, new Word { Value = 0b0110_00000010 });
			jez.Execute();
			Assert.AreEqual(2, cpu.GetPCValue());

			cpu.Const.Data.Value = 1;
			jez.Execute();
			Assert.AreEqual(2, cpu.GetPCValue());

			cpu.Const.Data.Value = -1;
			jez.Execute();
			Assert.AreEqual(2, cpu.GetPCValue());

			cpu.Const.Data.Value = 0;
			jez.Execute();
			Assert.AreEqual(4, cpu.GetPCValue());

			cpu.SetPCValue(Word.Max);
			jez.Execute();
			Assert.AreEqual(Word.Max, cpu.GetPCValue());

			cpu.SetPCValue(Word.Min);
			Assert.AreEqual(0, cpu.GetPCValue());
			jez.Execute();
			Assert.AreEqual(2, cpu.GetPCValue());
		}

		[TestMethod]
		public void TestJumpIfGreater()
		{
			var jgz = new JumpInstruction(cpu, new Word { Value = 0b0111_01111111 });
			jgz.Execute();
			Assert.AreEqual(0, cpu.GetPCValue());

			cpu.Const.Data.Value = 1;
			jgz.Execute();
			Assert.AreEqual(127, cpu.GetPCValue());

			cpu.Const.Data.Value = -1;
			jgz.Execute();
			Assert.AreEqual(127, cpu.GetPCValue());
		}

		[TestMethod]
		public void TestJumpIfLesser()
		{
			var jlz = new JumpInstruction(cpu, new Word { Value = SignExtend(0b1000_01000000, Word.Size - 1) });
			jlz.Execute();
			Assert.AreEqual(0, cpu.GetPCValue());

			cpu.Const.Data.Value = 1;
			jlz.Execute();
			Assert.AreEqual(0, cpu.GetPCValue());

			cpu.Const.Data.Value = -1;
			jlz.Execute();
			Assert.AreEqual(64, cpu.GetPCValue());
		}

		[TestMethod]
		public void TestJumpAddress()
		{
			cpu.Registers[0].Data.Value = 2;
			cpu.Registers[1].Data.Value = 4;
			cpu.Registers[2].Data.Value = 8;
			cpu.Registers[3].Data.Value = 16;

			var ja = new JumpInstruction(cpu, new Word { Value = SignExtend(0b1001_0000_0001, Word.Size - 1) });
			ja.Execute();
			Assert.AreEqual(4, cpu.GetPCValue());

			ja = new JumpInstruction(cpu, new Word { Value = SignExtend(0b1001_00000000, Word.Size - 1) });
			ja.Execute();
			Assert.AreEqual(2, cpu.GetPCValue());

			ja = new JumpInstruction(cpu, new Word { Value = SignExtend(0b1001_00000010, Word.Size - 1) });
			ja.Execute();
			Assert.AreEqual(8, cpu.GetPCValue());

			ja = new JumpInstruction(cpu, new Word { Value = SignExtend(0b1001_00000011, Word.Size - 1) });
			ja.Execute();
			Assert.AreEqual(16, cpu.GetPCValue());
		}
	}
}
