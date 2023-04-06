using Microsoft.EntityFrameworkCore;
using System;

namespace Hotel.Core
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //var now = DateTime.Now;
            //modelBuilder.Entity<User>().HasData(new User { Id = 1, CreatedTime = now, Email = "admin@qq.com", Name = "admin", Password = "1", UserType = UserType.Employee },
            //    new User { Id = 2, CreatedTime = now, Email = "user1@qq.com", Name = "user1", Password = "1" });
            //modelBuilder.Entity<Table>().HasData(new Table { Id = 1, Name = "Table1", Size = 2, CreatedTime = now },
            //    new Table { Id = 2, Name = "Table1", Size = 4, CreatedTime = now },
            //    new Table { Id = 3, Name = "Table1", Size = 6, CreatedTime = now });
            base.OnModelCreating(modelBuilder);
        }
    }
}
