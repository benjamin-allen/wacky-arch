using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Instructions;
using static WackyArch.Utilities.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Instructions
{
	[TestClass]
	public class CPUConstInstructionTest
	{
		private CPU cpu;

		[TestInitialize]
		public void TestInitialize()
		{
			cpu = new CPU();
		}


		[TestMethod]
		public void TestAddConst()
		{
			var addc = new ConstInstruction(cpu, new Word { Value = 0b1010_11111111 });
			addc.Execute();
			Assert.AreEqual(255, cpu.Const.Data.Value);

			addc.Execute();
			Assert.AreEqual(510, cpu.Const.Data.Value);

			addc = new ConstInstruction(cpu, new Word { Value = 0b1010_00000001 });
			addc.Execute();
			Assert.AreEqual(511, cpu.Const.Data.Value);

			cpu.Const.Data.Value = Word.Max;
			addc.Execute();
			Assert.AreEqual(Word.Min, cpu.Const.Data.Value);
		}


		[TestMethod]
		public void TestSubConst()
		{
			var subc = new ConstInstruction(cpu, new Word { Value = 0b1011_11111111 });
			subc.Execute();
			Assert.AreEqual(-255, cpu.Const.Data.Value);

			subc.Execute();
			Assert.AreEqual(-510, cpu.Const.Data.Value);

			cpu.Const.Data.Value = Word.Min;
			subc.Execute();
			Assert.AreEqual(Word.Max - 254, cpu.Const.Data.Value);
		}

		
		[TestMethod]
		public void TestModConst()
		{
			var modc = new ConstInstruction(cpu, new Word { Value = 0b1100_00000101 });
			modc.Execute();
			Assert.AreEqual(0, cpu.Const.Data.Value);

			cpu.Const.Data.Value = 6;
			modc.Execute();
			Assert.AreEqual(1, cpu.Const.Data.Value);

			cpu.Const.Data.Value = 77;
			modc.Execute();
			Assert.AreEqual(2, cpu.Const.Data.Value);

			cpu.Const.Data.Value = 2004;
			modc.Execute();
			Assert.AreEqual(4, cpu.Const.Data.Value);

			cpu.Const.Data.Value = -98;
			modc.Execute();
			Assert.AreEqual(-3, cpu.Const.Data.Value);
		}


		[TestMethod]
		public void TestAndConst()
		{
			var andc = new ConstInstruction(cpu, new Word { Value = 0b1101_10100011 });
			andc.Execute();
			Assert.AreEqual(0, cpu.Const.Data.Value);

			cpu.Const.Data.Value = 0b1110_1100_0010;
			andc.Execute();      //  1111_1010_0011;
			Assert.AreEqual(SignExtend(0b1110_1000_0010, Word.Size - 1), cpu.Const.Data.Value);

			andc = new ConstInstruction(cpu, new Word { Value = 0b1101_00000001 });
			cpu.Const.Data.Value = 0b0000_1111_1111;
			andc.Execute();
			Assert.AreEqual(1, cpu.Const.Data.Value);
		}


		[TestMethod]
		public void TestOrConst()
		{
			var orc = new ConstInstruction(cpu, new Word { Value = 0b1110_10101010 });
			orc.Execute();
			Assert.AreEqual(SignExtend(0b111110101010, Word.Size - 1), cpu.Const.Data.Value);

			orc.Execute();
			Assert.AreEqual(SignExtend(0b111110101010, Word.Size - 1), cpu.Const.Data.Value);

			orc = new ConstInstruction(cpu, new Word { Value = 0b1110_00000001 });
			orc.Execute();
			Assert.AreEqual(SignExtend(0b111110101011, Word.Size - 1), cpu.Const.Data.Value);
		}

		[TestMethod]
		public void TestMovConst()
		{
			var movc = new ConstInstruction(cpu, new Word { Value = 0b1111_01010000 });
			movc.Execute();
			Assert.AreEqual(80, cpu.Const.Data.Value);

			movc = new ConstInstruction(cpu, new Word { Value = 0b1111_10000000 });
			movc.Execute();
			Assert.AreEqual(-128, cpu.Const.Data.Value);
		}
	}
}
