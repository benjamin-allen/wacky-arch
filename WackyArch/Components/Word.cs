namespace WackyArch.Components
{

    /// <summary>
    /// For this project, a word is an Size-bit value that is "sticky" (doesn't overflow) 
    /// </summary>
    public class Word
    {
        public static int Size = 12; // in bits
        public static int Max = (1 << Size - 1) - 1;
        public static int Min = -(1 << Size - 1);

        private int _Value;
        public int Value
        {
            get { return _Value; }
            set { _Value = Utilities.Utilities.SignExtend(value, Size - 1); }
        }

        public int AssignBitwise(int value)
        {
            int term = value & (1 << Size) - 1;
            _Value = Utilities.Utilities.SignExtend(term, Size - 1);
            return term;
        }

        public int ReadAsUnsigned()
        {
            return _Value & (1 << Size) - 1;
        }

        public string ToBin()
        {
            string longForm = Convert.ToString(Value, 2).PadLeft(Size, '0');
            return longForm.Substring(longForm.Length - Size);
        }

        public string ToHex()
        {
            return Convert.ToInt32(ToBin(), 2).ToString("X").PadLeft((int)Math.Ceiling(Size / 4.0), '0');
        }
    }
}
