using WackyArch.Components;
using WackyArch.CPUs;
using WackyArch.Utilities;
using System.Text.RegularExpressions;

namespace WackyArch.Assemblers
{
	public static class Assembler
	{
		/// <summary>
		/// Takes in a textual assembly program and returns an ordered list of instructions.
		/// </summary>
		/// <param name="assemblyText"></param>
		/// <returns></returns>
		public static List<Word> Assemble(CPU cpu, string assemblyText, out Dictionary<int, int> pcTextLineMap)
		{
			var lines = CleanText(assemblyText);

			return AssembleText(cpu, lines, out pcTextLineMap);
		}

		private static List<string> CleanText(string assembly)
		{
			// Normalize newlines and convert to uppercase.
			string normalized = Regex.Replace(assembly, @"\r\n|\n\r|\n|\r", Environment.NewLine);
			normalized = normalized.ToUpper();
			return normalized.Split(Environment.NewLine).Select(s => s.Trim()).ToList();
		}

		private static List<Word> AssembleText(CPU cpu, List<string> assemblyLines, out Dictionary<int, int> pcTextLineMap)
		{
			var words = new List<Word>();
			var labelAddresses = new Dictionary<string, int>();
			var deferredInstructions = new List<Tuple<string, int, int>>();
			pcTextLineMap = new Dictionary<int, int>();
			int currentAddress = 0;

			for(int i = 0; i < assemblyLines.Count; i++)
			{
				var line = assemblyLines[i];

				if(new Regex("[^A-Z0-9 @\\-#]").IsMatch(line)) { throw new AssemblerException("Invalid characters", i, line); }

				if(line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) { continue; /* Ignore comments */ }

				List<string> tokens = line.Split(" ").ToList();

				if(line.StartsWith("@")) 
				{
					labelAddresses.Add(tokens[0], currentAddress);
					continue;
				}


				// first item should always be a mnemonic
				if (!Tokens.CheckTokenMatch(tokens[0], Tokens.MnemonicTokens))
				{
					throw new AssemblerException($"Invalid Mnemonic {tokens[0]}", i, line);
				}

				Word result;
				if(Tokens.CheckTokenMatch(tokens[0], Tokens.LongATypeMnemonicTokens))
				{
					// Long A type
					result = AssembleLongAType(tokens, i);
				}
				else if(Tokens.CheckTokenMatch(tokens[0], Tokens.ShortATypeMnemonicTokens))
				{
					// Short A Type
					result = AssembleShortAType(tokens, i);
				}
				else if (Tokens.CheckTokenMatch(tokens[0], Tokens.CTypeMnemonicTokens))
				{
					// C type
					result = AssembleCType(tokens, i);
				}
				else if (Tokens.CheckTokenMatch(tokens[0], Tokens.JTypeMnemonicTokens))
				{
					// J type
					deferredInstructions.Add(new Tuple<string, int, int>(line, currentAddress, i));
					result = new Word { Value = 0x00F }; // insert a dummy no-op.
				}
				else if (Tokens.CheckTokenMatch(tokens[0], Tokens.MTypeMnemonicTokens))
				{
					// M type
					result = AssembleMType(tokens, i);
				}
				else if (Tokens.CheckTokenMatch(tokens[0], Tokens.PTypeMnemonicTokens))
				{
					// P type
					result = AssemblePType(tokens, i, cpu);
				}
				else if (Tokens.CheckTokenMatch(tokens[0], Tokens.InterruptTokens))
                {
					result = AssembleInterrupt(tokens, i);
                }
				else if (Tokens.CheckTokenMatch(tokens[0], Tokens.FunctionTokens))
				{
					// functions are weird and hard.
					var resultWords = AssembleFunction(tokens, i);
					int? nextDefFunc = assemblyLines.Where((value, index) => index > i && value.Contains("DEF")).Select((value, index) => index).FirstOrDefault();
					int? nextEndFunc = assemblyLines.Where((value, index) => index > i && value.Contains("END")).Select((value, index) => index).FirstOrDefault();
					if (nextEndFunc == null || (nextDefFunc.HasValue && nextDefFunc < nextEndFunc))
					{
						throw new AssemblerException("No matching ENDFUNC for this DEFFUNC", i, string.Join(" ", tokens), "No ENDFUNC found.");
					}
					pcTextLineMap.Add(currentAddress, i);
					currentAddress += resultWords.Count;
					words.AddRange(resultWords);
					continue;
				}
				else
				{
					throw new NotImplementedException();
				}
				pcTextLineMap.Add(currentAddress, i);
				currentAddress += 1;
				words.Add(result);
			}

			// Go back and inject jump instructions
			foreach(Tuple<string, int, int> deferredInstruction in deferredInstructions)
			{
				string line = deferredInstruction.Item1;
				int address = deferredInstruction.Item2;
				int i = deferredInstruction.Item3;

				Word result = AssembleJType(line.Split(" ").ToList(), i, address, labelAddresses);
				Word oldWord = new Word { Value = words[address].Value };
				words[address] = result; // Overwrite the dummy word
				if(oldWord.Value != 0x00F)
				{
					throw new Exception("Tried to overwrite a non-no-op instruction.");
				}
			}

			return words;
		}

