using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db_tsh.Models
{
    public class Covers
    {
        
        public int cov_id { get; set; }

        public string cov_amount { get; set; }

        public string package_code { get; set; }
        public string PackageName { get; set; }
        public Package package { get; set; }

        public string cov_type { get; set; }

    }
}
