using CBF_Testing.Domain.DTOs.Genres;
using CBF_Testing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Domain.DTOs.Animes
{
    public class AnimeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int EpisodesCount { get; set; }
        public double Rating { get; set; }
        public int Members { get; set; }

        public ICollection<GenreResponse> Genres { get; set; } = new List<GenreResponse>();

        public string TypeName { get; set; }
        public int TypeId { get; set; }
    }
}
