using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Domain.Entities
{
    public class RatingFeedback
    {
        public int Id { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public Anime Anime { get; set; }
        public int AnimeId { get; set; }

        public int Rating { get; set; }
    }
}
