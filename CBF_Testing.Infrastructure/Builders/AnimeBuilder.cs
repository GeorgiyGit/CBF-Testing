using CBF_Testing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Infrastructure.Builders
{
    public class AnimeBuilder : IEntityTypeConfiguration<Anime>
    {
        public void Configure(EntityTypeBuilder<Anime> builder)
        {
            builder.HasOne(e => e.Type)
                   .WithMany(e => e.Animes)
                   .HasForeignKey(e => e.TypeId);

            builder.HasMany(e => e.Genres)
                   .WithMany(e => e.Animes);

            builder.HasMany(e => e.ViewFeedbacks)
                   .WithOne(e => e.Anime)
                   .HasForeignKey(e => e.AnimeId);
            
            builder.HasMany(e=>e.RatingFeedbacks)
                   .WithOne(e => e.Anime)
                   .HasForeignKey(e => e.AnimeId);
        }
    }
}
