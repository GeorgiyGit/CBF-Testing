using CBF_Testing.Domain.Entities;
using CBF_Testing.Infrastructure.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Infrastructure
{
    public class CBFTestingDbContext : DbContext
    {
        public DbSet<Anime> Animes { get; set; }
        public DbSet<AnimeType> AnimeTypes { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<RatingFeedback> RatingFeedbacks { get; set; }
        public DbSet<ViewFeedback> ViewFeedbacks { get; set; }
        public DbSet<User> Users { get; set; }

        public CBFTestingDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnimeBuilder).Assembly);
        }
    }
}
