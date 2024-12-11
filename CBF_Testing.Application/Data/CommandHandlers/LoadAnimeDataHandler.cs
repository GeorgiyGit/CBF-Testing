using CBF_Testing.Application.Data.Commands;
using CBF_Testing.Domain.Entities;
using CBF_Testing.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CBF_Testing.Application.Data.CommandHandlers
{
    public class LoadAnimeDataHandler(CBFTestingDbContext dbContext) : IRequestHandler<LoadAnimeData, bool>
    {
        private readonly CBFTestingDbContext _dbContext = dbContext;
        public async Task<bool> Handle(LoadAnimeData request, CancellationToken cancellationToken)
        {
            var cPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var fullPath = Path.Combine(cPath, "anime\\anime_info.dat");

            var lines = File.ReadLines(fullPath).ToList();
            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i];

                var parts = line.Split('	');

                int animeId = int.Parse(parts[0]);
                string name = parts[1];
                string genresStr = parts[2];
                string typeName = parts[3];
                int episodes = int.Parse(parts[4]);
                double rating = double.Parse(parts[5]);
                int members = int.Parse(parts[6]);

                if ((await _dbContext.Animes.FindAsync(animeId, cancellationToken)) == null)
                {
                    Anime newAnime = new Anime
                    {
                        Id = animeId,
                        Name = name,
                        EpisodesCount = episodes,
                        Rating = rating,
                        Members = members
                    };
                    _dbContext.Animes.Add(newAnime);

                    var genres = genresStr.Split(",").Select(g => g.Trim()).ToList();
                    foreach(var genreName in genres)
                    {
                        var genre = await _dbContext.Genres.Where(e => e.Name == genreName).FirstOrDefaultAsync(cancellationToken);
                        if (genre == null)
                        {
                            Genre newGenre = new()
                            {
                                Name = genreName
                            };
                            _dbContext.Genres.Add(newGenre);
                            genre = newGenre;
                        }
                        newAnime.Genres.Add(genre);
                    }

                    var type = await _dbContext.AnimeTypes.Where(e => e.Name == typeName).FirstOrDefaultAsync(cancellationToken);
                    if (type == null)
                    {
                        AnimeType newType = new()
                        {
                            Name = typeName
                        };
                        _dbContext.AnimeTypes.Add(newType);
                        type = newType;
                    }
                    newAnime.Type = type;
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
