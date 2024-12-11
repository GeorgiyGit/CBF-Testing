using CBF_Testing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Domain.DTOs.User
{
    public class ViewFeetbackResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string AnimeName { get; set; }
        public int AnimeId { get; set; }
    }
}
