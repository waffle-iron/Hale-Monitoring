using Hale.Core.Entities.Security;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using Hale.Core.Handlers;

namespace Hale.Core.Contexts
{
    internal class UserDetails : SqlHandler
    {
        public void Create (User user, UserDetail detail)
        {
            ConnectToDatabase();
            connection.Execute("exec uspCreateUserDetail @id @key @value",
                new
                {
                    id = user.Id,
                    key = detail.Key,
                    value = detail.Value
                }
            );
        }

        public void Update (User user, UserDetail detail)
        {
            ConnectToDatabase();
            connection.Execute("exec uspUpdateUserDetail @id @key @value",
                new
                {
                    id = user.Id,
                    key = detail.Key,
                    value = detail.Value
                }
            );
        }

        public void Delete (User user, UserDetail detail)
        {
            ConnectToDatabase();
            connection.Execute("exec uspDeleteUserDetail @id @key",
                new
                {
                    id = user.Id,
                    key = detail.Key
                }
            );
        }
        public UserDetail Get (User user, UserDetail detail)
        {
            ConnectToDatabase();
            return connection.Query<UserDetail>("exec uspGetUserDetail @id @key",
                new
                {
                    id = user.Id,
                    key = detail.Key
                })
                .First();
        }

        public List<UserDetail> List (User user)
        {
            ConnectToDatabase();
            return connection.Query<UserDetail>("exec uspListUserDetails @id",
                new
                {
                    id = user.Id
                })
                .ToList();
        }
    }
}
