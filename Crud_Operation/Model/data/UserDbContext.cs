using Microsoft.EntityFrameworkCore;

namespace Crud_Operation.Model.data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Remove unique constraints
            // The following lines are removed
            // modelBuilder.Entity<User>()
            //     .HasIndex(u => u.Email)
            //     .IsUnique()
            //     .HasDatabaseName("IX_Users_Email");

            // modelBuilder.Entity<User>()
            //     .HasIndex(u => u.PhoneNumber)
            //     .IsUnique()
            //     .HasDatabaseName("IX_Users_PhoneNumber");

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd(); // Ensures the Id is auto-generated
        }
    }
}
