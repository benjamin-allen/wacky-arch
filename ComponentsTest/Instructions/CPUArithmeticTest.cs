using Components;
using CPU.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static Utilities.Utilities;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
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
			var addInsn = new ArithmeticInstruction(cpu, insn);

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

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_0000 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1011_0000 });

			insn1.Execute();
			insn2.Execute();
			Assert.AreEqual(Word.Min + 4, cpu.Registers[0].Data.Value);
			Assert.AreEqual(Word.Max - 4, cpu.Registers[2].Data.Value);
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
				var insn = new ArithmeticInstruction(cpu, insnWord);
				insn.Execute();

				Assert.AreEqual(sums[i], cpu.Registers[regs1[i]].Data.Value);
				Assert.AreEqual(addends2[i], cpu.Registers[regs2[i]].Data.Value);
			}
		}


		[TestMethod]
		public void TestSubBasic()
		{
			cpu.Registers[0].Data.Value = 45;
			cpu.Registers[1].Data.Value = 47;
			cpu.Registers[2].Data.Value = 2;
			cpu.Registers[3].Data.Value = 3;

			var subInsn = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_0001 });
			subInsn.Execute();

			Assert.AreEqual(-2, cpu.Registers[0].Data.Value);
			Assert.AreEqual(47, cpu.Registers[1].Data.Value);
			Assert.AreEqual(2, cpu.Registers[2].Data.Value);
			Assert.AreEqual(3, cpu.Registers[3].Data.Value);
		}


		[TestMethod]
		public void TestSubEdgeCases()
		{
			cpu.Registers[0].Data.Value = Word.Max;
			cpu.Registers[1].Data.Value = -2;
			cpu.Registers[2].Data.Value = Word.Min;
			cpu.Registers[3].Data.Value = 2;

			var subInsn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_0001 });
			var subInsn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1011_0001 });
			subInsn1.Execute();
			subInsn2.Execute();

			Assert.AreEqual(Word.Min + 1, cpu.Registers[0].Data.Value);
			Assert.AreEqual(Word.Max - 1, cpu.Registers[2].Data.Value);
		}


		[TestMethod]
		public void TestSubExtended()
		{
			Random r = new Random();
			int n = 1 << r.Next(4, 11);
			int[] subtends = new int[n];
			int[] subtrahends = new int[n];
			int[] regs1 = new int[n];
			int[] regs2 = new int[n];
			int[] differences = new int[n];
			for (int i = 0; i < n; i++)
			{
				subtends[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				subtrahends[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				regs1[i] = r.Next(0, 4);
				regs2[i] = r.Next(0, 4);
				if (regs2[i] == regs1[i])
				{
					regs2[i] = (regs2[i] + 1) % 4;
				}
				differences[i] = subtends[i] - subtrahends[i];
			}

			for (int i = 0; i < n; i++)
			{
				cpu.Registers[regs1[i]].Data.Value = subtends[i];
				cpu.Registers[regs2[i]].Data.Value = subtrahends[i];
				int x = regs1[i];
				int y = regs2[i];
				Assert.AreNotEqual(x, y);
				Word insnWord = new Word { Value = (x << 6) + (y << 4) + 1 };
				var insn = new ArithmeticInstruction(cpu, insnWord);
				insn.Execute();

				Assert.AreEqual(differences[i], cpu.Registers[regs1[i]].Data.Value);
				Assert.AreEqual(subtrahends[i], cpu.Registers[regs2[i]].Data.Value);
			}
		}

		
		[TestMethod]
		public void TestMulBasic()
		{
			cpu.Registers[0].Data.Value = 2;
			cpu.Registers[1].Data.Value = 3;
			cpu.Registers[2].Data.Value = -5;
			cpu.Registers[3].Data.Value = 3;

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0111_0010 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1000_0010 });

			insn1.Execute();
			Assert.AreEqual(9, cpu.Registers[1].Data.Value);
			Assert.AreEqual(3, cpu.Registers[3].Data.Value);

			insn2.Execute();
			Assert.AreEqual(-10, cpu.Registers[2].Data.Value);
			Assert.AreEqual(2, cpu.Registers[0].Data.Value);
		}


		[TestMethod]
		public void TestMulEdgeCases()
		{
			cpu.Registers[0].Data.Value = 1024;
			cpu.Registers[1].Data.Value = 4;
			cpu.Registers[2].Data.Value = -1024;

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_0010 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1001_0010 });

			insn1.Execute();
			Assert.AreEqual(0, cpu.Registers[0].Data.Value);
			Assert.AreEqual(4, cpu.Registers[1].Data.Value);

			insn2.Execute();
			Assert.AreEqual(0, cpu.Registers[2].Data.Value);
			Assert.AreEqual(4, cpu.Registers[1].Data.Value);
		}


		[TestMethod]
		public void TestMulExtended()
		{
			Random r = new Random();
			int n = 1 << r.Next(4, 11);
			int[] factors1 = new int[n];
			int[] factors2 = new int[n];
			int[] regs1 = new int[n];
			int[] regs2 = new int[n];
			int[] products = new int[n];
			for (int i = 0; i < n; i++)
			{
				factors1[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				factors2[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				regs1[i] = r.Next(0, 4);
				regs2[i] = r.Next(0, 4);
				if (regs2[i] == regs1[i])
				{
					regs2[i] = (regs2[i] + 1) % 4;
				}
				products[i] = SignExtend(factors1[i] * factors2[i], Word.Size - 1);
			}

			for (int i = 0; i < n; i++)
			{
				cpu.Registers[regs1[i]].Data.Value = factors1[i];
				cpu.Registers[regs2[i]].Data.Value = factors2[i];
				int x = regs1[i];
				int y = regs2[i];
				Assert.AreNotEqual(x, y);
				Word insnWord = new Word { Value = (x << 6) + (y << 4) + 2 };
				var insn = new ArithmeticInstruction(cpu, insnWord);
				insn.Execute();

				Assert.AreEqual(products[i], cpu.Registers[regs1[i]].Data.Value);
				Assert.AreEqual(factors2[i], cpu.Registers[regs2[i]].Data.Value);
			}
		}


		[TestMethod]
		public void TestDivBasic()
		{
			cpu.Registers[0].Data.Value = 12;
			cpu.Registers[1].Data.Value = 5;
			cpu.Registers[2].Data.Value = 8;
			cpu.Registers[3].Data.Value = -2;

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_0011 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1011_0011 });

			insn1.Execute();
			insn2.Execute();
			Assert.AreEqual(2, cpu.Registers[0].Data.Value);
			Assert.AreEqual(2, cpu.Registers[1].Data.Value);
			Assert.AreEqual(-4, cpu.Registers[2].Data.Value);
			Assert.AreEqual(0, cpu.Registers[3].Data.Value);
		}


		[TestMethod]
		public void TestDivExtended()
		{
			Random r = new Random();
			int n = 1 << r.Next(4, 11);
			int[] dividends = new int[n];
			int[] divisors = new int[n];
			int[] regs1 = new int[n];
			int[] regs2 = new int[n];
			int[] quotients = new int[n];
			int[] remainders = new int[n];
			for (int i = 0; i < n; i++)
			{
				dividends[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				divisors[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				while(divisors[i] == 0)
				{
					divisors[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				}
				regs1[i] = r.Next(0, 4);
				regs2[i] = r.Next(0, 4);
				if (regs2[i] == regs1[i])
				{
					regs2[i] = (regs2[i] + 1) % 4;
				}
				quotients[i] = dividends[i] / divisors[i];
				remainders[i] = dividends[i] % divisors[i];
			}

			for (int i = 0; i < n; i++)
			{
				cpu.Registers[regs1[i]].Data.Value = dividends[i];
				cpu.Registers[regs2[i]].Data.Value = divisors[i];
				int x = regs1[i];
				int y = regs2[i];
				Assert.AreNotEqual(x, y);
				Word insnWord = new Word { Value = (x << 6) + (y << 4) + 3 };
				var insn = new ArithmeticInstruction(cpu, insnWord);
				insn.Execute();

				Assert.AreEqual(quotients[i], cpu.Registers[regs1[i]].Data.Value);
				Assert.AreEqual(remainders[i], cpu.Registers[regs2[i]].Data.Value);
			}
		}


		[TestMethod]
		public void TestModBasic()
		{
			cpu.Registers[0].Data.Value = 8;
			cpu.Registers[1].Data.Value = 3;
			cpu.Registers[2].Data.Value = 9;
			cpu.Registers[3].Data.Value = 1;

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_0100 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1011_0100 });

			insn1.Execute();
			insn2.Execute();

			Assert.AreEqual(2, cpu.Registers[0].Data.Value);
			Assert.AreEqual(0, cpu.Registers[2].Data.Value);
		}


		[TestMethod]
		public void TestModExtended()
		{
			Random r = new Random();
			int n = 1 << r.Next(4, 11);
			int[] numbers = new int[n];
			int[] moduli = new int[n];
			int[] regs1 = new int[n];
			int[] regs2 = new int[n];
			int[] congruences = new int[n];
			for (int i = 0; i < n; i++)
			{
				numbers[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				moduli[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				while(moduli[i] == 0)
				{
					moduli[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				}
				regs1[i] = r.Next(0, 4);
				regs2[i] = r.Next(0, 4);
				if (regs2[i] == regs1[i])
				{
					regs2[i] = (regs2[i] + 1) % 4;
				}
				congruences[i] = numbers[i] % moduli[i];
			}

			for (int i = 0; i < n; i++)
			{
				cpu.Registers[regs1[i]].Data.Value = numbers[i];
				cpu.Registers[regs2[i]].Data.Value = moduli[i];
				int x = regs1[i];
				int y = regs2[i];
				Assert.AreNotEqual(x, y);
				Word insnWord = new Word { Value = (x << 6) + (y << 4) + 4 };
				var insn = new ArithmeticInstruction(cpu, insnWord);
				insn.Execute();

				Assert.AreEqual(congruences[i], cpu.Registers[regs1[i]].Data.Value);
				Assert.AreEqual(moduli[i], cpu.Registers[regs2[i]].Data.Value);
			}
		}

		[TestMethod]
		public void TestNegBasic()
		{
			cpu.Registers[0].Data.Value = 0;
			cpu.Registers[1].Data.Value = 1;
			cpu.Registers[2].Data.Value = Word.Max;
			cpu.Registers[3].Data.Value = Word.Min;

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0000_0101 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_0101 });
			var insn3 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0010_0101 });
			var insn4 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0011_0101 });

			insn1.Execute();
			insn2.Execute();
			insn3.Execute();
			insn4.Execute();

			Assert.AreEqual(0, cpu.Registers[0].Data.Value); // Edge case: Negating 0 is 0
			Assert.AreEqual(-1, cpu.Registers[1].Data.Value);
			Assert.AreEqual(Word.Min + 1, cpu.Registers[2].Data.Value);
			Assert.AreEqual(Word.Min, cpu.Registers[3].Data.Value); // Edge case: Negating -2048 is 2048 which wraps to become -2048
		}


		[TestMethod]
		public void TestNegExtended()
		{
			Random r = new Random();
			int n = 1 << r.Next(4, 11);
			int[] numbers = new int[n];
			int[] regs1 = new int[n];
			int[] negatives = new int[n];
			for (int i = 0; i < n; i++)
			{
				numbers[i] = r.Next((Word.Min / 2) + 1, (Word.Max / 2) - 1);
				regs1[i] = r.Next(0, 4);
				negatives[i] = -numbers[i];
			}

			for (int i = 0; i < n; i++)
			{
				cpu.Registers[regs1[i]].Data.Value = numbers[i];
				int x = regs1[i];
				Word insnWord = new Word { Value = (x << 4) + 5 };
				var insn = new ArithmeticInstruction(cpu, insnWord);
				insn.Execute();

				Assert.AreEqual(negatives[i], cpu.Registers[regs1[i]].Data.Value);
			}
		}


		[TestMethod]
		public void TestAndBasic()
		{
			cpu.Registers[0].Data.AssignBitwise(0b001100110011);
			cpu.Registers[1].Data.AssignBitwise(0b010101010101);
			cpu.Registers[2].Data.AssignBitwise(0b000010101011);
			cpu.Registers[3].Data.AssignBitwise(0b111110001111);

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_1010 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1011_1010 });

			insn1.Execute();
			insn2.Execute();

			Assert.AreEqual(0b000100010001, cpu.Registers[0].Data.Value);
			Assert.AreEqual(0b000010001011, cpu.Registers[2].Data.Value);
		}


		[TestMethod]
		public void TestOrBasic()
		{
			cpu.Registers[0].Data.AssignBitwise(0b001100110011);
			cpu.Registers[1].Data.AssignBitwise(0b010101010101);
			cpu.Registers[2].Data.AssignBitwise(0b000010101011);
			cpu.Registers[3].Data.AssignBitwise(0b111110001111);

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_1011 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1011_1011 });

			insn1.Execute();
			insn2.Execute();

			Assert.AreEqual(0b011101110111, cpu.Registers[0].Data.Value);
			Assert.AreEqual(0b111110101111, cpu.Registers[2].Data.Value);
		}


		[TestMethod]
		public void TestXorBasic()
		{
			cpu.Registers[0].Data.AssignBitwise(0b001100110011);
			cpu.Registers[1].Data.AssignBitwise(0b010101010101);
			cpu.Registers[2].Data.AssignBitwise(0b000010101011);
			cpu.Registers[3].Data.AssignBitwise(0b111110001111);

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_1100 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_1011_1100 });

			insn1.Execute();
			insn2.Execute();

			Assert.AreEqual(0b011001100110, cpu.Registers[0].Data.Value);
			Assert.AreEqual(0b111100100100, cpu.Registers[2].Data.Value);
		}


		[TestMethod]
		public void TestNotBasic()
		{
			cpu.Registers[0].Data.AssignBitwise(0b001100110011);
			cpu.Registers[1].Data.AssignBitwise(0b010101010101);
			cpu.Registers[2].Data.AssignBitwise(0b000010101011);
			cpu.Registers[3].Data.AssignBitwise(0b111110001111);

			var insn1 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0000_1101 });
			var insn2 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0001_1101 });
			var insn3 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0010_1101 });
			var insn4 = new ArithmeticInstruction(cpu, new Word { Value = 0b0000_0011_1101 });

			insn1.Execute();
			insn2.Execute();
			insn3.Execute();
			insn4.Execute();

			Assert.AreEqual(0b110011001100, cpu.Registers[0].Data.Value);
			Assert.AreEqual(0b101010101010, cpu.Registers[1].Data.Value);
			Assert.AreEqual(0b111101010100, cpu.Registers[2].Data.Value);
			Assert.AreEqual(0b000001110000, cpu.Registers[3].Data.Value);
		}
	}
}
