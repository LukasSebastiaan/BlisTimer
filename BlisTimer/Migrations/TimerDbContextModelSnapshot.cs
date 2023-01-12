﻿// <auto-generated />
using System;
using BlisTimer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlisTimer.Migrations
{
    [DbContext(typeof(TimerDbContext))]
    partial class TimerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseSerialColumns(modelBuilder);

            modelBuilder.Entity("Domain.Models.Employee", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<string>("RunningTimerId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Domain.Models.EmployeeProject", b =>
                {
                    b.Property<string>("EmployeeId")
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .HasColumnType("text");

                    b.HasKey("EmployeeId", "ProjectId");

                    b.HasIndex("ProjectId");

                    b.ToTable("EmployeeProjects");
                });

            modelBuilder.Entity("Domain.Models.HourType", b =>
                {
                    b.Property<string>("HourTypeId")
                        .HasColumnType("text");

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("HourTypeId");

                    b.ToTable("HourTypes");
                });

            modelBuilder.Entity("Domain.Models.Preferences", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("ChangeCountTimeSeconds")
                        .HasColumnType("integer");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("NotificationEnabled")
                        .HasColumnType("integer");

                    b.Property<int>("NotificationTimeSeconds")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.ToTable("Preferences");
                });

            modelBuilder.Entity("Domain.Models.Project", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Domain.Models.RunningTimer", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ActivityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("HourTypeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.ToTable("RunningTimers");
                });

            modelBuilder.Entity("Domain.Models.TimeLog", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ActivityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("EmployeeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("HourTypeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("Submitted")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("HourTypeId");

                    b.HasIndex("ProjectId");

                    b.ToTable("TimeLogs");
                });

            modelBuilder.Entity("Domain.Models.WorkActivity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("WorkActivities");
                });

            modelBuilder.Entity("Domain.Models.WorkActivityHourType", b =>
                {
                    b.Property<string>("WorkActivityId")
                        .HasColumnType("text");

                    b.Property<string>("HourTypeId")
                        .HasColumnType("text");

                    b.HasKey("WorkActivityId", "HourTypeId");

                    b.HasIndex("HourTypeId");

                    b.ToTable("WorkActivityHourTypes");
                });

            modelBuilder.Entity("Domain.Models.EmployeeProject", b =>
                {
                    b.HasOne("Domain.Models.Employee", "Employee")
                        .WithMany("EmployeeProjects")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Models.Project", "Project")
                        .WithMany("EmployeeProjects")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Domain.Models.Preferences", b =>
                {
                    b.HasOne("Domain.Models.Employee", "Employee")
                        .WithOne("Preferences")
                        .HasForeignKey("Domain.Models.Preferences", "EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Domain.Models.RunningTimer", b =>
                {
                    b.HasOne("Domain.Models.Employee", "Employee")
                        .WithOne("RunningTimer")
                        .HasForeignKey("Domain.Models.RunningTimer", "EmployeeId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Domain.Models.TimeLog", b =>
                {
                    b.HasOne("Domain.Models.WorkActivity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("Domain.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("Domain.Models.HourType", "HourType")
                        .WithMany()
                        .HasForeignKey("HourTypeId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.HasOne("Domain.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Activity");

                    b.Navigation("Employee");

                    b.Navigation("HourType");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Domain.Models.WorkActivity", b =>
                {
                    b.HasOne("Domain.Models.Project", "Project")
                        .WithMany("Activities")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.SetNull)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Domain.Models.WorkActivityHourType", b =>
                {
                    b.HasOne("Domain.Models.HourType", "HourType")
                        .WithMany("WorkActivityHourTypes")
                        .HasForeignKey("HourTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Models.WorkActivity", "WorkActivity")
                        .WithMany("WorkActivityHourTypes")
                        .HasForeignKey("WorkActivityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HourType");

                    b.Navigation("WorkActivity");
                });

            modelBuilder.Entity("Domain.Models.Employee", b =>
                {
                    b.Navigation("EmployeeProjects");

                    b.Navigation("Preferences")
                        .IsRequired();

                    b.Navigation("RunningTimer");
                });

            modelBuilder.Entity("Domain.Models.HourType", b =>
                {
                    b.Navigation("WorkActivityHourTypes");
                });

            modelBuilder.Entity("Domain.Models.Project", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("EmployeeProjects");
                });

            modelBuilder.Entity("Domain.Models.WorkActivity", b =>
                {
                    b.Navigation("WorkActivityHourTypes");
                });
#pragma warning restore 612, 618
        }
    }
}
