using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Variel.ImageService.Helpers
{
    public static class IdentityGenerator
    {
        private const string Charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const string CharsetLowerCase = "abcdefghijklmnopqrstuvwxyz0123456789";

        private static readonly Random Random = new Random((int)DateTimeOffset.Now.Ticks);

        public static string Generate(int length = 16)
            => new string(Enumerable.Range(0, length)
                .Select(_ => Charset[Random.Next(0, Charset.Length)]).ToArray());
        public static string GenerateLowerCase(int length = 16)
            => new string(Enumerable.Range(0, length)
                .Select(_ => CharsetLowerCase[Random.Next(0, CharsetLowerCase.Length)]).ToArray());
    }
}
