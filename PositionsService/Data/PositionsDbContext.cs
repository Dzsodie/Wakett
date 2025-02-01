using System;
using Microsoft.EntityFrameworkCore;
using PositionsService.Model;

namespace PositionsService.Data
{
    public class PositionsDbContext : DbContext
    {
        public DbSet<Position> Positions { get; set; }

        public PositionsDbContext(DbContextOptions<PositionsDbContext> options) : base(options) { }
    }
}

