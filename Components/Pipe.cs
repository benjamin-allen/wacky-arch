using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
	/// <summary>
	/// Pipes can store a value to be read. Pipes contain one value.
	/// </summary>
	public class Pipe
	{
		public PipeStatus Status { get; set; }
	}

	public enum PipeStatus
	{
		Idle, // Reads or writes can be requested
		AwaitingRead, // Set when a value is written. Something must read from the pipe
		AwaitingWrite // Set when a read request has been sent to an empty pipe. Something must write to the pipe
	}
}
