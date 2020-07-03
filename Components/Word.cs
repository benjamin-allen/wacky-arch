using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components
{

	/// <summary>
	/// For this project, a word is an Size-bit value that is "sticky" (doesn't overflow) 
	/// </summary>
	public class Word
	{
		public static int Size = 12; // in bits
		public static int Max = (1 << (Size - 1)) - 1;
		public static int Min = -(1 << (Size - 1));

		private int _Value;
		public int Value
		{
			get { return _Value; }
			set { _Value = Bound(value); }
		}

		private int Bound(int value)
		{
			if(value > Max)
			{
				return Max;
			}
			else if(value < Min)
			{
				return Min;
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
