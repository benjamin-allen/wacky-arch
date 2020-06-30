using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
	/// <summary>
	/// Pipes act as a simple message passing implementation. Pipes contain one value.
	/// </summary>
	public class Pipe
	{
		public PipeStatus Status { get; set; } = PipeStatus.Idle;
		public Word Data { get; private set; } = new Word();

		/// <summary>
		/// Returns true if the write was completed, allowing the writer to unblock
		/// </summary>
		public bool Write(int value)
		{
			if(Status != PipeStatus.AwaitingRead)
			{
				Data.Value = value;
				Status = PipeStatus.AwaitingRead;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Returns true if the read was completed, allowing the reader to unblock
		/// </summary>
		/// <returns></returns>
		public bool Read(out int value)
		{
			if(Status == PipeStatus.AwaitingRead)
			{
				value = Data.Value;
				Status = PipeStatus.Idle;
				return true;
			}
			Status = PipeStatus.AwaitingWrite;
			value = 0;
			return false;
		}
	}

	public enum PipeStatus
	{
		Idle, // Reads or writes can be requested
		AwaitingRead, // Set when a value is written. Something must read from the pipe
		AwaitingWrite // Set when a read request has been sent to an empty pipe. Something must write to the pipe
	}
}
