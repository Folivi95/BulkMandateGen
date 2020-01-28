﻿// <auto-generated />
using BulkMandateGen.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BulkMandateGen.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20200128133714_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BulkMandateGen.Data.SetupMandate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Amount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EndDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MandateId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MandateType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MaxNoOfDebits")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayerAccount")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayerBankCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayerEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayerName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PayerPhone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RequestId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StartDate")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Mandates");
                });
#pragma warning restore 612, 618
        }
    }
}
