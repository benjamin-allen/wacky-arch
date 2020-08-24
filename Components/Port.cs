using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
	
	/// <summary>
	/// A port is just a pipe with a name attached, like a register.
	/// </summary>
	public class Port
	{
		public Port(Pipe pipe, string name)
		{
			Pipe = pipe;
			Name = name.ToUpper();
		}

		public Pipe Pipe { get; set; }
		public string Name { get; set; }
	}
}
