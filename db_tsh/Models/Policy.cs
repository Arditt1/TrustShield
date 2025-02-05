using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db_tsh.Models
{
    public class Policy
    {
        public int P_id { get; set; }
        public DateTime Sdate { get; set; }
        public DateTime Edate { get; set; }
        public int Package { get; set; }
        public Package PackageObject { get; set; }

        // Additional properties for fetching data
        public string PolicyType { get; set; }
        public string CustomerName { get; set; }
        public string PackageTitle { get; set; }
        public decimal PackageTotal { get; set; }
    }
}
