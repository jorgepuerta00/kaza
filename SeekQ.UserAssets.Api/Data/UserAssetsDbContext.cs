using Microsoft.EntityFrameworkCore;
using SeekQ.UserAssets.Api.Data.EntityConfigurations;
using SeekQ.UserAssets.Api.Models;

namespace SeekQ.UserAssets.Api.Data
{
    public class UserAssetsDbContext : DbContext
    {
        public UserAssetsDbContext(DbContextOptions<UserAssetsDbContext> options)
            : base(options)
        {
        }

        public DbSet<AssetTypeModel> AssetTypes { get; set; }
        public DbSet<UserAssetModel> UserAssets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserAssetModelConfiguration());
            builder.ApplyConfiguration(new AssetTypeModelConfiguration());
        }
    }
}