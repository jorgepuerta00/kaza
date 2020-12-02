using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SeekQ.UserAssets.Api.Data
{
    public class UserAssetsDbContextFactory : IDesignTimeDbContextFactory<UserAssetsDbContext>
    {
        public UserAssetsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserAssetsDbContext>();
            optionsBuilder.UseSqlServer("Server=127.0.0.1,1433;Database=SeekQ.Geo;User Id=sa;Password=Password123");

            return new UserAssetsDbContext(optionsBuilder.Options);
        }
    }
}