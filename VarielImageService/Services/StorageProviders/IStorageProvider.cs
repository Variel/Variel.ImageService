using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Variel.ImageService.Services.StorageProviders
{
    public interface IStorageProvider
    {
        public void Initialize(string settingsJson);
        public Task<string> UploadAsync(string containerName, string fileName, Stream stream);
    }
}
