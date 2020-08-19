using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Variel.ImageService.Services.StorageProviders
{
    public interface IStorageProvider
    {
        void Initialize(string settingsJson);
        Task<string> UploadAsync(string containerName, string fileName, Stream stream);
    }
}
