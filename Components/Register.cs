using System;

namespace Components
{
	/// <summary>
	/// Registers hold a single byte of data.
	/// </summary>
	public class Register
	{
		public Register(string name)
		{
			Name = name;
			Data = 0;
		}

		public byte Data { get; set; }

		private string _Name;
		public string Name 
		{
			get { return _Name; }
			set { _Name = value.Substring(0, 3); }
		}
	}
}
