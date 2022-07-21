namespace WackyArch.Components
{
    /// <summary>
    /// Registers hold a single byte of data.
    /// </summary>
    public class Register
    {
        public Register(string name)
        {
            Name = name;
            Data = new Word();
        }

        public Word Data { get; private set; }
        public string Name { get; private set; }
    }
}
