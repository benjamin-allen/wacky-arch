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
        public static List<Word> ShrinkBinary(List<Word> binary)
        {
            var intEndLoc = binary.FindLastIndex(x => x.Value == 0x4FF);
            if (intEndLoc != -1)
            {
                return binary.Take(intEndLoc+1).ToList();
            }
            return binary;
        }

        public static string Disassemble(CPU cpu, List<Word> programBinary, out Dictionary<int, int> pcLineMap)
        {
            programBinary = ShrinkBinary(programBinary);

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
                    var pc = pcLineMap.First(x => x.Value == i).Key;
                    var nameLength = programBinary[pc].Value & 0x07F;
                    for (int j = pc + 1; j <= pc + nameLength; j++)
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
            var pass4 = new List<string>(pass3);
            Dictionary<int, string> linesToInsertLabelsAbove = new();
            for (int i = 0; i < pass4.Count; i++)
            {
                var line = pass4[i];
                if (line.StartsWith(Tokens.Jump.Canonical) || line.StartsWith(Tokens.JumpIfZero.Canonical) || line.StartsWith(Tokens.JumpIfLesser.Canonical) || line.StartsWith(Tokens.JumpIfGreater.Canonical))
                {
                    // This is a jump instruction. Read the offset of the jump
                    var offset = Convert.ToInt32(line.Split(" ")[1]);
                    var pc = pcLineMap.First(x => x.Value == i).Key; // what is the PC of this line?
                    var newPc = pc + offset;
                    var didFind = pcLineMap.TryGetValue(newPc, out int newPcLine);

                    var name = "@L" + (linesToInsertLabelsAbove.Count + 1);
                    try
                    {
                        linesToInsertLabelsAbove.Add(didFind ? newPcLine : -1, name); // If we didn't find the line the program is jumping past its end. -1 marks that
                    }
                    catch (ArgumentException)
                    {
                        // There was already a label for this line. Reuse that
                        name = linesToInsertLabelsAbove[newPcLine];
                    }
                    pass4[i] = line.Split(" ")[0] + " " + name;
                }
            }
            foreach(var (lineNumber, label) in linesToInsertLabelsAbove.OrderByDescending(x => x.Key)) // Do this in reverse order so we're always inserting a valid label.
            {
                if (lineNumber == -1 )
                {
                    pass4.Add(label);
                    continue;
                }
                pass4.Insert(lineNumber, label);
                foreach (var (k,v) in pcLineMap) // Update PCLineMap.
                {
                    pcLineMap[k] = v >= lineNumber ? v + 1 : v;
                }
                
            }

            return String.Join(Environment.NewLine, pass4);

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
