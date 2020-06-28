using Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.CompilerServices;

namespace ComponentsTest
{
	[TestClass]
	public class WordTest
	{
		private static int Min = -(1 << (Word.Size - 1));
		private static int Max = (1 << (Word.Size - 1)) - 1;

		[TestMethod]
		public void InRangeValues()
		{
			Word word = new Word();
			Assert.AreEqual(word.Value, 0);

			for(int i = Min; i <= Max; i++)
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
				int x = r.Next(Max + 1, int.MaxValue);
				int y = r.Next(int.MinValue, Min - 1);
				word.Value = x;
				Assert.AreEqual(Max, word.Value);
				word.Value = y;
				Assert.AreEqual(Min, word.Value);
			}
		}

		[TestMethod]
		public void EdgeValues()
		{
			Word word = new Word();

			word.Value = Min;
			Assert.AreEqual(Min, word.Value);
			word.Value = Min - 1;
			Assert.AreEqual(Min, word.Value);

			word.Value = Max;
			Assert.AreEqual(Max, word.Value);
			word.Value = Max + 1;
			Assert.AreEqual(Max, word.Value);
		}
	}
}
