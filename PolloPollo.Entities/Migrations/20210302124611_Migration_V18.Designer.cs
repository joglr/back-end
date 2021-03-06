﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PolloPollo.Entities;

namespace PolloPollo.Entities.Migrations
{
    [DbContext(typeof(PolloPolloContext))]
    [Migration("20210302124611_Migration_V18")]
    partial class Migration_V18
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PolloPollo.Entities.Application", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateOfDonation")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DonationDate")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Motivation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("UserId");

                    b.ToTable("Applications");
                });

            modelBuilder.Entity("PolloPollo.Entities.ByteExchangeRate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<decimal>("GBYTE_USD")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.ToTable("ByteExchangeRate");
                });

            modelBuilder.Entity("PolloPollo.Entities.Contracts", b =>
                {
                    b.Property<int>("ApplicationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Bytes")
                        .HasColumnType("int");

                    b.Property<int?>("Completed")
                        .HasColumnType("int");

                    b.Property<string>("ConfirmKey")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("CreationTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("DonorDevice")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("DonorWallet")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int?>("Price")
                        .HasColumnType("int");

                    b.Property<string>("ProducerDevice")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("ProducerWallet")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SharedAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("ApplicationId");

                    b.ToTable("Contracts");
                });

            modelBuilder.Entity("PolloPollo.Entities.Producer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("DeviceAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PairingSecret")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("StreetNumber")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("WalletAddress")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Zipcode")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Producers");
                });

            modelBuilder.Entity("PolloPollo.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Available")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Country")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Location")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<int>("Price")
                        .HasColumnType("int");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("PolloPollo.Entities.Receiver", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Receivers");
                });

            modelBuilder.Entity("PolloPollo.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(191) CHARACTER SET utf8mb4")
                        .HasMaxLength(191);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("SurName")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<string>("Thumbnail")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasAlternateKey("Email")
                        .HasName("AlternateKey_UserEmail");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PolloPollo.Entities.UserRole", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("UserRoleEnum")
                        .HasColumnType("int");

                    b.HasKey("UserId", "UserRoleEnum");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("PolloPollo.Entities.Application", b =>
                {
                    b.HasOne("PolloPollo.Entities.Product", "Product")
                        .WithMany("Applications")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PolloPollo.Entities.User", "User")
                        .WithMany("Applications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PolloPollo.Entities.Producer", b =>
                {
                    b.HasOne("PolloPollo.Entities.User", "User")
                        .WithOne("Producer")
                        .HasForeignKey("PolloPollo.Entities.Producer", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PolloPollo.Entities.Product", b =>
                {
                    b.HasOne("PolloPollo.Entities.User", "User")
                        .WithMany("Products")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PolloPollo.Entities.Receiver", b =>
                {
                    b.HasOne("PolloPollo.Entities.User", "User")
                        .WithOne("Receiver")
                        .HasForeignKey("PolloPollo.Entities.Receiver", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PolloPollo.Entities.UserRole", b =>
                {
                    b.HasOne("PolloPollo.Entities.User", null)
                        .WithOne("UserRole")
                        .HasForeignKey("PolloPollo.Entities.UserRole", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
