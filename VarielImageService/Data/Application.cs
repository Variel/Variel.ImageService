using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Variel.ImageService.Extensions;
using Variel.ImageService.Helpers;

namespace Variel.ImageService.Data
{
    public class Application : IEntityTypeConfiguration<Application>
    {
        public string Id { get; set; } = IdentityGenerator.GenerateLowerCase();

        public string Name { get; set; }
        public string Note { get; set; }

        public string Secret { get; set; } = IdentityGenerator.Generate(32);

        public string[] AllowedOrigins { get; set; }

        public bool UseDirectUrlRedirection { get; set; }
        public string StorageProviderName { get; set; }
        public string StorageProviderSettingsJson { get; set; }

        public Dictionary<string, ProcessingPreset> ProcessingPresets { get; set; } = new Dictionary<string, ProcessingPreset>();


        public void Configure(EntityTypeBuilder<Application> builder)
        {
            builder.Property(app => app.AllowedOrigins)
                   .HasJsonConversion();

            builder.Property(app => app.ProcessingPresets)
                   .HasJsonConversion();
        }
    }

    public static class StorageProviderNames
    {
        public const string AzureBlobStorage = "azure-blob-storage";
        public const string AmazonS3 = "amazon-s3";
    }
}
