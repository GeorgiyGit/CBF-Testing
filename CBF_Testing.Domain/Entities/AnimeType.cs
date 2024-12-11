using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Domain.Entities
{
    public class AnimeType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Anime> Animes { get; set; } = new HashSet<Anime>();
    }
}
