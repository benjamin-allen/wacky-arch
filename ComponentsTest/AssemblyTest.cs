﻿using static CPU.Assembler;
using Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using CPU;
using CPU.CPUs;

namespace Test
{
	[TestClass]
	public class AssemblyTest
	{
		private CPU.CPU cpu;

		[TestInitialize]
		public void TestInitialize()
		{
			cpu = new CPU.CPU();
		}


		[TestMethod]
		public void FibonacciProgram()
		{
			string program = String.Join(Environment.NewLine,
				"MOVC 1",
				"MOV R0 CONST",
				"MOV R1 CONST",
				"# SET UP THA LOOP",
				"MOVC 10",
				"",
				"# COMPUTE THE NEXT FIBONACCI NUMBER AND STORE IT INTO R0",
				"@FIB",
				"ADD R0 R1",
				"SWP R0 R1",
				"SUBC 1",
				"JGZ @FIB");

			List<Word> binary = Assemble(cpu, program, out _);

			Assert.AreEqual(0b1111_0000_0001, binary[0].Value & 0xFFF);
			Assert.AreEqual(0b0011_0011_0000, binary[1].Value & 0xFFF);
			Assert.AreEqual(0b0011_0111_0000, binary[2].Value & 0xFFF);
			Assert.AreEqual(0b1111_0000_1010, binary[3].Value & 0xFFF);
			Assert.AreEqual(0b0000_0001_0000, binary[4].Value & 0xFFF);
			Assert.AreEqual(0b0011_0001_0001, binary[5].Value & 0xFFF);
			Assert.AreEqual(0b1011_0000_0001, binary[6].Value & 0xFFF);
			Assert.AreEqual(0b0111_1111_1101, binary[7].Value & 0xFFF);
		}

