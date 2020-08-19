using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Variel.ImageService.Helpers;

namespace Variel.ImageService.Data
{
    public class MasterImage
    {
        public string Id { get; set; } = IdentityGenerator.Generate();

        public string FileUrl { get; set; }

        public string FileName { get; set; }
        public int FileSize { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public string Note { get; set; }

        public string ApplicationId { get; set; }
        public Application Application { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
