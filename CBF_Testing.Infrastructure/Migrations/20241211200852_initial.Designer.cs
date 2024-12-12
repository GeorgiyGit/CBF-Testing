﻿// <auto-generated />
using CBF_Testing.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CBF_Testing.Infrastructure.Migrations
{
    [DbContext(typeof(CBFTestingDbContext))]
    [Migration("20241211200852_initial")]
    partial class initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("AnimeGenre", b =>
                {
                    b.Property<int>("AnimesId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GenresId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AnimesId", "GenresId");

                    b.HasIndex("GenresId");

                    b.ToTable("AnimeGenre");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.Anime", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EpisodesCount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Members")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Rating")
                        .HasColumnType("REAL");

                    b.Property<int>("TypeId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TypeId");

                    b.ToTable("Animes");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.AnimeType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AnimeTypes");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.RatingFeedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AnimeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Rating")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AnimeId");

                    b.HasIndex("UserId");

                    b.ToTable("RatingFeedbacks");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.ViewFeedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AnimeId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AnimeId");

                    b.HasIndex("UserId");

                    b.ToTable("ViewFeedbacks");
                });

            modelBuilder.Entity("AnimeGenre", b =>
                {
                    b.HasOne("CBF_Testing.Domain.Entities.Anime", null)
                        .WithMany()
                        .HasForeignKey("AnimesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CBF_Testing.Domain.Entities.Genre", null)
                        .WithMany()
                        .HasForeignKey("GenresId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.Anime", b =>
                {
                    b.HasOne("CBF_Testing.Domain.Entities.AnimeType", "Type")
                        .WithMany("Animes")
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Type");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.RatingFeedback", b =>
                {
                    b.HasOne("CBF_Testing.Domain.Entities.Anime", "Anime")
                        .WithMany("RatingFeedbacks")
                        .HasForeignKey("AnimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CBF_Testing.Domain.Entities.User", "User")
                        .WithMany("RatingFeedbacks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Anime");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.ViewFeedback", b =>
                {
                    b.HasOne("CBF_Testing.Domain.Entities.Anime", "Anime")
                        .WithMany("ViewFeedbacks")
                        .HasForeignKey("AnimeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CBF_Testing.Domain.Entities.User", "User")
                        .WithMany("ViewFeedbacks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Anime");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.Anime", b =>
                {
                    b.Navigation("RatingFeedbacks");

                    b.Navigation("ViewFeedbacks");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.AnimeType", b =>
                {
                    b.Navigation("Animes");
                });

            modelBuilder.Entity("CBF_Testing.Domain.Entities.User", b =>
                {
                    b.Navigation("RatingFeedbacks");

                    b.Navigation("ViewFeedbacks");
                });
#pragma warning restore 612, 618
        }
    }
}
