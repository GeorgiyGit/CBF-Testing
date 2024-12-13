using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CBF_Testing.Domain.DTOs.Analysis
{
    public class DTResponse
    {
        public double F1Score { get; set; }
        public double F110Score { get; set; }
        public double RSME { get; set; }
        public double RSME10 { get; set; }
        public TimeSpan BuildTime { get; set; }
        public TimeSpan PredictTime { get; set; }
    }
}