		private static Word AssembleLongAType(List<string> tokens, int i)
		{
			ValidateLongAType(tokens, i);

			int RX = GetRegisterNumber(tokens[1]) << 6;
			int RY = GetRegisterNumber(tokens[2]) << 4;

			int wordValue = RX | RY;

			switch (Tokens.GetCanonicalToken(tokens[0], Tokens.LongATypeMnemonicTokens))
			{
				case "ADD":
					wordValue |= 0x000; break;
				case "SUB":
					wordValue |= 0x001; break;
				case "MUL":
					wordValue |= 0x002; break;
				case "DIV":
					wordValue |= 0x003; break;
				case "MOD":
					wordValue |= 0x004; break;
				case "AND":
					wordValue |= 0x00A; break;
				case "OR":
					wordValue |= 0x00B; break;
				case "XOR":
					wordValue |= 0x00C; break;
				case "MOV":
					wordValue |= 0x300; break;
				case "SWP":
					wordValue |= 0x301; break;
				case "CMP":
					wordValue |= 0x302; break;
				default:
					throw new NotImplementedException();
			}

			return new Word { Value = wordValue };
		}

		private static Word AssembleShortAType(List<string> tokens, int i)
		{
			ValidateShortAType(tokens, i);

			int RX = GetRegisterNumber(tokens[1]);
			int wordValue = 0;

			switch(Tokens.GetCanonicalToken(tokens[0], Tokens.ShortATypeMnemonicTokens))
			{
				case "NEG":
					wordValue = 0x005 | (RX << 4); break;
				case "NOT":
					wordValue = 0x00D | (RX << 4); break;
				case "JA":
					wordValue = 0x900 | RX; break;
			}

			return new Word { Value = wordValue };
		}

		private static Word AssembleCType(List<string> tokens, int i)
		{
			ValidateCType(tokens, i);

			int valueUnsigned = Math.Max(0, Math.Min(255, Int32.Parse(tokens[1])));
			int valueExtended = Utilities.Utilities.SignExtend(Int32.Parse(tokens[1]), 7);
			int wordValue = 0;

			switch(Tokens.GetCanonicalToken(tokens[0], Tokens.CTypeMnemonicTokens))
			{
				case "ADDC":
					wordValue = 0b1010_0000_0000 | valueUnsigned; break;
				case "SUBC":
					wordValue = 0b1011_0000_0000 | valueUnsigned; break;
				case "MODC":
					wordValue = 0b1100_0000_0000 | valueUnsigned; break;
				case "ANDC":
					wordValue = 0b1101_0000_0000 | valueExtended; break;
				case "ORC":
					wordValue = 0b1110_0000_0000 | valueExtended; break;
				case "MOVC":
					wordValue = 0b1111_0000_0000 | valueExtended; break;
				default:
					throw new NotImplementedException();
			}

			return new Word { Value = wordValue };
		}

		private static Word AssembleJType(List<string> tokens, int i, int currentAddress, Dictionary<string, int> labelAddresses)
		{
			ValidateJType(tokens, i, labelAddresses);

			int targetAddress = labelAddresses[tokens[1]];
			int wordValue = 0;
			int diff = targetAddress - currentAddress;
			if(diff > 127 || diff < -128)
			{
				throw new AssemblerException("Jump out of range. Label should be within 127 instructions of this jump.", i, string.Join(" ", tokens), "Jump out of range");
			}
			wordValue = 0x0FF & Utilities.Utilities.SignExtend(diff, 7);

			switch(Tokens.GetCanonicalToken(tokens[0], Tokens.JTypeMnemonicTokens))
			{
				case "JMP":
					wordValue |= 0x500; break;
				case "JEZ":
					wordValue |= 0x600; break;
				case "JGZ":
					wordValue |= 0x700; break;
				case "JLZ":
					wordValue |= 0x800; break;
				default:
					throw new NotImplementedException();
			}

			return new Word { Value = wordValue };
		}

