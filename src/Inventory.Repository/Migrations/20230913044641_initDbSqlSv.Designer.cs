﻿// <auto-generated />
using System;
using Inventory.Repository.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Inventory.Repository.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230913044641_initDbSqlSv")]
    partial class initDbSqlSv
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Inventory.Repository.Model.CatalogEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Catalogs");
                });

            modelBuilder.Entity("Inventory.Repository.Model.DecisionEntity", b =>
                {
                    b.Property<string>("ById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("ById", "Date");

                    b.ToTable("DecisionEntity");
                });

            modelBuilder.Entity("Inventory.Repository.Model.ExportDetailEntity", b =>
                {
                    b.Property<int>("ExportId")
                        .HasColumnType("int");

                    b.Property<Guid>("ItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ExportId", "ItemId");

                    b.HasIndex("ItemId");

                    b.ToTable("ExportDetails");
                });

            modelBuilder.Entity("Inventory.Repository.Model.ExportEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ForId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UpdatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("ForId");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Exports");
                });

            modelBuilder.Entity("Inventory.Repository.Model.ItemEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("CatalogId")
                        .HasColumnType("int");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CreatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("InStock")
                        .HasColumnType("int");

                    b.Property<int>("InUsing")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UpdatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Inventory.Repository.Model.OrderDetailEntity", b =>
                {
                    b.Property<Guid>("ItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("OrderInfoId")
                        .HasColumnType("int");

                    b.Property<long>("MaxPrice")
                        .HasColumnType("bigint");

                    b.Property<long>("MaxTotal")
                        .HasColumnType("bigint");

                    b.Property<long>("MinPrice")
                        .HasColumnType("bigint");

                    b.Property<long>("MinTotal")
                        .HasColumnType("bigint");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ItemId", "OrderInfoId");

                    b.HasIndex("OrderInfoId");

                    b.ToTable("OrderDetailEntity");
                });

            modelBuilder.Entity("Inventory.Repository.Model.OrderEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CompleteDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Inventory.Repository.Model.OrderInfoEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DecisionById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DecisionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("MaxTotal")
                        .HasColumnType("bigint");

                    b.Property<long>("MinTotal")
                        .HasColumnType("bigint");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("DecisionById", "DecisionDate");

                    b.ToTable("OrderInfos");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TeamEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LeaderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("LeaderId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TicketDetailEntity", b =>
                {
                    b.Property<Guid>("ItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("TicketInfoId")
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("ItemId", "TicketInfoId");

                    b.HasIndex("TicketInfoId");

                    b.ToTable("TicketDetailEntity");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TicketEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CloseDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("UpdatedById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    b.HasIndex("UpdatedById");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TicketInfoEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CloseAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DecisionById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("DecisionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LeaderDecisionById")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("LeaderDecisionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Purpose")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int?>("TicketId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TicketId");

                    b.HasIndex("DecisionById", "DecisionDate");

                    b.HasIndex("LeaderDecisionById", "LeaderDecisionDate");

                    b.ToTable("TicketInfoEntity");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "46a4f2b7-2a9e-4977-ae32-e0e5793e6267",
                            Name = "Normal User",
                            NormalizedName = "NORMAL USER"
                        },
                        new
                        {
                            Id = "f8b59b69-fabb-4386-948e-5fb7054ffff4",
                            Name = "Team Leader",
                            NormalizedName = "TEAM LEADER"
                        },
                        new
                        {
                            Id = "4e5e4a2b-9b92-40fa-87f2-1fefc574336b",
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR"
                        },
                        new
                        {
                            Id = "fc2a7273-a3c2-47be-bc55-aab11097e09a",
                            Name = "Super Administrator",
                            NormalizedName = "SUPER ADMINISTRATOR"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "d2f7a36c-d4a6-43db-8fe9-74598da4c352",
                            RoleId = "fc2a7273-a3c2-47be-bc55-aab11097e09a"
                        },
                        new
                        {
                            UserId = "F5EE313D-9B16-45C0-BA54-8D4E9628EFD8",
                            RoleId = "4e5e4a2b-9b92-40fa-87f2-1fefc574336b"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Inventory.Repository.Model.AppUserEntity", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("RefreshTokenExpireTime")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("TeamId");

                    b.HasDiscriminator().HasValue("AppUserEntity");

                    b.HasData(
                        new
                        {
                            Id = "d2f7a36c-d4a6-43db-8fe9-74598da4c352",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "3a896ede-cfbd-4bf5-b412-7acfb4d35566",
                            Email = "sa@local.com",
                            EmailConfirmed = false,
                            LockoutEnabled = false,
                            NormalizedEmail = "SA@LOCAL.COM",
                            NormalizedUserName = "SUPERADMIN",
                            PasswordHash = "AQAAAAIAAYagAAAAEEcB5Ia0Tk9GDL1zxG6RYOrNblMYDJPjAvSTstIDSjtfCxoEbm3rT5HwFUcKsOCtAA==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "598834df-22d0-498e-ad7b-865e9b9813d3",
                            TwoFactorEnabled = false,
                            UserName = "superadmin"
                        },
                        new
                        {
                            Id = "F5EE313D-9B16-45C0-BA54-8D4E9628EFD8",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "507c91a6-6bc5-4198-a535-3e98c8b4de95",
                            Email = "admin@local.com",
                            EmailConfirmed = false,
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@LOCAL.COM",
                            NormalizedUserName = "ADMIN",
                            PasswordHash = "AQAAAAIAAYagAAAAEAEInLbgly2MRI2KPxxgMx/iHaP7vGR51EOeX6FtSAHIi/SntL7M68o7fa7SKffd7g==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "f311cb33-4319-4513-9e57-1654b544814b",
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("Inventory.Repository.Model.DecisionEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "ByUser")
                        .WithMany()
                        .HasForeignKey("ById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ByUser");
                });

            modelBuilder.Entity("Inventory.Repository.Model.ExportDetailEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.ExportEntity", "Export")
                        .WithMany("Details")
                        .HasForeignKey("ExportId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Inventory.Repository.Model.ItemEntity", "Item")
                        .WithMany("ExportDetails")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Export");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Inventory.Repository.Model.ExportEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "ForUser")
                        .WithMany()
                        .HasForeignKey("ForId");

                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "UpdatedByUser")
                        .WithMany()
                        .HasForeignKey("UpdatedById");

                    b.Navigation("CreatedByUser");

                    b.Navigation("ForUser");

                    b.Navigation("UpdatedByUser");
                });

            modelBuilder.Entity("Inventory.Repository.Model.ItemEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.CatalogEntity", "Catalog")
                        .WithMany()
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "UpdatedByUser")
                        .WithMany()
                        .HasForeignKey("UpdatedById");

                    b.Navigation("Catalog");

                    b.Navigation("CreatedByUser");

                    b.Navigation("UpdatedByUser");
                });

            modelBuilder.Entity("Inventory.Repository.Model.OrderDetailEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.ItemEntity", "Item")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Inventory.Repository.Model.OrderInfoEntity", null)
                        .WithMany("Details")
                        .HasForeignKey("OrderInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Inventory.Repository.Model.OrderEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "UpdatedByUser")
                        .WithMany()
                        .HasForeignKey("UpdatedById");

                    b.Navigation("CreatedByUser");

                    b.Navigation("UpdatedByUser");
                });

            modelBuilder.Entity("Inventory.Repository.Model.OrderInfoEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.OrderEntity", "Order")
                        .WithMany("History")
                        .HasForeignKey("OrderId");

                    b.HasOne("Inventory.Repository.Model.DecisionEntity", "Decision")
                        .WithMany()
                        .HasForeignKey("DecisionById", "DecisionDate");

                    b.Navigation("Decision");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TeamEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "Leader")
                        .WithMany()
                        .HasForeignKey("LeaderId");

                    b.Navigation("Leader");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TicketDetailEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.ItemEntity", "Item")
                        .WithMany("TicketDetails")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Inventory.Repository.Model.TicketInfoEntity", null)
                        .WithMany("Details")
                        .HasForeignKey("TicketInfoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TicketEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedById");

                    b.HasOne("Inventory.Repository.Model.AppUserEntity", "UpdatedByUser")
                        .WithMany()
                        .HasForeignKey("UpdatedById");

                    b.Navigation("CreatedByUser");

                    b.Navigation("UpdatedByUser");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TicketInfoEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.TicketEntity", "Ticket")
                        .WithMany("History")
                        .HasForeignKey("TicketId");

                    b.HasOne("Inventory.Repository.Model.DecisionEntity", "Decision")
                        .WithMany()
                        .HasForeignKey("DecisionById", "DecisionDate");

                    b.HasOne("Inventory.Repository.Model.DecisionEntity", "LeaderDecision")
                        .WithMany()
                        .HasForeignKey("LeaderDecisionById", "LeaderDecisionDate");

                    b.Navigation("Decision");

                    b.Navigation("LeaderDecision");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Inventory.Repository.Model.AppUserEntity", b =>
                {
                    b.HasOne("Inventory.Repository.Model.TeamEntity", "Team")
                        .WithMany("Members")
                        .HasForeignKey("TeamId");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Inventory.Repository.Model.ExportEntity", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("Inventory.Repository.Model.ItemEntity", b =>
                {
                    b.Navigation("ExportDetails");

                    b.Navigation("OrderDetails");

                    b.Navigation("TicketDetails");
                });

            modelBuilder.Entity("Inventory.Repository.Model.OrderEntity", b =>
                {
                    b.Navigation("History");
                });

            modelBuilder.Entity("Inventory.Repository.Model.OrderInfoEntity", b =>
                {
                    b.Navigation("Details");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TeamEntity", b =>
                {
                    b.Navigation("Members");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TicketEntity", b =>
                {
                    b.Navigation("History");
                });

            modelBuilder.Entity("Inventory.Repository.Model.TicketInfoEntity", b =>
                {
                    b.Navigation("Details");
                });
#pragma warning restore 612, 618
        }
    }
}