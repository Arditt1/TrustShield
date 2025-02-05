using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db_tsh.Models
{
    public class Customer
    {
        public int C_id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Type { get; set; }
        public int success { get; set; }
    }
}
