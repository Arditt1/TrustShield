using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db_tsh.Models
{
    public class Pol_travel
    {
        public int TrId { get; set; }
        public int PolId { get; set; }

        // Navigation property for the related policy
        public Policy PolicyObject { get; set; }
    }
}
