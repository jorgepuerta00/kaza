using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeekQ.UserAssets.Api.Models;

namespace SeekQ.UserAssets.Api.Data.EntityConfigurations
{
    public class AssetTypeModelConfiguration : IEntityTypeConfiguration<AssetTypeModel>
    {
        public void Configure(EntityTypeBuilder<AssetTypeModel> configuration)
        {
            configuration.HasKey(g => g.Id);

            configuration.Property(g => g.Id)
                .ValueGeneratedNever()
                .IsRequired();

            configuration.Property(g => g.Name)
                .HasMaxLength(10)
                .IsRequired();
        }
    }
}