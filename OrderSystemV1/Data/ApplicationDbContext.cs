using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using OrderSystemV1.Models;
using OrderSystemV1.Models.Entities;

namespace OrderSystemV1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Clients> Clients { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<BirthdayOrderTotalResult> BirthdayOrderTotalResults { get; set; }
        public DbSet<AverageCheckByHourResult> AverageCheckByHourResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BirthdayOrderTotalResult>().HasNoKey();
            modelBuilder.Entity<AverageCheckByHourResult>().HasNoKey();

            var mapping = new Dictionary<OrderStatus, string>
            {
                [OrderStatus.NotProcessed] = "Не обработан",
                [OrderStatus.Cancelled] = "Отменён",
                [OrderStatus.Completed] = "Выполнен"
            };

            modelBuilder.Entity<Orders>()
                .Property(o => o.Status)
                .HasConversion(
                    v => mapping[v],
                    v => mapping.FirstOrDefault(x => x.Value == v).Key
                );
        }
    }
}
