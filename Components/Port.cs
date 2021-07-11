using System;
using System.Collections.Generic;
using System.Text;

namespace Components
{
	
	/// <summary>
	/// A port is just a pipe with a name attached, like a register.
	/// </summary>
	public class Port : ICyclable
	{
		public Port(Pipe pipe, string name)
		{
			Pipe = pipe;
			Name = name.ToUpper();
		}

		public virtual void Cycle() { }

		public virtual void Reset() { }

		public Pipe Pipe { get; set; }
		public string Name { get; set; }
	}
}
