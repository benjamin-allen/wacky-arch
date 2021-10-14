using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProgRunner.Model
{
	public class PRContext : DbContext
	{
		public PRContext(DbContextOptions<PRContext> options) : base(options) { }

		public DbSet<RunLog> RunLogs { get; set; }
		public DbSet<AlphaChallenge> AlphaChallenges { get; set; }
		public DbSet<AlphaChallengeTest> AlphaChallengeTests { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<RunLog>().ToTable("RunLogs");
		}
	}
}
