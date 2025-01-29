using Microsoft.EntityFrameworkCore;
using FoodSalesAPI.Models;

namespace FoodSalesAPI.Data
{
    public class FoodSalesDbContext : DbContext
    {
        public FoodSalesDbContext(DbContextOptions<FoodSalesDbContext> options) : base(options) { }

        public DbSet<FoodSale> FoodSales { get; set; }
    }
}