		[TestMethod]
		public void ArithmeticHeavyProgram()
		{
			string program = String.Join(Environment.NewLine,
				"MOVC 10",
				"MOV R0 CONST",
				"MOVC 15",
				"MOV R1 CONST",
				"MOV R2 R1",
				"ADD R2 R0",
				"MOV R2 R1",
				"SUB R2 R0",
				"MOV R2 R0",
				"MUL R2 R1",
				"DIV R2 R1",
				"CMP R2 R0",
				"NEG R2",
				"NEG R2");

			List<Word> binary = Assemble(cpu, program, out _);

			Assert.AreEqual(0xF0A, binary[0].Value & 0xFFF);
			Assert.AreEqual(0x330, binary[1].Value & 0xFFF);
			Assert.AreEqual(0xF0F, binary[2].Value & 0xFFF);
			Assert.AreEqual(0x370, binary[3].Value & 0xFFF);
			Assert.AreEqual(0x390, binary[4].Value & 0xFFF);
			Assert.AreEqual(0x080, binary[5].Value & 0xFFF);
			Assert.AreEqual(0x390, binary[6].Value & 0xFFF);
			Assert.AreEqual(0x081, binary[7].Value & 0xFFF);
			Assert.AreEqual(0x380, binary[8].Value & 0xFFF);
			Assert.AreEqual(0x092, binary[9].Value & 0xFFF);
			Assert.AreEqual(0x093, binary[10].Value & 0xFFF);
			Assert.AreEqual(0x382, binary[11].Value & 0xFFF);
			Assert.AreEqual(0x025, binary[12].Value & 0xFFF);
			Assert.AreEqual(0x025, binary[13].Value & 0xFFF);


			int[] reg0States = new int[] { 0, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
			int[] reg1States = new int[] { 0, 0, 0, 15, 15, 15, 15, 15, 15, 15, 0, 0, 0, 0 };
			int[] reg2States = new int[] { 0, 0, 0, 0, 15, 25, 15, 5, 10, 150, 10, 10, -10, 10 };
			int[] constStates = new int[] { 10, 10, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 0 };

			for(int i = 0; i < binary.Count; i++)
			{
				Word word = binary[i];
				Instruction insn = InstructionFactory.CreateInstruction(cpu, word);
				insn.Execute();

				Assert.AreEqual(reg0States[i], cpu.Registers[0].Data.Value);
				Assert.AreEqual(reg1States[i], cpu.Registers[1].Data.Value);
				Assert.AreEqual(reg2States[i], cpu.Registers[2].Data.Value);
				Assert.AreEqual(constStates[i], cpu.Registers[3].Data.Value);
			}
		}


		[TestMethod]
		public void TestProgramOnInterpretedCPU()
		{
			Port inputA = new Port(new Pipe(), "ALPHA");
			Port inputB = new Port(new Pipe(), "BETA");
			Port inputC = new Port(new Pipe(), "GAMMA");
			Port output = new Port(new Pipe(), "OUTPUT");
			InterpreterCPU cpu2 = new InterpreterCPU(new Port[] { inputA, inputB, inputC, output });

			string program = string.Join(Environment.NewLine,
				"READ R0 ALPHA",
				"CMP R0 R2",
				"JLZ @OUTPUT",
				"READ R1 BETA",
				"CMP R1 R2",
				"JLZ @OUTPUT",
				"READ R2 GAMMA",
				"XOR CONST CONST",
				"CMP R2 CONST",
				"JLZ @OUTPUT",
				"JMP @DONE",
				"",
				"@OUTPUT", // 10
				"ADD R1 R2",
				"ADD R0 R1",
				"WRITE R0 OUTPUT",
				"",
				"@DONE"
				);

			List<Word> binary = Assembler.Assemble(cpu2, program, out _);

			Assert.AreEqual(0x400, binary[0].Value & 0xFFF);
			Assert.AreEqual(0x322, binary[1].Value & 0xFFF);
			Assert.AreEqual(0x809, binary[2].Value & 0xFFF);
			Assert.AreEqual(0x411, binary[3].Value & 0xFFF);
			Assert.AreEqual(0x362, binary[4].Value & 0xFFF);
			Assert.AreEqual(0x806, binary[5].Value & 0xFFF);
			Assert.AreEqual(0x422, binary[6].Value & 0xFFF);
			Assert.AreEqual(0x0FC, binary[7].Value & 0xFFF);
			Assert.AreEqual(0x3B2, binary[8].Value & 0xFFF);
			Assert.AreEqual(0x802, binary[9].Value & 0xFFF);
			Assert.AreEqual(0x504, binary[10].Value & 0xFFF);
			Assert.AreEqual(0x060, binary[11].Value & 0xFFF);
			Assert.AreEqual(0x010, binary[12].Value & 0xFFF);
			Assert.AreEqual(0x483, binary[13].Value & 0xFFF);


			// Now load the program in the CPU and execute it.
			cpu2.Load(program);
			inputA.Pipe.Write(55);
			inputB.Pipe.Write(45);
			inputC.Pipe.Write(-60);
			for(int i = 0; i < 100; i++) {
				cpu2.Cycle(); }

			Assert.AreEqual(40, output.Pipe.Read(out _).Value);
			Assert.AreEqual(40, cpu2.Registers[0].Data.Value);
			Assert.AreEqual(-15, cpu2.Registers[1].Data.Value);
			Assert.AreEqual(-60, cpu2.Registers[2].Data.Value);
			Assert.AreEqual(-1, cpu2.Const.Data.Value);
			Assert.AreEqual(14, cpu2.GetPCValue());


			// now load the program again in the CPU and execute it
			cpu2.Load(program);
			inputA.Pipe.Status = PipeStatus.Idle;
			inputA.Pipe.Write(-1);

			for(int i = 0; i < 6; i++) { cpu2.Cycle(); }

			Assert.AreEqual(-1, output.Pipe.Read(out _).Value);
			Assert.AreEqual(-1, cpu2.Registers[0].Data.Value);
			Assert.AreEqual(0, cpu2.Registers[1].Data.Value);
			Assert.AreEqual(0, cpu2.Registers[2].Data.Value);
			Assert.AreEqual(-1, cpu2.Const.Data.Value);
			Assert.AreEqual(14, cpu2.GetPCValue());
		}

		[TestMethod]
		public void TestAssemblyLineCounter()
		{
			var program = String.Join(Environment.NewLine,
				"MOVC 1",
				"MOV R0 CONST",
				"MOV R1 CONST",
				"# SET UP THA LOOP",
				"MOVC 10",
				"",
				"# COMPUTE THE NEXT FIBONACCI NUMBER AND STORE IT INTO R0",
				"@FIB",
				"ADD R0 R1",
				"SWP R0 R1",
				"SUBC 1",
				"JGZ @FIB");

			var cpu = new InterpreterCPU();
			var _ = Assembler.Assemble(cpu, program, out var pcTextLineMap);

			Assert.AreEqual(0, pcTextLineMap[0]);
			Assert.AreEqual(1, pcTextLineMap[1]);
			Assert.AreEqual(2, pcTextLineMap[2]);
			Assert.AreEqual(4, pcTextLineMap[3]);
			Assert.AreEqual(8, pcTextLineMap[4]);
			Assert.AreEqual(9, pcTextLineMap[5]);
			Assert.AreEqual(10, pcTextLineMap[6]);
			Assert.AreEqual(11, pcTextLineMap[7]);
		}
	}
}