		private static Word AssembleMType(List<string> tokens, int i)
		{
			ValidateMType(tokens, i);

			int RX = GetRegisterNumber(tokens[1]) << 4;
			int wordValue = RX;
			int value = Math.Max(0, Math.Min(15, Int32.Parse(tokens[2])));
			wordValue |= value;

			switch(Tokens.GetCanonicalToken(tokens[0], Tokens.MTypeMnemonicTokens))
			{
				case "SL":
					wordValue |= 0b0001_0000_0000; break;
				case "SR":
					wordValue |= 0b0001_1000_0000; break;
				case "SRA":
					wordValue |= 0b0001_1100_0000; break;
				case "RL":
					wordValue |= 0b0010_0000_0000; break;
				case "RR":
					wordValue |= 0b0010_1000_0000; break;
				default:
					throw new NotImplementedException();
			}

			return new Word { Value = wordValue };
		}

		private static Word AssemblePType(List<string> tokens, int i, CPU cpu)
		{
			ValidatePType(tokens, i, cpu);

			int RX = GetRegisterNumber(tokens[1]) << 4;
			int portNum = cpu.Ports.ToList().FindIndex(p => p.Name == tokens[2].ToUpper()); // already validated.
			int wordValue = RX | portNum;

			switch(Tokens.GetCanonicalToken(tokens[0], Tokens.PTypeMnemonicTokens))
			{
				case "READ":
					wordValue |= 0b0100_0000_0000; break;
				case "WRITE":
					wordValue |= 0b0100_1000_0000; break;
				default:
					throw new NotImplementedException();
			}

			return new Word { Value = wordValue };
		}

		private static Word AssembleInterrupt(List<string> tokens, int i)
        {
			ValidateInterrupt(tokens, i);
			int wordValue;

			switch (Tokens.GetCanonicalToken(tokens[1], Tokens.InterruptTokens))
            {
				case "UNLOCK":
					wordValue = 0b0100_1111_0000; break;
				case "HALT":
					wordValue = 0b0100_1111_1111; break;
				default:
					throw new NotImplementedException();
            }

			return new Word { Value = wordValue };
        }

		private static List<Word> AssembleFunction(List<string> tokens, int i)
		{
            ValidateFunction(tokens, i);

			switch (Tokens.GetCanonicalToken(tokens[0], Tokens.FunctionTokens))
			{
				case "DEFFUNC":
					var wordList = new List<Word> { new Word { Value = 0b0011_0000_0000 + tokens[1].Length } };
					foreach (char x in tokens[1])
					{
						wordList.Add(new Word { Value = (int)x });
					}
					return wordList;
				case "ENDFUNC":
					return new List<Word> { new Word { Value = 0b0011_0000_0000 } };
				default:
					throw new NotImplementedException();
			}
		}

		private static void ValidateLongAType(List<string> tokens, int i)
		{
			string line = string.Join(" ", tokens);
			tokens.ValidateTokenArraySize(i, 3, "MNEMONIC RX RY");
			if (!Tokens.CheckTokenMatch(tokens[1], Tokens.RegisterTokens))
			{
				throw new AssemblerException($"{tokens[1]} is not a register name.", i, line, $"Not a register: {tokens[1]}");
			}
			if (!Tokens.CheckTokenMatch(tokens[2], Tokens.RegisterTokens))
			{
				throw new AssemblerException($"{tokens[2]} is not a register name.", i, line, $"Not a register: {tokens[2]}");
			}
		}

		private static void ValidateShortAType(List<string> tokens, int i)
		{
			string line = string.Join(" ", tokens);
			tokens.ValidateTokenArraySize(i, 2, "MNEMONIC RX");

			if(!Tokens.CheckTokenMatch(tokens[1], Tokens.RegisterTokens))
			{
				throw new AssemblerException($"{tokens[1]} is not a register name.", i, line, $"Not a register: {tokens[1]}");
			}
		}

		private static void ValidateCType(List<string> tokens, int i)
		{
			string line = string.Join(" ", tokens);
			tokens.ValidateTokenArraySize(i, 2, "MNEMONIC VALUE");
			if (Int32.TryParse(tokens[1], out _) == false)
			{
				throw new AssemblerException($"{tokens[1]} is not a numeric literal.", i, line, $"Not a number: {tokens[1]}");
			}
		}

