using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Instructions;

namespace Test.Instructions
{
    [TestClass]
    public class CPUFunctionInstructionTest
    {
        public CPU cpu;

        [TestInitialize]
        public void TestInitialize()
        {
            cpu = new CPU(new WackyArch.Components.Port[] { });
        }

        [TestMethod]
        public void TestDefFuncInstruction()
        {
            // THIS TEST FAILS RIGHT NOW B/C A NORMAL CPU DOESN'T DO ANYTHING WITH FUNCTION INSNs


            var noop = new ArithmeticInstruction(cpu, new Word { Value = 0x00F });
            noop.Execute();
            cpu.Cycle();
            Assert.AreEqual(1, cpu.GetPCValue());

            var defnInsn = new FunctionInstruction(cpu, new Word { Value = 0b0011_0000_0010 });
            defnInsn.Execute();
			cpu.Cycle();
			Assert.AreEqual(4, cpu.GetPCValue());
        }

        [TestMethod]
        public void TestEndFuncInstruction()
        {
			var noop = new ArithmeticInstruction(cpu, new Word { Value = 0x00F });
			noop.Execute();
			cpu.Cycle();
			Assert.AreEqual(1, cpu.GetPCValue());

			var endInsn = new FunctionInstruction(cpu, new Word { Value = 0b0011_0000_0000 });
            endInsn.Execute();
			cpu.Cycle();
			Assert.AreEqual(2, cpu.GetPCValue());
        }
    }
}
