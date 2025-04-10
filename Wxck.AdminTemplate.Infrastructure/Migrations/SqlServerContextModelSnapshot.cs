﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Wxck.AdminTemplate.Infrastructure.EntityConfigurations;

#nullable disable

namespace Wxck.AdminTemplate.Infrastructure.Migrations
{
    [DbContext(typeof(SqlServerContext))]
    partial class SqlServerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Wxck.AdminTemplate.Domain.Entities.Logs.OperationLogInfoModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedTime");

                    b.Property<bool>("IsQueryDb")
                        .HasColumnType("bit")
                        .HasColumnName("IsQueryDb");

                    b.Property<bool>("IsSuccessful")
                        .HasColumnType("bit")
                        .HasColumnName("IsSuccessful");

                    b.Property<string>("ModifyIp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ModifyIp");

                    b.Property<DateTime>("ModifyTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("ModifyTime");

                    b.Property<string>("OperationDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("OperationDescription");

                    b.Property<DateTime>("OperationTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("OperationTime");

                    b.Property<long>("OperatorId")
                        .HasColumnType("bigint")
                        .HasColumnName("OperatorId");

                    b.Property<long>("QueryDbElapsed")
                        .HasColumnType("bigint")
                        .HasColumnName("QueryDbElapsed");

                    b.Property<string>("Remarks")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Remarks");

                    b.Property<string>("RequestContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("RequestContent");

                    b.Property<string>("RequestIp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("RequestIP");

                    b.Property<string>("RequestMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("RequestMethod");

                    b.Property<string>("ResponseContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ResponseContent");

                    b.Property<long>("TimeSpent")
                        .HasColumnType("bigint")
                        .HasColumnName("TimeSpent");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime")
                        .IsDescending();

                    b.HasIndex("OperationTime")
                        .IsDescending();

                    b.HasIndex("OperatorId");

                    b.HasIndex("RequestMethod");

                    b.ToTable("Log_OperationLog", "dbo");
                });

            modelBuilder.Entity("Wxck.AdminTemplate.Domain.Entities.User.UserInfoModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedTime");

                    b.Property<string>("InviteCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("InviteCode");

                    b.Property<string>("ModifyIp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ModifyIp");

                    b.Property<DateTime>("ModifyTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("ModifyTime");

                    b.Property<string>("PassWord")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PassWord");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Phone");

                    b.Property<string>("Remarks")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Remarks");

                    b.Property<int>("Role")
                        .HasColumnType("int")
                        .HasColumnName("Role");

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnName("Status");

                    b.Property<string>("UserCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("UserCode");

                    b.Property<string>("UserIcon")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserIcon");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UserName");

                    b.HasKey("Id");

                    b.HasIndex("CreatedTime")
                        .IsDescending();

                    b.HasIndex("UserCode")
                        .IsUnique()
                        .IsDescending();

                    b.ToTable("User_UserInfo", "dbo");
                });

            modelBuilder.Entity("Wxck.AdminTemplate.Domain.Entities.User.UserVipInfoModel", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedTime");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Description");

                    b.Property<string>("ModifyIp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ModifyIp");

                    b.Property<DateTime>("ModifyTime")
                        .HasColumnType("datetime2")
                        .HasColumnName("ModifyTime");

                    b.Property<decimal>("RechargePoints")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("RechargePoints");

                    b.Property<string>("Remarks")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Remarks");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint")
                        .HasColumnName("UserId");

                    b.Property<decimal>("VipDiscount")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("VipDiscount");

                    b.Property<int>("VipExperience")
                        .HasColumnType("int")
                        .HasColumnName("VipExperience");

                    b.Property<int>("VipLevel")
                        .HasColumnType("int")
                        .HasColumnName("VipLevel");

                    b.Property<string>("VipName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("VipName");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("User_UserVipInfo", "dbo");
                });

            modelBuilder.Entity("Wxck.AdminTemplate.Domain.Entities.User.UserVipInfoModel", b =>
                {
                    b.HasOne("Wxck.AdminTemplate.Domain.Entities.User.UserInfoModel", "UserInfo")
                        .WithOne("UserVipInfo")
                        .HasForeignKey("Wxck.AdminTemplate.Domain.Entities.User.UserVipInfoModel", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserInfo");
                });

            modelBuilder.Entity("Wxck.AdminTemplate.Domain.Entities.User.UserInfoModel", b =>
                {
                    b.Navigation("UserVipInfo");
                });
#pragma warning restore 612, 618
        }
    }
}
