using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Components;
using Emulator.Architectures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgRunner.Model;
using Utilities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProgRunner.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EmulatorController : ControllerBase
	{
		public int allowedCycles = 100_000;
		private PRContext _context;

		public EmulatorController(PRContext context)
		{
			_context = context;
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Ok("You must be Igor! ZmxhZz17bjAgMXRzIHByMG4wdW5jM2QgM3kzLWcwcn0=");
		}


		// Am I proud of this controller? Not at all.
		[HttpPost("alpha")]
		public IActionResult Post([FromBody] ProgramRunDTO dto)
		{
			var runlog = new RunLog
			{
				ChallengeId = dto.ChallengeId,
				Code = dto.Code,
				SubmittedTime = DateTime.Now
			};

			// Find the challenge
			var challenge = _context.AlphaChallenges
				.Where(c => c.Id == dto.ChallengeId)
				.Include(c => c.AlphaChallengeTests).SingleOrDefault();
			if (challenge == null)
			{
				runlog.CompletedTime = DateTime.Now;
				runlog.Result = "Challenge not found";
				_context.RunLogs.Add(runlog);
				_context.SaveChanges();
				return NotFound("Challenge not found");
			}

			// For each challenge test: Load up an alpha architecture with that challenge's test loaded
			foreach(var challengeTest in challenge.AlphaChallengeTests) {
				var c = new Emulator.Challenges.AlphaChallenge
				{
					TopInputData = buildList(challengeTest.TopInput),
					BottomInputData = buildList(challengeTest.BottomInput),
					OutputData = buildList(challengeTest.ExpectedOutput)
				};
				var ac = new AlphaComponents(c);
				ac.Cpu.Load(dto.Code);
				for(int i = 0; i < allowedCycles; i++)
				{
					try
					{
						ac.cyclables.ForEach(c => c.Cycle());
					}
					catch (ComponentException cex)
					{
						runlog.Result = $"Test failure: {cex.Message}. Test ID {challengeTest.Id}";
						runlog.CompletedTime = DateTime.Now;
						_context.RunLogs.Add(runlog);
						_context.SaveChanges();
						return Ok($"A test failed. {cex.Message}");
					}
					if (ac.Cpu.IsErrored || ac.Cpu.IsHalted)
					{
						break;
					}
				}

				if (ac.Output.ExpectedData.Count != 0)
				{
					runlog.Result = $"Test failure: Not all expected output was written or program failed to finish in 100000 cycles. Test ID {challengeTest.Id}";
					runlog.CompletedTime = DateTime.Now;
					_context.RunLogs.Add(runlog);
					_context.SaveChanges();
					return Ok("A test failed! Not all expected output was written or the program failed to finish in 100000 cycles.");
				}
			}

			// If successful on all tests, spit out flaggo
			runlog.Result = "Success";
			runlog.CompletedTime = DateTime.Now;
			_context.RunLogs.Add(runlog);
			_context.SaveChanges();
			return Ok(challenge.Flag);
		}

		private List<int> buildList(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				return new List<int>();
			}
			else
			{
				return input.Split(",").Select(s => Int32.Parse(s)).ToList();
			}
		}
	}
}
