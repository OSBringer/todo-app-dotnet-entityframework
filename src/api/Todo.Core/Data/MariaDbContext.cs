using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Todo.Core.Data
{
    public partial class AzureSqlDbContext : DbContext
    {
        public AzureSqlDbContext(DbContextOptions<AzureSqlDbContext> options)
            : base(options)
        { }

        public virtual DbSet<Models.Task>? Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Models.Task>(b =>
            {
                b.ToTable("Tasks"); // Map the entity to the "Tasks" table
                b.HasKey(t => t.Id); // Define the primary key
                b.Property(t => t.Id).IsRequired(); // Mark Id as required
                b.Property(t => t.Description)
                    .HasMaxLength(500) // Set max length for Description
                    .IsRequired(false); // Allow null values for Description
                b.Property(t => t.Completed).IsRequired(); // Mark Completed as required
            });
        }

    }
}
