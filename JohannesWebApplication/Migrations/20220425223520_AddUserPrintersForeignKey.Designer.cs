﻿// <auto-generated />
using JohannesWebApplication.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace JohannesWebApplication.Migrations
{
    [DbContext(typeof(PrintersDbContext))]
    [Migration("20220425223520_AddUserPrintersForeignKey")]
    partial class AddUserPrintersForeignKey
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("JohannesWebApplication.Models.MaterialModel", b =>
                {
                    b.Property<int>("MaterialID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("MaterialID");

                    b.ToTable("Models");
                });

            modelBuilder.Entity("JohannesWebApplication.Models.PrinterMaterial", b =>
                {
                    b.Property<int>("PrinterMaterialID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaterialId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PrinterId")
                        .HasColumnType("INTEGER");

                    b.HasKey("PrinterMaterialID");

                    b.HasIndex("MaterialId")
                        .IsUnique();

                    b.HasIndex("PrinterId")
                        .IsUnique();

                    b.ToTable("PrinterMaterial");
                });

            modelBuilder.Entity("JohannesWebApplication.Models.PrinterModel", b =>
                {
                    b.Property<int>("PrinterID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SizeX")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SizeY")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SizeZ")
                        .HasColumnType("INTEGER");

                    b.HasKey("PrinterID");

                    b.ToTable("Printers");
                });

            modelBuilder.Entity("JohannesWebApplication.Models.UserPrinter", b =>
                {
                    b.Property<int>("PrinterId")
                        .HasColumnType("INTEGER");

                    b.HasIndex("PrinterId");

                    b.ToTable("UserPrinters");
                });

            modelBuilder.Entity("JohannesWebApplication.Models.PrinterMaterial", b =>
                {
                    b.HasOne("JohannesWebApplication.Models.MaterialModel", "Material")
                        .WithOne("Printer")
                        .HasForeignKey("JohannesWebApplication.Models.PrinterMaterial", "MaterialId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JohannesWebApplication.Models.PrinterModel", "Printer")
                        .WithOne("Material")
                        .HasForeignKey("JohannesWebApplication.Models.PrinterMaterial", "PrinterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Material");

                    b.Navigation("Printer");
                });

            modelBuilder.Entity("JohannesWebApplication.Models.UserPrinter", b =>
                {
                    b.HasOne("JohannesWebApplication.Models.PrinterModel", "Printer")
                        .WithMany()
                        .HasForeignKey("PrinterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Printer");
                });

            modelBuilder.Entity("JohannesWebApplication.Models.MaterialModel", b =>
                {
                    b.Navigation("Printer")
                        .IsRequired();
                });

            modelBuilder.Entity("JohannesWebApplication.Models.PrinterModel", b =>
                {
                    b.Navigation("Material")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
