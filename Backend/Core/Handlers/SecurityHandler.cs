using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Hale.Core.Entities.Security;
using NLog;
using Hale.Core.Contexts;

namespace Hale.Core.Handlers
{

    internal class SecurityHandler
    {
        internal readonly Users users = new Users();
        readonly Logger _log;

        
        internal SecurityHandler() {
            _log = LogManager.GetCurrentClassLogger();
        }

        private string CreateSalt()
        {
            return Guid.NewGuid().ToString();
        }

        private bool CompareHashes(string userHash, string dbHash)
        {
            return (userHash == dbHash);
        }


        internal bool Authenticate(string username, string password)
        {
            User user = users.Get(new User() { UserName = username });
            
            if (user == null)
            {
                return false;
            }
            else
            {
                string inHash = HashPassword(password, Encoding.ASCII.GetBytes(user.Salt));
                string dbHash = user.Password;


                return CompareHashes(inHash, dbHash);
            }
        }
        
        internal string HashPassword(string password, byte[] salt)
        {
            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            byte[] hashInput = passwordBytes.Concat(salt).ToArray();

            using (SHA512 cryptographyService = new SHA512Managed())
            {
                byte[] hash = cryptographyService.ComputeHash(hashInput);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
