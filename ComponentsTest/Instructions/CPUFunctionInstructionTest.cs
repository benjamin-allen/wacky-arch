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
        public StackCPU cpu;

        [TestInitialize]
        public void TestInitialize()
        {
            cpu = new StackCPU();
        }

        [TestMethod]
        public void TestDefFuncEndFuncInstruction()
        {
            var programBinary = new List<int> { 0x302, 'F', '0', 0x00F, 0x00F, 0x300 };
            cpu.Load(programBinary.Select(x => new Word { Value = x}).ToList());

            Assert.AreEqual(0, cpu.GetPCValue());
            Assert.AreEqual(0, cpu.GetPCOfNthFunction(0));
            cpu.Cycle();
            Assert.AreEqual(3, cpu.GetPCValue());
            Assert.AreEqual(0, cpu.GetPCOfNthFunction(0));

            cpu.Cycle();
            Assert.AreEqual(4, cpu.GetPCValue());
            cpu.Cycle();
            Assert.AreEqual(5, cpu.GetPCValue());
            cpu.Cycle();
            Assert.AreEqual(6, cpu.GetPCValue());

            cpu.Cycle();
            Assert.AreEqual(true, cpu.IsHalted);
            cpu.Cycle();
            cpu.Cycle();
            Assert.AreEqual(true, cpu.IsHalted);
        }

        [TestMethod]
        public void TestCallAndReturn()
        {
            var programBinary = new List<int> { 0x3C1, 0x00F, 0x302, 'F', '0', 0xA01, 0x300, 0x302, 'F', '1', 0xA01, 0x3F0 };
            cpu.Load(programBinary.Select(x => new Word { Value = x }).ToList());

            Assert.AreEqual(2, cpu.GetPCOfNthFunction(0));
            Assert.AreEqual(7, cpu.GetPCOfNthFunction(1));

            cpu.Cycle();
            Assert.AreEqual(7, cpu.GetPCValue());
            Assert.IsNotNull(cpu.Stack.Words[0]);
            Assert.AreEqual(0, cpu.Stack.Words[0].Value);
            Assert.AreEqual(1, cpu.Stack.SP);

            cpu.Cycle();
            Assert.AreEqual(10, cpu.GetPCValue());
            cpu.Cycle();
            Assert.AreEqual(11, cpu.GetPCValue());
            Assert.AreEqual(1, cpu.Registers[3].Data.Value);

            cpu.Cycle();
            Assert.AreEqual(1, cpu.GetPCValue());
            Assert.AreEqual(0, cpu.Stack.SP);
            Assert.AreEqual(0, cpu.Stack.Words[0].Value);
        }
    }
}
