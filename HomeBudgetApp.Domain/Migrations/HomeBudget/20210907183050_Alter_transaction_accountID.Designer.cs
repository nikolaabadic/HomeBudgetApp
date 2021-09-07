﻿// <auto-generated />
using System;
using HomeBudgetApp.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HomeBudgetApp.Domain.Migrations.HomeBudget
{
    [DbContext(typeof(HomeBudgetContext))]
    [Migration("20210907183050_Alter_transaction_accountID")]
    partial class Alter_transaction_accountID
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.9")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("HomeBudgetApp.Domain.Account", b =>
                {
                    b.Property<int>("AccountID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountType")
                        .HasColumnType("int");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<int>("Currency")
                        .HasColumnType("int");

                    b.Property<string>("Number")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TotalExpense")
                        .HasColumnType("float");

                    b.Property<double>("TotalIncome")
                        .HasColumnType("float");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("AccountID");

                    b.HasIndex("UserID");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Category", b =>
                {
                    b.Property<int>("CategoryID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CategoryID");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryID = 1,
                            Description = "Electricity, Water, Cabel,..",
                            Name = "Bills"
                        },
                        new
                        {
                            CategoryID = 2,
                            Description = "Grocery shopping, Restorants,...",
                            Name = "Food"
                        },
                        new
                        {
                            CategoryID = 3,
                            Description = "Cinema, Coffee shop,...",
                            Name = "Social"
                        },
                        new
                        {
                            CategoryID = 4,
                            Description = "Swimming, tennis,...",
                            Name = "Sports"
                        },
                        new
                        {
                            CategoryID = 5,
                            Description = "Plane tickets, hotel,...",
                            Name = "Travel"
                        });
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Template", b =>
                {
                    b.Property<int>("TemplateID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<int>("CategoryID")
                        .HasColumnType("int");

                    b.Property<int>("Model")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Purpose")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientAccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReferenceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("TemplateID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("UserID");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Transaction", b =>
                {
                    b.Property<int>("TransactionID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AccountID")
                        .HasColumnType("int");

                    b.Property<string>("AccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Amount")
                        .HasColumnType("float");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Model")
                        .HasColumnType("int");

                    b.Property<int?>("PaymentCardID")
                        .HasColumnType("int");

                    b.Property<string>("Purpose")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientAccountNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecipientAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RecipientID")
                        .HasColumnType("int");

                    b.Property<string>("RecipientName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ReferenceNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TransactionID");

                    b.HasIndex("AccountID");

                    b.HasIndex("RecipientID");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.TransactionCategory", b =>
                {
                    b.Property<int>("CategoryID")
                        .HasColumnType("int");

                    b.Property<int>("TransactionID")
                        .HasColumnType("int");

                    b.Property<int>("OwnerID")
                        .HasColumnType("int");

                    b.HasKey("CategoryID", "TransactionID", "OwnerID");

                    b.HasIndex("TransactionID");

                    b.ToTable("TransactionCategories");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserID");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserID = 1,
                            Name = "Mark",
                            Password = "1234",
                            Surname = "Ronson",
                            Username = "markronson"
                        },
                        new
                        {
                            UserID = 2,
                            Name = "John",
                            Password = "1111",
                            Surname = "Parker",
                            Username = "johnparker"
                        });
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Account", b =>
                {
                    b.HasOne("HomeBudgetApp.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("HomeBudgetApp.Domain.PaymentCard", "PaymentCards", b1 =>
                        {
                            b1.Property<int>("AccountID")
                                .HasColumnType("int");

                            b1.Property<int>("PaymentCardID")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("PINCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("PaymentCardNumber")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("AccountID", "PaymentCardID");

                            b1.ToTable("Accounts_PaymentCards");

                            b1.WithOwner()
                                .HasForeignKey("AccountID");
                        });

                    b.Navigation("PaymentCards");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Template", b =>
                {
                    b.HasOne("HomeBudgetApp.Domain.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeBudgetApp.Domain.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("User");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Transaction", b =>
                {
                    b.HasOne("HomeBudgetApp.Domain.Account", "Account")
                        .WithMany("TransactionsTo")
                        .HasForeignKey("AccountID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("HomeBudgetApp.Domain.Account", "Recipient")
                        .WithMany("TransactionsFrom")
                        .HasForeignKey("RecipientID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.OwnsOne("HomeBudgetApp.Domain.PaymentCard", "PaymentCard", b1 =>
                        {
                            b1.Property<int>("TransactionID")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("int")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<string>("PINCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<int>("PaymentCardID")
                                .HasColumnType("int");

                            b1.Property<string>("PaymentCardNumber")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("TransactionID");

                            b1.ToTable("Transactions");

                            b1.WithOwner()
                                .HasForeignKey("TransactionID");
                        });

                    b.Navigation("Account");

                    b.Navigation("PaymentCard");

                    b.Navigation("Recipient");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.TransactionCategory", b =>
                {
                    b.HasOne("HomeBudgetApp.Domain.Category", "Category")
                        .WithMany("Transactions")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeBudgetApp.Domain.Transaction", "Transaction")
                        .WithMany("Categories")
                        .HasForeignKey("TransactionID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Account", b =>
                {
                    b.Navigation("TransactionsFrom");

                    b.Navigation("TransactionsTo");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Category", b =>
                {
                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("HomeBudgetApp.Domain.Transaction", b =>
                {
                    b.Navigation("Categories");
                });
#pragma warning restore 612, 618
        }
    }
}
