using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public ICollection<ViewFeedback> ViewFeedbacks { get; set; } = new HashSet<ViewFeedback>();
        public ICollection<RatingFeedback> RatingFeedbacks { get; set; } = new HashSet<RatingFeedback>();
    }
}
