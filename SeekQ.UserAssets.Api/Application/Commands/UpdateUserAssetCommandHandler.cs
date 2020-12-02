using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.Common.Exceptions;
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
    public class UpdateUserAssetCommandHandler
    {
        public class Command : IRequest<UserAssetModel>
        {
            public Guid Id { get; set; }
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
                    Guid Id = request.Id;
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

                    var existingUserAsset = await _userAssetsDbContext.UserAssets.FindAsync(Id);

                    if (existingUserAsset == null)
                    {
                        throw new AppException($"The UserId {UserId} doesn't have any asset.");
                    }

                    existingUserAsset.Url = blobUri.ToString();
                    existingUserAsset.Order = request.Order;
                    existingUserAsset.AssetTypeId = AssetTypeModel.Image.Id;

                    _userAssetsDbContext.UserAssets.Update(existingUserAsset);
                    await _userAssetsDbContext.SaveChangesAsync();

                    return existingUserAsset;
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
