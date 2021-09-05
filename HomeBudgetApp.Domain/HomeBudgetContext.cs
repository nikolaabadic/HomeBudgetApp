using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetApp.Domain
{
    public class HomeBudgetContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionCategory> TransactionCategories { get; set; }
        public DbSet<Template> Templates { get; set; }

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
            modelBuilder.Entity<Account>().OwnsMany(a => a.PaymentCards);

            modelBuilder.Entity<Transaction>(t =>
            {
                t.HasOne(t => t.Account).WithMany(a => a.TransactionsTo).OnDelete(DeleteBehavior.Restrict);
                t.HasOne(t => t.Recipient).WithMany(a => a.TransactionsFrom).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TransactionCategory>(c =>
            {
                c.HasKey(c => new { c.CategoryID, c.TransactionID, c.OwnerID });
                c.HasOne(b => b.Transaction).WithMany(p => p.Categories).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Transaction>().OwnsOne(t => t.PaymentCard);

            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { UserID = 1, Name = "Mark", Surname = "Ronson", Username = "markronson", Password = "1234" },
                new User { UserID = 2, Name = "John", Surname = "Parker", Username = "johnparker", Password = "1111" }
            );
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, Name = "Bills", Description = "Electricity, Water, Cabel,.." },
                new Category { CategoryID = 2, Name = "Food", Description = "Grocery shopping, Restorants,..." },
                new Category { CategoryID = 3, Name = "Social", Description = "Cinema, Coffee shop,..." },
                new Category { CategoryID = 4, Name = "Sports", Description = "Swimming, tennis,..." },
                new Category { CategoryID = 5, Name = "Travel", Description = "Plane tickets, hotel,..." }
            );
        }
    }
}
