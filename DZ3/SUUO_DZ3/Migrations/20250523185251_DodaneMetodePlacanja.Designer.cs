﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SUUO_DZ3.Data;

#nullable disable

namespace SUUO_DZ3.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250523185251_DodaneMetodePlacanja")]
    partial class DodaneMetodePlacanja
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SUUO_DZ3.Models.Konobar", b =>
                {
                    b.Property<Guid>("IdKonobar")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Aktivan")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prezime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdKonobar");

                    b.ToTable("Konobari");
                });

            modelBuilder.Entity("SUUO_DZ3.Models.Kuhar", b =>
                {
                    b.Property<Guid>("IdKuhar")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Aktivan")
                        .HasColumnType("bit");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Prezime")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Specijaliteti")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Telefon")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdKuhar");

                    b.ToTable("Kuhari");
                });

            modelBuilder.Entity("SUUO_DZ3.Models.Narudzba", b =>
                {
                    b.Property<Guid>("NarudzbaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("KonobarId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("MetodaPlacanja")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Stol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("VrijemeNarudzbe")
                        .HasColumnType("datetime2");

                    b.HasKey("NarudzbaId");

                    b.HasIndex("KonobarId");

                    b.ToTable("Narudzbe");
                });

            modelBuilder.Entity("SUUO_DZ3.Models.StavkaNarudzbe", b =>
                {
                    b.Property<Guid>("StavkaNarudzbeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("AkcijskaPonuda")
                        .HasColumnType("bit");

                    b.Property<decimal>("Cijena")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Kolicina")
                        .HasColumnType("int");

                    b.Property<Guid>("NarudzbaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Naziv")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("StavkaNarudzbeId");

                    b.HasIndex("NarudzbaId");

                    b.ToTable("StavkeNarudzbe");
                });

            modelBuilder.Entity("SUUO_DZ3.Models.Narudzba", b =>
                {
                    b.HasOne("SUUO_DZ3.Models.Konobar", "Konobar")
                        .WithMany()
                        .HasForeignKey("KonobarId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Konobar");
                });

            modelBuilder.Entity("SUUO_DZ3.Models.StavkaNarudzbe", b =>
                {
                    b.HasOne("SUUO_DZ3.Models.Narudzba", null)
                        .WithMany("StavkeNarudzbi")
                        .HasForeignKey("NarudzbaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SUUO_DZ3.Models.Narudzba", b =>
                {
                    b.Navigation("StavkeNarudzbi");
                });
#pragma warning restore 612, 618
        }
    }
}
