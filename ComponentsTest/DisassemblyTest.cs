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
            cpu = new CPU(new Port[] { new Port(new Pipe(), "A"), new Port(new Pipe(), "B") });
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

        [TestMethod]
        public void RegisterDisassembly()
        {
            Assert.AreEqual("MOV R0 R2", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0010_0010_0000 }, out int skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("SWP R1 CONST", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0010_0111_0001 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("CMP R0 R1", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0010_0001_0010 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);
        }

        [TestMethod]
        public void IODisassembly()
        {
            Assert.AreEqual("READ R0 A", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0100_0000_0000 }, out int skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("READ R1 B", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0100_0101_0001 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("WRITE R2 A", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0100_1010_0000 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("INT UNLOCK", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0100_1100_0000 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("INT HALT", Disassembler.DisassembleWord(cpu, new Word { Value = 0b0100_1110_1111 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);
        }

        [TestMethod]
        public void ConstDisassembly()
        {
            Assert.AreEqual("ADDC 255", Disassembler.DisassembleWord(cpu, new Word { Value = 0xAFF }, out int skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("SUBC 255", Disassembler.DisassembleWord(cpu, new Word { Value = 0xBFF }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("MODC 255", Disassembler.DisassembleWord(cpu, new Word { Value = 0xCFF }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("ANDC -1", Disassembler.DisassembleWord(cpu, new Word { Value = 0xDFF }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("ORC 127", Disassembler.DisassembleWord(cpu, new Word { Value = 0xE7F }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("MOVC 127", Disassembler.DisassembleWord(cpu, new Word { Value = 0xF7F }, out skipNWords));
            Assert.AreEqual(0, skipNWords);
        }

        [TestMethod]
        public void FunctionWordDisassembly()
        {
            Assert.AreEqual("DEFFUNC", Disassembler.DisassembleWord(cpu, new Word { Value = 0x30F }, out int skipNWords));
            Assert.AreEqual(15, skipNWords);

            Assert.AreEqual("ENDFUNC", Disassembler.DisassembleWord(cpu, new Word { Value = 0x300 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("CALL 12", Disassembler.DisassembleWord(cpu, new Word { Value = 0x3CC }, out skipNWords));
            Assert.AreEqual(0, skipNWords);

            Assert.AreEqual("RETURN", Disassembler.DisassembleWord(cpu, new Word { Value = 0x3F0 }, out skipNWords));
            Assert.AreEqual(0, skipNWords);
        }

        [TestMethod]
        public void FunctionDisassembly()
        {
            var programBinary = new List<int> { 0x302, 'F', '0', 0x00F, 0x00F, 0x300 };
            var expectedDisassembly = string.Join(Environment.NewLine, "DEFFUNC F0", "NOOP", "NOOP", "ENDFUNC");

            Assert.AreEqual(expectedDisassembly, Disassembler.Disassemble(cpu, programBinary.Select(x => new Word { Value = x }).ToList(), out var pcLineMap));
            Assert.AreEqual(0, pcLineMap[0]);
            Assert.AreEqual(0, pcLineMap[1]);
            Assert.AreEqual(0, pcLineMap[2]);
            Assert.AreEqual(1, pcLineMap[3]);
            Assert.AreEqual(2, pcLineMap[4]);
            Assert.AreEqual(3, pcLineMap[5]);


            programBinary = new List<int> { 0x3C0, 0x302, 'F', '0', 0x00F, 0x00F, 0x3F0, 0x300 };
            expectedDisassembly = string.Join(Environment.NewLine, "CALL F0", "DEFFUNC F0", "NOOP", "NOOP", "RETURN", "ENDFUNC");

            Assert.AreEqual(expectedDisassembly, Disassembler.Disassemble(cpu, programBinary.Select(x => new Word { Value = x }).ToList(), out pcLineMap));
            Assert.AreEqual(0, pcLineMap[0]);
            Assert.AreEqual(1, pcLineMap[1]);
            Assert.AreEqual(1, pcLineMap[2]);
            Assert.AreEqual(1, pcLineMap[3]);
            Assert.AreEqual(2, pcLineMap[4]);
            Assert.AreEqual(3, pcLineMap[5]);
            Assert.AreEqual(4, pcLineMap[6]);
            Assert.AreEqual(5, pcLineMap[7]);
        }
    }
}
