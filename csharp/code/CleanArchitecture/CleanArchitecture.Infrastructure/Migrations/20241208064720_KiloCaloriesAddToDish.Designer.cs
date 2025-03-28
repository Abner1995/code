﻿// <auto-generated />
using System;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CleanArchitecture.Infrastructure.Migrations
{
    [DbContext(typeof(RestaurantsDbContext))]
    [Migration("20241208064720_KiloCaloriesAddToDish")]
    partial class KiloCaloriesAddToDish
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CleanArchitecture.Domain.Entities.Dish", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasComment("菜单介绍");

                    b.Property<int?>("KiloCalories")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasComment("菜单名");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)")
                        .HasComment("价格");

                    b.Property<int>("RestaurantId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RestaurantId");

                    b.ToTable("T_Dish", null, t =>
                        {
                            t.HasComment("菜单");
                        });
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Entities.Restaurant", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasComment("餐厅分类");

                    b.Property<string>("ContactEmail")
                        .HasColumnType("nvarchar(max)")
                        .HasComment("邮箱");

                    b.Property<string>("ContactNumber")
                        .HasColumnType("nvarchar(max)")
                        .HasComment("联系号码");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasComment("餐厅介绍");

                    b.Property<bool>("HasDelivery")
                        .HasColumnType("bit")
                        .HasComment("是否配送");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasComment("餐厅名");

                    b.HasKey("Id");

                    b.ToTable("T_Restaurant", null, t =>
                        {
                            t.HasComment("餐厅");
                        });
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Entities.Dish", b =>
                {
                    b.HasOne("CleanArchitecture.Domain.Entities.Restaurant", null)
                        .WithMany("Dishes")
                        .HasForeignKey("RestaurantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Entities.Restaurant", b =>
                {
                    b.OwnsOne("CleanArchitecture.Domain.Entities.Address", "Address", b1 =>
                        {
                            b1.Property<int>("RestaurantId")
                                .HasColumnType("int");

                            b1.Property<string>("City")
                                .HasColumnType("nvarchar(max)")
                                .HasComment("城市");

                            b1.Property<string>("PostalCode")
                                .HasColumnType("nvarchar(max)")
                                .HasComment("邮政编码");

                            b1.Property<string>("Street")
                                .HasColumnType("nvarchar(max)")
                                .HasComment("街道");

                            b1.HasKey("RestaurantId");

                            b1.ToTable("T_Restaurant");

                            b1.WithOwner()
                                .HasForeignKey("RestaurantId");
                        });

                    b.Navigation("Address");
                });

            modelBuilder.Entity("CleanArchitecture.Domain.Entities.Restaurant", b =>
                {
                    b.Navigation("Dishes");
                });
#pragma warning restore 612, 618
        }
    }
}
