﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VkStorageSQlite.Infrastructure.Data;

#nullable disable

namespace VkStorage.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("VkStorage.Domain.Entities.FileChunk", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileUrl")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<int>("VkFileId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("VkFileId");

                    b.ToTable("FileChunks");
                });

            modelBuilder.Entity("VkStorage.Domain.Entities.VkFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessLevel")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("Created")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Deleted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("Guid")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("SizeInBytes")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("VkFiles");
                });

            modelBuilder.Entity("VkStorage.Domain.Entities.FileChunk", b =>
                {
                    b.HasOne("VkStorage.Domain.Entities.VkFile", "VkFile")
                        .WithMany("Chunks")
                        .HasForeignKey("VkFileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VkFile");
                });

            modelBuilder.Entity("VkStorage.Domain.Entities.VkFile", b =>
                {
                    b.Navigation("Chunks");
                });
#pragma warning restore 612, 618
        }
    }
}
