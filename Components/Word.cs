using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{

	/// <summary>
	/// For this project, a word is a 8-bit value that overflows 
	/// </summary>
	public class Word
	{
		public static int Size = 8; // in bits

		private int _Value;
		public int Value
		{
			get { return _Value; }
			set { _Value = Bound(value); }
		}

		private int Bound(int value)
		{
			int max = (1 << (Size - 1)) - 1;
			int min = -(1 << (Size - 1));
			if(value > max)
			{
				return max;
			}
			else if(value < min)
			{
				return min;
			}
			return value;
		}
	}
}
