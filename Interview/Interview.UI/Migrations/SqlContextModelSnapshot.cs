﻿// <auto-generated />
using System;
using Interview.UI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Interview.UI.Migrations
{
    [DbContext(typeof(SqlContext))]
    partial class SqlContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ContestGroup", b =>
                {
                    b.Property<Guid>("ContestsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GroupsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ContestsId", "GroupsId");

                    b.HasIndex("GroupsId");

                    b.ToTable("ContestGroup");
                });

            modelBuilder.Entity("EquityUserSetting", b =>
                {
                    b.Property<Guid>("EquitiesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserSettingsId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EquitiesId", "UserSettingsId");

                    b.HasIndex("UserSettingsId");

                    b.ToTable("EquityUserSetting");
                });

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

                    b.Property<DateTime?>("DeadlineCandidate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeadlineInterviewer")
                        .HasColumnType("datetime2");

                    b.Property<Guid?>("DepartmentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EmailServiceSentFrom")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

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

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Contests");
                });

            modelBuilder.Entity("Interview.Entities.ContestGroup", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.HasIndex("GroupId");

                    b.ToTable("ContestGroups");
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

                    b.Property<int>("EmailType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.ToTable("EmailTemplates");
                });

            modelBuilder.Entity("Interview.Entities.Equity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFR")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ViewEN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ViewFR")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Equities");
                });

            modelBuilder.Entity("Interview.Entities.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("NameEn")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFr")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Interview.Entities.GroupOwner", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("HasAccessEE")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupsOwners");
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

            modelBuilder.Entity("Interview.Entities.Schedule", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("ScheduleType")
                        .HasColumnType("int");

                    b.Property<int?>("StartValue")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.ToTable("Schedules");
                });

            modelBuilder.Entity("Interview.Entities.UserSetting", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ContestId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("DateInserted")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("IsExternal")
                        .HasColumnType("bit");

                    b.Property<int?>("LanguageType")
                        .HasColumnType("int");

                    b.Property<int>("RoleType")
                        .HasColumnType("int");

                    b.Property<string>("UserFirstname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserLastname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ContestId");

                    b.ToTable("UserSettings");
                });

            modelBuilder.Entity("Interview.Entities.UserSettingEquity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("EquityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserSettingId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("EquityId");

                    b.HasIndex("UserSettingId");

                    b.ToTable("UserSettingEquities");
                });

            modelBuilder.Entity("Interview.UI.Services.Mock.Departments.MockDepartment", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Key")
                        .HasColumnType("int");

                    b.Property<string>("NameEN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameFR")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MockDepartments");
                });

            modelBuilder.Entity("Interview.UI.Services.Mock.Identity.MockUser", b =>
                {
                    b.Property<Guid?>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Roles")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("UserType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MockUsers");
                });

            modelBuilder.Entity("ContestGroup", b =>
                {
                    b.HasOne("Interview.Entities.Contest", null)
                        .WithMany()
                        .HasForeignKey("ContestsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Interview.Entities.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("EquityUserSetting", b =>
                {
                    b.HasOne("Interview.Entities.Equity", null)
                        .WithMany()
                        .HasForeignKey("EquitiesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Interview.Entities.UserSetting", null)
                        .WithMany()
                        .HasForeignKey("UserSettingsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.ContestGroup", b =>
                {
                    b.HasOne("Interview.Entities.Contest", "Contest")
                        .WithMany("ContestGroups")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Interview.Entities.Group", "Group")
                        .WithMany("ContestGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contest");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("Interview.Entities.EmailTemplate", b =>
                {
                    b.HasOne("Interview.Entities.Contest", null)
                        .WithMany("EmailTemplates")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.GroupOwner", b =>
                {
                    b.HasOne("Interview.Entities.Group", "Group")
                        .WithMany("GroupOwners")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");
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

            modelBuilder.Entity("Interview.Entities.UserSetting", b =>
                {
                    b.HasOne("Interview.Entities.Contest", null)
                        .WithMany("UserSettings")
                        .HasForeignKey("ContestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Interview.Entities.UserSettingEquity", b =>
                {
                    b.HasOne("Interview.Entities.Equity", "Equity")
                        .WithMany("EmailTemplateEquities")
                        .HasForeignKey("EquityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Interview.Entities.UserSetting", "UserSetting")
                        .WithMany("UserSettingEquities")
                        .HasForeignKey("UserSettingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equity");

                    b.Navigation("UserSetting");
                });

            modelBuilder.Entity("Interview.Entities.Contest", b =>
                {
                    b.Navigation("ContestGroups");

                    b.Navigation("EmailTemplates");

                    b.Navigation("Interviews");

                    b.Navigation("Schedules");

                    b.Navigation("UserSettings");
                });

            modelBuilder.Entity("Interview.Entities.Equity", b =>
                {
                    b.Navigation("EmailTemplateEquities");
                });

            modelBuilder.Entity("Interview.Entities.Group", b =>
                {
                    b.Navigation("ContestGroups");

                    b.Navigation("GroupOwners");
                });

            modelBuilder.Entity("Interview.Entities.Interview", b =>
                {
                    b.Navigation("InterviewUsers");
                });

            modelBuilder.Entity("Interview.Entities.UserSetting", b =>
                {
                    b.Navigation("UserSettingEquities");
                });
#pragma warning restore 612, 618
        }
    }
}
