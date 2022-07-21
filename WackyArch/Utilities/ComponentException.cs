namespace WackyArch.Utilities
{
    public class ComponentException : Exception
    {
        public string ShortMessage { get; set; }

        public ComponentException(string message, string shortMessage) : base(message)
        {
            ShortMessage = shortMessage;
        }
    }
}
