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
    public class UserBuilder : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(e => e.ViewFeedbacks)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.UserId);

            builder.HasMany(e => e.RatingFeedbacks)
                   .WithOne(e => e.User)
                   .HasForeignKey(e => e.UserId);
        }
    }
}
