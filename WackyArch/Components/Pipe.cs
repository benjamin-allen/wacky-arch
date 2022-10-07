namespace WackyArch.Components
{
    /// <summary>
    /// Pipes act as a simple message passing implementation. Pipes contain one value.
    /// </summary>
    /// <remarks>
    /// It's expected that readers and writers will wrap calls to pipe methods in appropriate blocker code.
    /// </remarks>
    public class Pipe
    {
        public string Name { get; set; }
        public PipeStatus Status { get; set; } = PipeStatus.Idle;
        public Word Data { get; set; } = new Word();

        /// <summary>
        /// Returns true if the write was completed, allowing the writer to unblock
        /// </summary>
        public virtual bool Write(int value)
        {
            if (Status != PipeStatus.AwaitingRead)
            {
                Data = new Word { Value = value };
                Status = PipeStatus.AwaitingRead;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the read was completed, allowing the reader to unblock
        /// </summary>
        /// <returns></returns>
        public virtual Word Read(out bool didRead)
        {
            if (Status == PipeStatus.AwaitingRead || Status == PipeStatus.ForceRead)
            {
                Word word = new Word { Value = Data.Value };
                didRead = true;
                Status = PipeStatus.Idle;
                return word;
            }
            Status = PipeStatus.AwaitingWrite;
            didRead = false;
            return null;
        }
    }

    public class AlwaysWriteablePipe : Pipe
    {
        ///<summary>
        ///Always allows a write
        ///</summary>
        public override bool Write(int value)
        {
            Status = PipeStatus.Idle;
            return base.Write(value);
        }
    }

    public class StackPipe : AlwaysWriteablePipe
    {
        ///<summary>
        ///Always allows a read. Returns the last thing put into the pipe
        ///</summary>
        public override Word Read(out bool didRead)
        {
            Status = PipeStatus.ForceRead;
            var ret = base.Read(out didRead);
            Status = PipeStatus.ForceRead;
            return ret;
        }
    }

    public enum PipeStatus
    {
        Idle, // Reads or writes can be requested
        AwaitingRead, // Set when a value is written. Something must read from the pipe
        AwaitingWrite, // Set when a read request has been sent to an empty pipe. Something must write to the pipe
        ForceRead
    }
}
