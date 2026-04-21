using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Data.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<CollectedData> CollectedData => Set<CollectedData>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CollectedData>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Source)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Title)
                    .HasMaxLength(300)
                    .IsRequired();

                entity.Property(x => x.Description)
                    .HasMaxLength(2000)
                    .IsRequired();

                entity.Property(x => x.Url)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(x => x.CollectedAt)
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
