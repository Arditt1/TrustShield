using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db_tsh.Models
{
    public class Pol_auto
    {
        public int A_id { get; set; } // Primary Key

        public int Pol_id { get; set; } // Foreign Key referencing project.Policy(p_id)

        // Navigation property for the related policy
        public Policy PolicyObject { get; set; }
    }
}
