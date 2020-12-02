namespace SeekQ.UserAssets.Api.Models
{
    using System;
    using App.Common.Repository;

    public class UserAssetModel : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public int? AssetTypeId { get; set; }
        public AssetTypeModel AssetType { get; set; }
    }
}