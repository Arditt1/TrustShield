using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db_tsh.Models
{
    public class Vehicle
    {
        public int V_id { get; set; } // Primary Key

        public int Policy { get; set; } // Foreign Key referencing project.Pol_auto(a_id)

        public string Type { get; set; } // Not null

        public string Marka { get; set; }

        public string Model { get; set; }

        public string License_Plate { get; set; } // Not null

        // Navigation property for the related policy
        public Pol_auto PolicyObject { get; set; }
    }
}
