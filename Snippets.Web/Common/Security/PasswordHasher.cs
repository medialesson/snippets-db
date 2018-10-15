using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Snippets.Web.Common.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly AppSettings _settings;
        readonly HMACSHA512 _algorithm;

        /// <summary>
        /// Initializes a PasswordHasher
        /// </summary>
        /// <param name="settings">Mapped version of "appsettings.json"</param>
        public PasswordHasher(AppSettings settings)
        {
            _settings = settings;
            _algorithm = new HMACSHA512(Encoding.UTF8.GetBytes(_settings.Secret));
        }

        public byte[] Hash(string password, byte[] salt)
        {
            var bytes = Encoding.UTF8.GetBytes(password);

            // Merge the plain text password with the salt
            var allBytes = new byte[bytes.Length + salt.Length];
            Buffer.BlockCopy(bytes, 0, allBytes, 0, bytes.Length);
            Buffer.BlockCopy(salt, 0, allBytes, bytes.Length, salt.Length);

            // Generate a sha512 hash for verification
            return _algorithm.ComputeHash(allBytes);
        }
    }
}