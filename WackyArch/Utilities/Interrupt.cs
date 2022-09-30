using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WackyArch.Utilities
{
    public enum InterruptType
    {
        UNLOCK,
        HALT,
        END = 15
    }

    public class Interrupt : Exception
    {
        public InterruptType InterruptType { get; set; }
        public Interrupt(InterruptType interruptType)
        {
            InterruptType = interruptType;
        }
    }
}
