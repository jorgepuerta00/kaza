using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SeekQ.UserAssets.Api.Data;
using SeekQ.UserAssets.Api.Models;
using Serilog;

namespace SeekQ.UserAssets.Api
{
    public class SeedData
    {
        // Statics IDs for users
        public static readonly Guid ID_USER_MOCK1 = new Guid("545DE66E-19AC-47D2-57F6-08D8715337D7");

        // Statics IDs for tupla
        public static readonly Guid ID_ROW_1 = new Guid("545DE66E-19AC-47D2-57F6-08D8715337D7");

        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddDbContext<UserAssetsDbContext>(options =>
               options.UseSqlServer(connectionString));

            using (var serviceProvider = services.BuildServiceProvider())
            {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<UserAssetsDbContext>();

                    context.Database.EnsureCreated();

                    var image = context.AssetTypes.Find(1);
                    if (image == null)
                    {
                        context.AssetTypes.Add(new AssetTypeModel(1, "Image"));
                        context.SaveChanges();
                        Log.Debug("ImageType was created");
                    }
                    else
                    {
                        Log.Debug("ImageType already exists");
                    }

                    var video = context.AssetTypes.Find(2);
                    if (video == null)
                    {
                        context.AssetTypes.Add(new AssetTypeModel(2, "Video"));
                        context.SaveChanges();
                        Log.Debug("VideoType was created");
                    }
                    else
                    {
                        Log.Debug("VideoType already exists");
                    }
                }
            }
        }
    }
}