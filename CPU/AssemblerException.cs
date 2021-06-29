using System;
using System.Collections.Generic;
using System.Text;

namespace CPU
{
	public class AssemblerException : Exception
	{
		public int LineNumber { get; set; }
		public string LineText { get; set; }

		/// <summary>
		/// Expected to fit in the small status window.
		/// </summary>
		public string ShortMessage { get; set; }

		public AssemblerException(string message, int lineNumber, string lineText, string shortMessage = null) : base(message)
		{
			LineNumber = lineNumber;
			LineText = lineText;
			if (shortMessage == null)
			{
				ShortMessage = message;
			}
			else
			{
				ShortMessage = shortMessage;
			}
		}
	}
}
