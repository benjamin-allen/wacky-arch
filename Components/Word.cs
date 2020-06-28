using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{

	/// <summary>
	/// For this project, a word is an 11-bit value that is "sticky" (doesn't overflow) 
	/// </summary>
	public class Word
	{
		public static int Size = 11; // in bits

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

		public string ToBin()
		{
			string longForm = Convert.ToString(Value, 2).PadLeft(Size, '0');
			return longForm.Substring(longForm.Length - Size);
		}

		public string ToHex()
		{
			return Convert.ToInt32(ToBin(), 2).ToString("X").PadLeft((int)Math.Ceiling(Word.Size / 4.0), '0');
		}
	}
}
