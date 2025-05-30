﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TestApp.DbContext;

#nullable disable

namespace TestApp.Migrations
{
    [DbContext(typeof(MainContext))]
    [Migration("20250215084531_changes")]
    partial class changes
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("TestApp.Model.SurveyAnswers", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("AnswerText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("bit");

                    b.Property<long>("QuestionId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("SurveyAnswers");
                });

            modelBuilder.Entity("TestApp.Model.SurveyQuestions", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SurveyId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SurveyId");

                    b.ToTable("SurveyQuestions");
                });

            modelBuilder.Entity("TestApp.Model.Surveys", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("CreatedBy")
                        .HasColumnType("bigint");

                    b.Property<long?>("CreatedByUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("VideoId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("VideoId")
                        .IsUnique()
                        .HasFilter("[VideoId] IS NOT NULL");

                    b.ToTable("Surveys");
                });

            modelBuilder.Entity("TestApp.Model.UserReactions", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<bool>("HasAttemptedSurvey")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ReactedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReactionUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long?>("VideoId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("VideoId");

                    b.ToTable("UserReactions");
                });

            modelBuilder.Entity("TestApp.Model.UserSurveyResponses", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("AnsweredAt")
                        .HasColumnType("datetime2");

                    b.Property<long>("QuestionId")
                        .HasColumnType("bigint");

                    b.Property<long>("SelectedAnswerId")
                        .HasColumnType("bigint");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.HasIndex("SelectedAnswerId");

                    b.HasIndex("UserId");

                    b.ToTable("UserSurveyResponses");
                });

            modelBuilder.Entity("TestApp.Model.Users", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long?>("CreatedById")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailOrPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("ModifiedById")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TestApp.Model.Videos", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("SurveyId")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("UploadedBy")
                        .HasColumnType("bigint");

                    b.Property<DateTime?>("UploadedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("VideoType")
                        .HasColumnType("int");

                    b.Property<string>("VideoUrl")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UploadedBy");

                    b.ToTable("Videos");
                });

            modelBuilder.Entity("TestApp.Model.SurveyAnswers", b =>
                {
                    b.HasOne("TestApp.Model.SurveyQuestions", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");
                });

            modelBuilder.Entity("TestApp.Model.SurveyQuestions", b =>
                {
                    b.HasOne("TestApp.Model.Surveys", "Survey")
                        .WithMany("Questions")
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Survey");
                });

            modelBuilder.Entity("TestApp.Model.Surveys", b =>
                {
                    b.HasOne("TestApp.Model.Users", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId");

                    b.HasOne("TestApp.Model.Videos", "Video")
                        .WithOne("Survey")
                        .HasForeignKey("TestApp.Model.Surveys", "VideoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("CreatedByUser");

                    b.Navigation("Video");
                });

            modelBuilder.Entity("TestApp.Model.UserReactions", b =>
                {
                    b.HasOne("TestApp.Model.Users", "Users")
                        .WithMany("Reactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("TestApp.Model.Videos", "Videos")
                        .WithMany("Reactions")
                        .HasForeignKey("VideoId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Users");

                    b.Navigation("Videos");
                });

            modelBuilder.Entity("TestApp.Model.UserSurveyResponses", b =>
                {
                    b.HasOne("TestApp.Model.SurveyQuestions", "Question")
                        .WithMany("UserResponses")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TestApp.Model.SurveyAnswers", "SelectedAnswer")
                        .WithMany("UserResponses")
                        .HasForeignKey("SelectedAnswerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("TestApp.Model.Users", "User")
                        .WithMany("SurveyResponses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Question");

                    b.Navigation("SelectedAnswer");

                    b.Navigation("User");
                });

            modelBuilder.Entity("TestApp.Model.Videos", b =>
                {
                    b.HasOne("TestApp.Model.Users", "Users")
                        .WithMany("UploadedVideos")
                        .HasForeignKey("UploadedBy")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Users");
                });

            modelBuilder.Entity("TestApp.Model.SurveyAnswers", b =>
                {
                    b.Navigation("UserResponses");
                });

            modelBuilder.Entity("TestApp.Model.SurveyQuestions", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("UserResponses");
                });

            modelBuilder.Entity("TestApp.Model.Surveys", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("TestApp.Model.Users", b =>
                {
                    b.Navigation("Reactions");

                    b.Navigation("SurveyResponses");

                    b.Navigation("UploadedVideos");
                });

            modelBuilder.Entity("TestApp.Model.Videos", b =>
                {
                    b.Navigation("Reactions");

                    b.Navigation("Survey");
                });
#pragma warning restore 612, 618
        }
    }
}
