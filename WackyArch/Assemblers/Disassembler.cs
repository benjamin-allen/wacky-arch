using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Instructions;
using WackyArch.Utilities;

namespace WackyArch.Assemblers
{
    public static class Disassembler
    {
        public static string Disassemble(CPU cpu, List<Word> programBinary, out Dictionary<int, int> pcLineMap)
        {
            // Pass 1. Get opcodes in place, but don't resolve call names, function names, or jump labels yet
            pcLineMap = new Dictionary<int, int>();
            var pass1 = new List<string>();
            for(int i = 0; i < programBinary.Count; i++)
            {
                var word = programBinary[i];
                var line = DisassembleWord(cpu, word, out int skipNextNWords);
                pcLineMap.Add(i, pass1.Count);
                pass1.Add(line);
                i += skipNextNWords;
            }

            // In pass 2, we name all of the deffuncs
            var pass2 = new List<string>(pass1);
            Dictionary<int, string> functionNumsToNames = new();
            int fnum = 0;
            for(int i = 0; i < pass2.Count; i++)
            {
                var line = pass2[i];
                if (line == Tokens.DefineFunction.Canonical)
                {
                    var name = "";
                    var nameLength = programBinary[pcLineMap.First(x => x.Value == i).Key].Value & 0x07F;
                    for (int j = i + 1; j <= i + nameLength; j++)
                    {
                        name = name + (char)(programBinary[j].Value & 0x7F);
                        pcLineMap[j] = i;
                    }
                    functionNumsToNames.Add(fnum, name);
                    fnum++;
                    pass2[i] = line + " " + name;
                }
            }

            // In pass 3, rename the CALL commands
            var pass3 = new List<string>(pass2);
            for (int i = 0; i < pass3.Count; i++)
            {
                var line = pass3[i];
                if (line.StartsWith(Tokens.Call.Canonical))
                {
                    var num = Convert.ToInt32(line.Split(" ")[1]);
                    var name = functionNumsToNames[num];
                    pass3[i] = Tokens.Call.Canonical + " " + name;
                }
            }

            // In pass 4, inject the JUMP labels. Ugh.
            // Update the PCLinemap by pushing each line past the new jump instruction one down.
            return String.Join(Environment.NewLine, pass3);

        }

        public static string DisassembleWord(CPU cpu, Word word, out int skipNextNWords)
        {
            skipNextNWords = 0;
            var insn = InstructionFactory.CreateInstruction(cpu, word);
            var line = insn.Disassemble();

            if (line == Tokens.DefineFunction.Canonical)
            {
                skipNextNWords = (insn as FunctionInstruction).FunctionNumber;
            }

            return line;
        }
    }
}
