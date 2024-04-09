using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Practice.Model
{
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Clients> Clients { get; set; }
        public virtual DbSet<Services> Services { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Services>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Services>()
                .HasMany(e => e.Clients)
                .WithOptional(e => e.Services)
                .HasForeignKey(e => e.Course);
        }
    }
}
