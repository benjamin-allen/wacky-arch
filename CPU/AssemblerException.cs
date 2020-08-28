using System;
using System.Collections.Generic;
using System.Text;

namespace CPU
{
	public class AssemblerException : Exception
	{
		public int LineNumber { get; set; }
		public string LineText { get; set; }

		public AssemblerException(string message, int lineNumber, string lineText) : base(message)
		{
			LineNumber = lineNumber;
			LineText = lineText;
		}
	}
}
