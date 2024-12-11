using CBF_Testing.Application.Data.Commands;
using CBF_Testing.Domain.Entities;
using CBF_Testing.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Application.Data.CommandHandlers
{
    public class LoadRatingFeetbackDataHandler(CBFTestingDbContext dbContext) : IRequestHandler<LoadRatingFeetbackData, bool>
    {
        private readonly CBFTestingDbContext _dbContext = dbContext;

        public async Task<bool> Handle(LoadRatingFeetbackData request, CancellationToken cancellationToken)
        {
            var cPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fullPath = Path.Combine(cPath, "anime\\anime_ratings.dat");

            var lines = File.ReadLines(fullPath).ToList();
            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i];

                var parts = line.Split('	');

                int userId = int.Parse(parts[0]);
                int animeId = int.Parse(parts[1]);
                int rating = int.Parse(parts[2]);

                if ((await _dbContext.RatingFeedbacks.Where(e => e.UserId == userId && e.AnimeId == animeId).FirstOrDefaultAsync(cancellationToken) == null))
                {
                    RatingFeedback feedBack = new()
                    {
                        UserId = userId,
                        AnimeId = animeId,
                        Rating = rating
                    };
                    _dbContext.RatingFeedbacks.Add(feedBack);
                    
                    var user = await _dbContext.Users.FindAsync(userId, cancellationToken);
                    if(user == null)
                    {
                        User newUser = new User
                        {
                            Id = userId
                        };
                        _dbContext.Users.Add(newUser);
                        user = newUser;
                    }
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
