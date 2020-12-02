using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SeekQ.UserAssets.Api.Models;

namespace SeekQ.UserAssets.Api.Data.EntityConfigurations
{
    public class UserAssetModelConfiguration : IEntityTypeConfiguration<UserAssetModel>
    {
        public void Configure(EntityTypeBuilder<UserAssetModel> configuration)
        {
            configuration.HasKey(g => g.Id);

            configuration.Property(g => g.Id)
                .ValueGeneratedNever()
                .IsRequired();

            configuration.Property(g => g.UserId)
                .ValueGeneratedNever()
                .IsRequired();

            configuration.Property(g => g.Url)
                .ValueGeneratedNever()
                .IsRequired();

            configuration.Property(g => g.Order)
                .ValueGeneratedNever()
                .IsRequired();

            configuration.Property<int?>("AssetTypeId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("AssetTypeId")
                .IsRequired();

            configuration.HasOne(c => c.AssetType)
                .WithMany()
                .HasForeignKey("AssetTypeId");
        }
    }
}