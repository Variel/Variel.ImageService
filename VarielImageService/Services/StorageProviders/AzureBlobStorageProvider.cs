using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Variel.ImageService.Data;

namespace Variel.ImageService.Services.StorageProviders
{
    [StorageProvider(StorageProviderNames.AzureBlobStorage)]
    public class AzureBlobStorageProvider : IStorageProvider
    {
        private bool _initialized = false;
        private BlobServiceClient _serviceClient;

        public void Initialize(string settingsJson)
        {
            var settings = JsonConvert.DeserializeObject<AzureBlobStorageSettings>(settingsJson);
            _serviceClient = new BlobServiceClient(settings.ConnectionString);
            _initialized = true;
        }

        public async Task<string> UploadAsync(string containerName, string fileName, Stream stream)
        {
            if (!_initialized)
                throw new InvalidOperationException("Storage provider not initialized");

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);

            var savedFileName = $"{fileName}-{Guid.NewGuid()}{extension}";

            var containerClient = _serviceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(stream);

            return blobClient.Uri.ToString();
        }

        public class AzureBlobStorageSettings
        {
            public string ConnectionString { get; set; }
        }
    }
}
