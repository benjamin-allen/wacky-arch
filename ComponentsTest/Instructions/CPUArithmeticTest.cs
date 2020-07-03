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
		private CPU.CPU cpu;

		[TestInitialize]
		public void TestInitialize()
		{
			cpu = new CPU.CPU();
		}

		[TestMethod]
		public void TestAddBasic()
		{
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

		[TestMethod]
		public void TestAddEdgeCases()
		{
			cpu.Registers[0].Data.Value = Word.Max - 5;
			cpu.Registers[1].Data.Value = 10;
			cpu.Registers[2].Data.Value = Word.Min + 5;
			cpu.Registers[3].Data.Value = -10;

			var insn1 = new AddInstruction(cpu, new Word { Value = 0b0000_0001_0000 });
			var insn2 = new AddInstruction(cpu, new Word { Value = 0b0000_1011_0000 });

			insn1.Execute();
			insn2.Execute();
			Assert.AreEqual(Word.Max, cpu.Registers[0].Data.Value);
			Assert.AreEqual(Word.Min, cpu.Registers[2].Data.Value);
		}

		[TestMethod]
		public void TestAddExtended()
		{
			Random r = new Random();
			int n = 1 << r.Next(4, 11);
			int[] addends1 = new int[n];
			int[] addends2 = new int[n];
			int[] regs1 = new int[n];
			int[] regs2 = new int[n];
			int[] sums = new int[n];
			for(int i = 0; i < n; i++)
			{
				addends1[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				addends2[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				regs1[i] = r.Next(0, 4);
				regs2[i] = r.Next(0, 4);
				if(regs2[i] == regs1[i])
				{
					regs2[i] = (regs2[i] + 1) % 4;
				}
				sums[i] = addends1[i] + addends2[i];
			}

			for(int i = 0; i < n; i++)
			{
				cpu.Registers[regs1[i]].Data.Value = addends1[i];
				cpu.Registers[regs2[i]].Data.Value = addends2[i];
				int x = regs1[i];
				int y = regs2[i];
				Assert.AreNotEqual(x, y);
				Word insnWord = new Word { Value = (x << 6) + (y << 4) };
				AddInstruction insn = new AddInstruction(cpu, insnWord);
				insn.Execute();

				Assert.AreEqual(sums[i], cpu.Registers[regs1[i]].Data.Value);
				Assert.AreEqual(addends2[i], cpu.Registers[regs2[i]].Data.Value);
			}
		}
	}
}
