using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyArch.Components
{
	public interface ISupportsFunctionCall
	{
		public int GetPCOfNthFunction(int n);
		public int GetReturnAddress();
		public void AddToCallStack(int currentPC);
	}
}
