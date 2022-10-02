namespace WackyArch.Components
{
    /// <summary>
    /// Memory is just a batch of words that can be accessed positionally
    /// </summary>
    public class Memory : ICyclable
    {
        public Word[] Words;
        public Pipe Address;
        public AlwaysWriteablePipe Data;
        private Word LastAddress;

        public Memory(int numWords)
        {
            // numWords should be a power of 2
            if (numWords <= 0 || !((numWords & numWords - 1) == 0))
            {
                throw new ArgumentException("NumWords must be positive");
            }
            if ((numWords & numWords - 1) == 0 == false)
            {
                throw new ArgumentException("NumWords must be a power of 2");
            }
            if (numWords > Word.Max + 1) // Max address is numWords - 1
            {
                throw new ArgumentException("NumWords must be less than or equal to " + (Word.Max + 1));
            }

            LastAddress = new Word { Value = 0 };
            Words = new Word[numWords];
            Address = new Pipe() { Name = "Addr" };
            Data = new AlwaysWriteablePipe() { Name = "Data" };

            Fill(0);
        }

        /// <summary>
        /// If a value has just been written into the memory address pipe, fetch the data and write it to the data pipe.
        /// Then, if a value has just been written into the memory data pipe, write the data to the cell in memory
        /// </summary>
        public void Cycle()
        {
            // Long story short, if the address is written to, the memory will fetch data from there and back it up,
            // along with outputting it to the data column. If the Data is written to, then the memory will write
            // to the last address written to the pipe.
            if (Address.Status == PipeStatus.AwaitingRead)
            {
                LastAddress = Address.Read(out _); // we know the read will be successful
                if (LastAddress.Value < 0)
                {
                    throw new InvalidOperationException("Memory can't be read from negative addresses");
                }
                if (LastAddress.Value > Words.Length)
                {
                    throw new InvalidOperationException("Memory can't be read from addresses larger than " + Words.Length);
                }
                int val = Words[LastAddress.Value].Value;

                Data.Write(val); // we know the write will be successful
            }
            else if (Data.Status == PipeStatus.AwaitingRead)
            {
                // read the value from the pipe and write it to memory. 
                // TODO: Maybe if I just write the value back after reading it'll be okay? I'll try without at first
                // Nope, that didn't work. Having any cycles between writing addr and reading data caused it to fail. We'll try
                // writing it back as a dirty hack
                // That took care of the delays.
                Word word = Data.Read(out _);
                Words[LastAddress.Value].Value = word.Value;
                Data.Write(word.Value);
            }
        }

        public void Reset()
        {
            Fill(0);
            LastAddress = new Word { Value = 0 };
        }


        /// <summary>
        /// Fill the memory with words of a preset value
        /// </summary>
        public void Fill(int value)
        {
            for (int i = 0; i < Words.Length; i++)
            {
                if (Words[i] == null)
                {
                    Words[i] = new Word();
                }
                Words[i].Value = value;
            }
        }
    }
}
