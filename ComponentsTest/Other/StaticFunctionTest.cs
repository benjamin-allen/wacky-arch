using Microsoft.VisualStudio.TestTools.UnitTesting;
using static WackyArch.Utilities.Utilities;
using WackyArch.Components;

namespace Test.Other
{
	[TestClass]
	public class StaticFunctionTest
	{
		[TestMethod]
		public void TestSignExtend()
		{
			Assert.AreEqual(-16,    SignExtend(0b0000_1111_0000_1111_0000_1111_0001_0000, 4));
			
			Assert.AreEqual(1,      SignExtend(unchecked((int)0b1111_1111_1111_1111_1111_1111_1111_1101), 1));

			Assert.AreEqual(1,      SignExtend(unchecked((int)0b1000_0000_0000_0000_0000_0000_0000_0001), 30));

			Assert.AreEqual(2047,   SignExtend(0b0111_1111_1111, Word.Size - 1));

			Assert.AreEqual(-2048,  SignExtend(0b1000_0000_0000, Word.Size - 1));

			Assert.AreEqual(-2047,  SignExtend(0b0111_1111_1111 + 2, Word.Size - 1));
		}
	}
}
