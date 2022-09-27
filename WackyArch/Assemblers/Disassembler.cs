using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;

namespace WackyArch.Assemblers
{
    public static class Disassembler
    {
        public static string Disassemble(CPU cpu, List<Word> programBinary, out Dictionary<int, int> pcLineMap)
        {
            pcLineMap = new Dictionary<int, int>();
            var text = new List<string>();
            for(int i = 0; i < programBinary.Count; i++)
            {
                var word = programBinary[i];
                var line = DisassembleWord(cpu, word, out int skipNextNWords);
                text.Add(line);
            }
            return string.Join(Environment.NewLine, text);
        }

        public static string DisassembleWord(CPU cpu, Word word, out int skipNextNWords)
        {
            skipNextNWords = 0;
            var insn = InstructionFactory.CreateInstruction(cpu, word);
            var line = insn.Disassemble();
            return line;
        }
    }
}