		private static void ValidateJType(List<string> tokens, int i, Dictionary<string, int> labelAddresses)
		{
			string line = string.Join(" ", tokens);
			tokens.ValidateTokenArraySize(i, 2, "MNEMONIC LABEL");

			try
			{
				var _ = labelAddresses[tokens[1]];
			}
			catch (KeyNotFoundException)
			{
				throw new AssemblerException($"Label \"{tokens[1]}\" not found.", i, line, $"{tokens[1]} not found");
			}
		}

		private static void ValidateMType(List<string> tokens, int i)
		{
			string line = string.Join(" ", tokens);
			tokens.ValidateTokenArraySize(i, 3, "MNEMONIC RX VALUE");
			if(!Tokens.CheckTokenMatch(tokens[1], Tokens.RegisterTokens))
			{
				throw new AssemblerException($"{tokens[1]} is not a register name.", i, line, $"Not a register: {tokens[1]}");
			}
			if(Int32.TryParse(tokens[2], out _) == false)
			{
				throw new AssemblerException($"{tokens[2]} is not a numeric literal.", i, line, $"Not a number: {tokens[2]}");
			}
		}

		private static void ValidatePType(List<string> tokens, int i, CPU cpu)
		{
			string line = string.Join(" ", tokens);
			tokens.ValidateTokenArraySize(i, 3, "MNEMONIC RX PORT");
			if(!Tokens.CheckTokenMatch(tokens[1], Tokens.RegisterTokens))
			{
				throw new AssemblerException($"{tokens[1]} is not a register name.", i, line, $"Not a register: {tokens[1]}");
			}

			string portName = tokens[2].ToUpper();
			int portIndex = cpu.Ports.ToList().FindIndex(p => p.Name == portName);
			if(portIndex < 0 || portIndex > 15) 
			{
				throw new AssemblerException($"{portName} is not a port name.", i, line, $"Not a port: {portName}");
			}
		}

		private static void ValidateInterrupt(List<string> tokens, int i)
        {
			string line = string.Join(" ", tokens);
			tokens.ValidateTokenArraySize(i, 2, "INT INTERRUPT");
            if (!Tokens.CheckTokenMatch(tokens[1], Tokens.InterruptTokens))
            {
				throw new AssemblerException($"{tokens[1]} is not a valid interrupt.", i, line, $"Not an interrupt: {tokens[1]}");
            }

			if (Tokens.GetCanonicalToken(tokens[1], Tokens.InterruptTokens) == Tokens.Interrupt.Canonical)
            {
				throw new AssemblerException($"{tokens[1]} is not a valid interrupt.", i, line, $"Not an interrupt: {tokens[1]}");
            }
        }

		private static void ValidateFunction(List<string> tokens, int i)
		{
			string line = string.Join(" ", tokens);
			if (Tokens.GetCanonicalToken(tokens[0], Tokens.FunctionTokens) == "DEFFUNC")
			{
				tokens.ValidateTokenArraySize(i, 2, "DEFFUNC NAME");
				if (tokens[1].Length > 255)
				{
					throw new AssemblerException($"{tokens[1]} is too long of a name.", i, line, $"Name too long: {tokens[1]}");
				}
			}
			else
			{
				tokens.ValidateTokenArraySize(i, 1, "ENDFUNC");
			}
		}

		private static int GetRegisterNumber(string token)
		{
			switch (Tokens.GetCanonicalToken(token, Tokens.RegisterTokens))
			{
				case "R0":
					return 0;
				case "R1":
					return 1;
				case "R2":
					return 2;
				case "CONST":
					return 3;
				default:
					throw new NotImplementedException();
			}
		}

		private static void ValidateTokenArraySize(this List<string> tokens, int i, int size, string suggestedFormat)
		{
			string line = string.Join(" ", tokens);
			if(tokens.Count > size)
			{
				throw new AssemblerException($"Malformed instruction (Too many tokens). Did you mean {string.Join(" ", tokens.Take(size))}?", i, line, $"Bad instruction. Did you mean {string.Join(" ", tokens.Take(size))}?");
			}
			if(tokens.Count < size)
			{
				throw new AssemblerException($"Malformed instruction (Too few tokens). Format should be {suggestedFormat}.", i, line, $"Bad instruction. Format is {suggestedFormat}");
			}
		}
	}
}
