using System;
using Microsoft.AspNetCore.Http;

namespace SeekQ.UserAssets.Api.Application.ViewModel
{
    public class UserAssetViewModel
    {
        public Guid UserId { get; set; }
        public IFormFile File { get; set; }
        public int Order { get; set; }
    }
}
