using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Assembler
{
	public static class Tokens
	{
		static Tokens()
		{
			LongATypeMnemonicTokens = new List<Token>
			{
				Add, Subtract, Multiply, Divide, Modulus, And, Or, Xor,
				Move, Swap, Compare,
			};
			ShortATypeMnemonicTokens = new List<Token>
			{
				Negate, Not, JumpAddress
			};
			CTypeMnemonicTokens = new List<Token>
			{
				AddConstant, SubConstant, ModConstant, AndConstant, OrConstant, MoveConstant
			};
			JTypeMnemonicTokens = new List<Token>
			{
				Jump, JumpIfZero, JumpIfGreater, JumpIfLesser
			};
			MTypeMnemonicTokens = new List<Token>
			{
				ShiftLeft, ShiftRight, ShiftRightArithmetic, RotateLeft, RotateRight
			};
			PTypeMnemonicTokens = new List<Token>
			{
				Read, Write
			};

			MnemonicTokens = LongATypeMnemonicTokens.Concat(ShortATypeMnemonicTokens)
							.Concat(PTypeMnemonicTokens).Concat(MTypeMnemonicTokens)
							.Concat(CTypeMnemonicTokens).Concat(JTypeMnemonicTokens).ToList();
		}

		// All mnemonics are in uppercase. Incoming text will be converted to uppercase.
		#region Mnemonic
		public static readonly Token Add = new Token("ADD", "ADD");
		public static readonly Token Subtract = new Token("SUB", "SUB,SUBTRACT");
		public static readonly Token Multiply = new Token("MUL", "MUL,MULT,MULTIPLY");
		public static readonly Token Divide = new Token("DIV", "DIV,DIVIDE");
		public static readonly Token Modulus = new Token("MOD", "MOD,MODULUS");
		public static readonly Token Negate = new Token("NEG", "NEG,NEGATE");
		public static readonly Token And = new Token("AND", "AND");
		public static readonly Token Or = new Token("OR", "OR");
		public static readonly Token Xor = new Token("XOR", "XOR");
		public static readonly Token Not = new Token("NOT", "NOT");

		public static readonly Token ShiftLeft = new Token("SL", "SL,SHL,SHIFTLEFT");
		public static readonly Token ShiftRight = new Token("SR", "SR,SHR,SHIFTRIGHT");
		public static readonly Token ShiftRightArithmetic = new Token("SRA", "SRA,SHRA,SHIFTRIGHTA");
		public static readonly Token RotateLeft = new Token("RL", "RL,ROL,ROTL,ROTATELEFT");
		public static readonly Token RotateRight = new Token("RR", "RR,ROR,ROTR,ROTATERIGHT");

		public static readonly Token Move = new Token("MOV", "MOV,MOVE");
		public static readonly Token Swap = new Token("SWP", "SWP,SWAP");
		public static readonly Token Compare = new Token("CMP", "CMP,COMP,COMPARE");

		public static readonly Token Read = new Token("READ", "READ");
		public static readonly Token Write = new Token("WRITE", "WRITE");

		public static readonly Token Jump = new Token("JMP", "JMP,JUMP");
		public static readonly Token JumpIfZero = new Token("JEZ", "JEZ,JUMPIFZERO");
		public static readonly Token JumpIfGreater = new Token("JGZ", "JGZ,JUMPIFGREATER");
		public static readonly Token JumpIfLesser = new Token("JLZ", "JLZ,JUMPIFLESSER");
		public static readonly Token JumpAddress = new Token("JA", "JA,JUMPADDRESS");

		public static readonly Token AddConstant = new Token("ADDC", "ADDC,ADDCONST");
		public static readonly Token SubConstant = new Token("SUBC", "SUBC,SUBTRACTC,SUBCONST,SUBTRACTCONST");
		public static readonly Token ModConstant = new Token("MODC", "MODC,MODULUSC,MODCONST,MODULUSCONST");
		public static readonly Token AndConstant = new Token("ANDC", "ANDC,ANDCONST");
		public static readonly Token OrConstant = new Token("ORC", "ORC,ORCONST");
		public static readonly Token MoveConstant = new Token("MOVC", "MOVC,MOVEC,MOVCONST,MOVECONST");

		public static readonly List<Token> MnemonicTokens;
		public static readonly List<Token> LongATypeMnemonicTokens;
		public static readonly List<Token> ShortATypeMnemonicTokens;
		public static readonly List<Token> CTypeMnemonicTokens;
		public static readonly List<Token> JTypeMnemonicTokens;
		public static readonly List<Token> MTypeMnemonicTokens;
		public static readonly List<Token> PTypeMnemonicTokens;
		#endregion

		#region Registers
		public static readonly Token R0 = new Token("R0", "R0,REG0,REGISTER0");
		public static readonly Token R1 = new Token("R1", "R1,REG1,REGISTER1");
		public static readonly Token R2 = new Token("R2", "R2,REG2,REGISTER2");
		public static readonly Token Const = new Token("CONST", "CONST,R3,REG3,REGISTER3");

		public static readonly List<Token> RegisterTokens = new List<Token>
		{
			R0, R1, R2, Const
		};
		#endregion

		public static bool CheckTokenMatch(string toCheck, List<Token> tokens)
		{
			int matches = tokens.Where(t => t.Allowed.Contains(toCheck)).Count();
			if(matches == 1)
			{
				return true;
			}
			else if (matches > 1)
			{
				throw new ArgumentException($"{toCheck} matched multiple tokens: {string.Join(",", tokens.Where(t => t.Allowed.Contains(toCheck)).Select(m => m.Canonical).ToArray())}");
			}
			return false;
		}

		/// <summary>
		/// Assumes that this token will match one item in the tokens list.
		/// </summary>
		/// <param name="token"></param>
		/// <param name="tokens"></param>
		/// <returns></returns>
		public static string GetCanonicalToken(string token, List<Token> tokens)
		{
			Token match = tokens.Where(t => t.Allowed.Contains(token)).Single();
			return match.Canonical;
		}
	}

	public class Token
	{
		// Mnemonics have a canonical form (what the parser expects) and allowed forms (what can be input to be understood)
		public string Canonical { get; private set; }

		public HashSet<string> Allowed { get; private set; }

		public Token(string Canonical, string Allowed)
		{
			this.Canonical = Canonical;
			this.Allowed = Allowed.Split(",").ToHashSet<string>();
		}
	}
}
