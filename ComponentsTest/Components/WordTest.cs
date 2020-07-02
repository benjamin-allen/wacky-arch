using Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Test.Components
{
	[TestClass]
	public class WordTest
	{
		[TestMethod]
		public void InRangeValues()
		{
			Word word = new Word();
			Assert.AreEqual(word.Value, 0);

			for(int i = Word.Min; i <= Word.Max; i++)
			{
				word.Value = i;
				Assert.AreEqual(i, word.Value);
			}
		}

		[TestMethod]
		public void OutOfRangeValues()
		{
			Word word = new Word();
			Assert.AreEqual(0, word.Value);

			Random r = new Random();
			for(int i = 0; i < 1000; i++)
			{
				int x = r.Next(Word.Max + 1, int.MaxValue);
				int y = r.Next(int.MinValue, Word.Min - 1);
				word.Value = x;
				Assert.AreEqual(Word.Max, word.Value);
				word.Value = y;
				Assert.AreEqual(Word.Min, word.Value);
			}
		}

		[TestMethod]
		public void EdgeValues()
		{
			Word word = new Word();

			word.Value = Word.Min;
			Assert.AreEqual(Word.Min, word.Value);
			word.Value = Word.Min - 1;
			Assert.AreEqual(Word.Min, word.Value);

			word.Value = Word.Max;
			Assert.AreEqual(Word.Max, word.Value);
			word.Value = Word.Max + 1;
			Assert.AreEqual(Word.Max, word.Value);
		}

		[TestMethod]
		public void PrintBinValues()
		{
			Word word = new Word();

			// 0
			Assert.AreEqual(new string('0', Word.Size), word.ToBin());

			// Word.Max
			word.Value = Word.Max;
			Assert.AreEqual("0" + new string('1', Word.Size - 1), word.ToBin());

			// Word.Min
			word.Value = Word.Min;
			Assert.AreEqual("1" + new string('0', Word.Size - 1), word.ToBin());
		}

		[TestMethod]
		public void InspectBinValues()
		{
			Trace.Listeners.Add(new ConsoleTraceListener());
			Word word = new Word();

			for(int i = 0; i < Word.Max; i+=16)
			{
				word.Value = i;
				Trace.WriteLine(word.ToBin());
			}
			for(int i = Word.Min; i < 0; i+=16)
			{
				word.Value = i;
				Trace.WriteLine(word.ToBin());
			}
		}

		[TestMethod]
		public void PrintHexValues()
		{
			Word word = new Word();

			// 0
			Assert.AreEqual(new string('0', (int)Math.Ceiling(Word.Size / 4.0)), word.ToHex());

			// Word.Max
			word.Value = Word.Max;
			Assert.AreEqual(Convert.ToString(word.Value, 16).ToUpper(), word.ToHex());

			// Word.Min
			word.Value = Word.Min;
			// We expect the value to be 100....000 in binary, so we need to construct the equivalent hex string.
			int signBitLocation = (Word.Size - 1) % 4;
			string frontDigit = Convert.ToString(1 << signBitLocation, 16);
			string hex = frontDigit.PadRight((int)Math.Ceiling(Word.Size / 4.0), '0');
			Assert.AreEqual(hex.ToUpper(), word.ToHex());

			// 101
			word.Value = 101;
			hex = "65".PadLeft((int)Math.Ceiling(Word.Size / 4.0), '0');
			Assert.AreEqual(hex, word.ToHex());

			// -26
			word.Value = -26;
			hex = "e6".PadLeft((int)Math.Ceiling(Word.Size / 4.0), 'f');
			var frontDigits = new string[] { "F", "1", "3", "7" };
			frontDigit = frontDigits[Word.Size % 4];
			hex = frontDigit + hex.Substring(1);
			Assert.AreEqual(hex.ToUpper(), word.ToHex());
		}

		[TestMethod]
		public void InspectHexValues()
		{
			Trace.Listeners.Add(new ConsoleTraceListener());
			Word word = new Word();

			for (int i = 0; i < Word.Max; i += 16)
			{
				word.Value = i;
				Trace.WriteLine(word.ToHex());
			}
			for (int i = Word.Min; i < 0; i += 16)
			{
				word.Value = i;
				Trace.WriteLine(word.ToHex());
			}
		}
	}
}
