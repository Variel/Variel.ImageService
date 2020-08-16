using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Variel.ImageService.Services.StorageProviders
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StorageProviderAttribute : Attribute
    {
        public string Name { get; set; }

        public StorageProviderAttribute(string name) => Name = name;
    }
}
