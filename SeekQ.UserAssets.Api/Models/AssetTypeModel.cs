using App.Common.SeedWork;

namespace SeekQ.UserAssets.Api.Models
{
    public class AssetTypeModel : Enumeration
    {
        public static AssetTypeModel Image = new AssetTypeModel(1, "Image");
        public static AssetTypeModel Video = new AssetTypeModel(2, "Video");

        public AssetTypeModel(int id, string name)
                   : base(id, name)
        {
        }
    }
}