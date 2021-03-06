﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using WebApp.Data;

namespace WebApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20180425002044_AddressModelMakeGooglePlaceId60Chars")]
    partial class AddressModelMakeGooglePlaceId60Chars
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.0-preview2-30571")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApp.Models.AccountModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(35);

                    b.Property<byte[]>("IpAddress")
                        .HasMaxLength(16);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(35);

                    b.Property<string>("Phone")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("WebApp.Models.AddressModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<string>("GooglePlaceId")
                        .IsRequired()
                        .HasMaxLength(60);

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(2);

                    b.Property<string>("StreetName")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("StreetNumber")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("GooglePlaceId")
                        .IsUnique();

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("WebApp.Models.RentEstimateModel", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("AccountId");

                    b.Property<Guid>("AddressId");

                    b.Property<decimal>("ExpectedRent");

                    b.Property<decimal?>("RentZestimateHigh");

                    b.Property<decimal?>("RentZestimateLow");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("AddressId");

                    b.ToTable("RentEstimates");
                });

            modelBuilder.Entity("WebApp.Models.RentEstimateModel", b =>
                {
                    b.HasOne("WebApp.Models.AccountModel", "Account")
                        .WithMany("RentEstimates")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApp.Models.AddressModel", "Address")
                        .WithMany("RentEstimates")
                        .HasForeignKey("AddressId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
