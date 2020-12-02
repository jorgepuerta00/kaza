using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using SeekQ.UserAssets.Api.Configuration;
using SeekQ.UserAssets.Api.Data;
using SeekQ.UserAssets.Api.Models;

namespace SeekQ.UserAssets.Api.Application.Commands
{
    public class CreateUserAssetCommandHandler
    {
        public class Command : IRequest<UserAssetModel>
        {
            public Guid UserId { get; set; }
            public IFormFile File { get; set; }
            public int Order { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.File).NotNull()
                    .WithMessage("File required");
            }
        }

        public static bool IsImage(IFormFile file)
        {
            if (file.ContentType.Contains("image"))
            {
                return true;
            }

            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg" };

            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        public class Handler : IRequestHandler<Command, UserAssetModel>
        {
            private UserAssetsDbContext _userAssetsDbContext;
            private AzureStorageConfig _storageConfig;
            private ServiceSettings _serviceSettings;

            public Handler(
                UserAssetsDbContext userAssetsDbContext,
                IOptions<AzureStorageConfig> storageConfig,
                IOptions<ServiceSettings> serviceSettings)
            {
                _userAssetsDbContext = userAssetsDbContext;
                _storageConfig = storageConfig.Value;
                _serviceSettings = serviceSettings.Value;
            }

            public async Task<UserAssetModel> Handle(
                Command request,
                CancellationToken cancellationToken
            )
            {
                try
                {
                    Guid Id = Guid.NewGuid();
                    Guid UserId = request.UserId;
                    IFormFile FormFile = request.File;
                    int Order = request.Order;
                    string Extension = Path.GetExtension(FormFile.FileName);

                    string AccountName = _storageConfig.AccountName;
                    string ImageContainer = _storageConfig.ThumbnailContainer;
                    string AccountKey = _storageConfig.AccountKey;

                    // Create a URI to the blob
                    Uri blobUri = new Uri($"https://{AccountName}.blob.core.windows.net/{ImageContainer}/{Id}{Extension}");

                    // Create StorageSharedKeyCredentials object by reading
                    // the values from the configuration (appsettings.json)
                    StorageSharedKeyCredential storageCredentials = new StorageSharedKeyCredential(AccountName, AccountKey);

                    // Create the blob client.
                    BlobClient blobClient = new BlobClient(blobUri, storageCredentials);
                    
                    using (Stream stream = FormFile.OpenReadStream())
                    {
                        // Upload the file
                        var result = await blobClient.UploadAsync(
                            stream,
                            new BlobHttpHeaders
                            {
                                ContentType = GetContentType(FormFile.FileName)
                            },
                            conditions: null);
                    }

                    UserAssetModel userAsset = new UserAssetModel();
                    userAsset.Id = Id;
                    userAsset.UserId = UserId;
                    userAsset.Url = blobUri.ToString();
                    userAsset.Order = request.Order;
                    userAsset.AssetTypeId = AssetTypeModel.Image.Id;

                    _userAssetsDbContext.UserAssets.Add(userAsset);
                    await _userAssetsDbContext.SaveChangesAsync();

                    return userAsset;
                }
                catch (Exception)
                {
                    throw;
                }
            }

            private string GetContentType(string fileName)
            {
                var provider = new FileExtensionContentTypeProvider();
                string contentType;
                if (!provider.TryGetContentType(fileName, out contentType))
                {
                    contentType = "application/octet-stream";
                }
                return contentType;
            }
        }
    }
}
