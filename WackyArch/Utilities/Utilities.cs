namespace WackyArch.Utilities
{
	public static class Utilities
	{
		/// <summary>
		/// Sets the bits above the the <see cref="n"/>th bit to the value of the nth bit. n ranges from 0 to 31, where 0 is the least significant bit.
		/// </summary>
		/// <param name="value">The value to sign-extend</param>
		/// <param name="bit">The bit from which to sign-extend</param>
		/// <returns></returns>
		public static int SignExtend(int value, int bit)
		{
			// Int is a 32-bit value. We need to left-shift by 32-(bit+1)
			int shiftAmt = 32 - (bit + 1);
			return (value << shiftAmt) >> shiftAmt;
		}

		/// <summary>
		/// Center-aligns this string with the specified padding character
		/// </summary>
		/// From https://stackoverflow.com/a/32768784/8286492
		public static string PadSides(this string str, int totalWidth, char paddingChar = ' ')
		{
			int padding = totalWidth - str.Length;
			int padLeft = padding / 2 + str.Length;
			return str.PadLeft(padLeft, paddingChar).PadRight(totalWidth, paddingChar);
		}
	}
}
