using Microsoft.EntityFrameworkCore;
using BookStore.Models;


namespace BookStore.Data
{
    public class BookstoreDbContext: DbContext
    {
        public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options): base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuring relationships 
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
