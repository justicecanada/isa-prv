﻿// <auto-generated />
using System;
using Interview.UI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Interview.UI.Migrations
{
    [DbContext(typeof(SqlContext))]
    [Migration("20230914162407_RoleDemoProperty")]
    partial class RoleDemoProperty
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Interview.Entities.Contest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CandidatesIntroEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CandidatesIntroFR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTimeOffset?>("DeadlineCandidate")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("DeadlineInterviewer")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("EmailServiceSentFrom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("EndDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("GroupNiv")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("InitUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan?>("InterviewDuration")
                        .HasColumnType("time");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<TimeSpan?>("MaxTime")
                        .HasColumnType("time");

                    b.Property<string>("MembersIntroEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MembersIntroFR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan?>("MinTime")
                        .HasColumnType("time");

                    b.Property<string>("NoProcessus")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("StartDate")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("Contests");
                });

            modelBuilder.Entity("Interview.Entities.EmailTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EmailBody")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailCC")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EmailSubject")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.ToTable("EmailTemplates");
                });

            modelBuilder.Entity("Interview.Entities.EmailType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DescEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DescFR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("EmailTemplateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFR")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("EmailTemplateId")
                        .IsUnique();

                    b.ToTable("EmailTypes");
                });

            modelBuilder.Entity("Interview.Entities.Equity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserSettingId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ViewEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ViewFR")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UserSettingId");

                    b.ToTable("Equities");
                });

            modelBuilder.Entity("Interview.Entities.Interview", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ContactName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContactNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ContestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Duration")
                        .HasColumnType("int");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Room")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset?>("StartDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.ToTable("Interviews");
                });

            modelBuilder.Entity("Interview.Entities.InterviewUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("InterviewId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("InterviewId");

                    b.ToTable("InterviewUsers");
                });

            modelBuilder.Entity("Interview.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DemoProperty")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("RoleNameEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleNameFR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Interview.Entities.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int?>("StartValue")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("Interview.Entities.ScheduleType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ScheduleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId")
                        .IsUnique();

                    b.ToTable("ScheduleTypes");
                });

            modelBuilder.Entity("Interview.Entities.UserLanguage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("UserLanguages");
                });

            modelBuilder.Entity("Interview.Entities.UserSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset?>("DateInserted")
                        .HasColumnType("datetimeoffset");

                    b.Property<bool?>("IsExternal")
                        .HasColumnType("bit");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserFirstname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserLanguageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserLastname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserLanguageId");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("Interview.Entities.EmailTemplate", b =>
                {
                    b.HasOne("Interview.Entities.Contest", null)
                        .WithMany("EmailTemplates")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.EmailType", b =>
                {
                    b.HasOne("Interview.Entities.EmailTemplate", null)
                        .WithOne("EmailType")
                        .HasForeignKey("Interview.Entities.EmailType", "EmailTemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.Equity", b =>
                {
                    b.HasOne("Interview.Entities.UserSetting", null)
                        .WithMany("Equities")
                        .HasForeignKey("UserSettingId");
                });

            modelBuilder.Entity("Interview.Entities.Interview", b =>
                {
                    b.HasOne("Interview.Entities.Contest", null)
                        .WithMany("Interviews")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.InterviewUser", b =>
                {
                    b.HasOne("Interview.Entities.Interview", null)
                        .WithMany("InterviewUsers")
                        .HasForeignKey("InterviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.Schedule", b =>
                {
                    b.HasOne("Interview.Entities.Contest", null)
                        .WithMany("Schedules")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.ScheduleType", b =>
                {
                    b.HasOne("Interview.Entities.Schedule", null)
                        .WithOne("ScheduleType")
                        .HasForeignKey("Interview.Entities.ScheduleType", "ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.UserSetting", b =>
                {
                    b.HasOne("Interview.Entities.Contest", null)
                        .WithMany("UserSettings")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Interview.Entities.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Interview.Entities.UserLanguage", "UserLanguage")
                        .WithMany()
                        .HasForeignKey("UserLanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("UserLanguage");
                });

            modelBuilder.Entity("Interview.Entities.Contest", b =>
                {
                    b.Navigation("EmailTemplates");

                    b.Navigation("Interviews");

                    b.Navigation("Schedules");

                    b.Navigation("UserSettings");
                });

            modelBuilder.Entity("Interview.Entities.EmailTemplate", b =>
                {
                    b.Navigation("EmailType")
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.Interview", b =>
                {
                    b.Navigation("InterviewUsers");
                });

            modelBuilder.Entity("Interview.Entities.Schedule", b =>
                {
                    b.Navigation("ScheduleType")
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.UserSetting", b =>
                {
                    b.Navigation("Equities");
                });
#pragma warning restore 612, 618
        }
    }
}
