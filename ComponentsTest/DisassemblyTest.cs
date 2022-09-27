using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Assemblers;
using WackyArch.Components;
using WackyArch.CPUs;

namespace Test
{
    [TestClass]
    public class DisassemblyTest
    {
        private CPU cpu;

        [TestInitialize]
        public void TestInitialize()
        {
            cpu = new CPU();
        }

        [TestMethod]
        public void ArithmeticDisassembly()
        {
            Assert.AreEqual("ADD R0 R1", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_0001_0000 }, out int skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("SUB R1 R0", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_0100_0001 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("MUL R1 R1", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_0101_0010 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("DIV R2 R0", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_1000_0011 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("MOD R2 R1", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_1001_0100 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("NEG CONST", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_0011_0101 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("AND R0 R0", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_0000_1010 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("OR CONST R2", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_1110_1011 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("XOR R2 CONST", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_1011_1100 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("NOT R2", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_0010_1101 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("NOOP", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0000_0000_1111 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);
        }

        [TestMethod]
        public void ShiftDisassembly()
        {
            Assert.AreEqual("SL R2 7", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0001_0010_0111 }, out int skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("SL R1 7", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0001_0101_0111 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("SR R0 15", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0001_1000_1111 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("SRA CONST 5", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0001_1111_0101 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);
        }
    }
}
