using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Snippets.Web.Common.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly IConfiguration _configuration;
        readonly HMACSHA512 _algorithm; 

        public PasswordHasher(IConfiguration configuration)
        {
            _configuration = configuration;
            _algorithm = new HMACSHA512(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Secret")));
        }

        public byte[] Hash(string password, byte[] salt)
        {
            var bytes = Encoding.UTF8.GetBytes(password);

            var allBytes = new byte[bytes.Length + salt.Length];
            Buffer.BlockCopy(bytes, 0, allBytes, 0, bytes.Length);
            Buffer.BlockCopy(salt, 0, allBytes, bytes.Length, salt.Length);

            return _algorithm.ComputeHash(allBytes);
        }
    }
}