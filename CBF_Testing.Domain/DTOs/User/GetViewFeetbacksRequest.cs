using CBF_Testing.Domain.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBF_Testing.Domain.DTOs.User
{
    public class GetViewFeetbacksRequest
    {
        public int UserId { get; set; }
        public PageParameters PageParameters { get; set; }
    }
}
