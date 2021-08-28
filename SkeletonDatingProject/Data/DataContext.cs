using Microsoft.EntityFrameworkCore;
using SkeletonDatingProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        { }
        public DbSet<AppUser> Users { get; set; }
    }
}
