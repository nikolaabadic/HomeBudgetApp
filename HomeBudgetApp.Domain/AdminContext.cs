using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class AdminContext : DbContext
    {
        public DbSet<Admin> Admin { get; set; }
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlServer(@"Server = (localdb)\mssqllocaldb; Database = HomeBudget; ");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin { AdminID = 1, Name = "Nikola", Surname = "Abadic", Username = "nikolaabadic", Password = "1234" }
            );
        }
    }
}
