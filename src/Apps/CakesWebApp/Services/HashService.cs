using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CakesWebApp.Services
{
    public class HashService : IHashService
    {
        public string Hash(string stringToHash)
        {
            stringToHash = stringToHash + "myAppSalt1316387123718#";
            using (var sha256 = SHA256.Create())
            {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
                // Get the hashed string.  
                var hash = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                return hash;
            }
        }
    }

    public interface IHashService
    {
        string Hash(string stringToHash);
    }
}
