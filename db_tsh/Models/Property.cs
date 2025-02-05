using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db_tsh.Models
{
    public class Property
    {
        public int P_id { get; set; } // Primary Key

        public int Policy { get; set; } // Foreign Key referencing project.Pol_auto(a_id)

        public string Address { get; set; }
        public string Floor { get; set; }
        public int YearBuild { get; set; }
        public string Security { get; set; }

        // Navigation property for the related policy
        public Pol_auto PolicyObject { get; set; }
    }
}
