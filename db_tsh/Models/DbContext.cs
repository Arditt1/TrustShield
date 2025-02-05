using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace db_tsh.Models
{
    public class TrustShield : DbContext
    {
        public TrustShield(DbContextOptions<TrustShield> options) : base(options)
        {
        }

        // DbSet properties for your entities
        public DbSet<Customer> Customer { get; set; }

    }
}
