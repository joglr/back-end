﻿using Microsoft.EntityFrameworkCore;

namespace PolloPollo.Entities
{
    public class PolloPolloContext : DbContext, IPolloPolloContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Producer> Producers { get; set; }
        public DbSet<Receiver> Receivers { get; set; }

        public PolloPolloContext(DbContextOptions<PolloPolloContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserRole>()
                .Property(e => e.UserRoleEnum)
                .HasConversion<int>();
            modelBuilder
                .Entity<UserRole>()
                .HasKey(e => new { e.UserId, e.UserRoleEnum });
            modelBuilder
                .Entity<User>()
                .HasAlternateKey(c => c.Email)
                .HasName("AlternateKey_UserEmail");

        }
    }
}
