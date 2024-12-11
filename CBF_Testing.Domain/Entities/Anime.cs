using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Domain.Entities
{
    public class Anime
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public int EpisodesCount { get; set; }
        public double Rating { get; set; }
        public int Members { get; set; }

        public ICollection<Genre> Genres { get; set; } = new HashSet<Genre>();
        
        public AnimeType Type { get; set; }
        public int TypeId { get; set; }
    
        public ICollection<ViewFeedback> ViewFeedbacks { get; set; } = new HashSet<ViewFeedback>();
        public ICollection<RatingFeedback> RatingFeedbacks { get; set; } = new HashSet<RatingFeedback>();
    }
}
