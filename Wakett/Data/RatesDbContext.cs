using System;
using Microsoft.EntityFrameworkCore;
using RatesService.Model;

namespace RatesService.Data
{
    public class RatesDbContext : DbContext
    {
        public DbSet<Rate> Rates { get; set; }

        public RatesDbContext(DbContextOptions<RatesDbContext> options) : base(options) { }
    }
}

