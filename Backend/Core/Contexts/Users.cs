using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Hale.Core.Entities.Security;
using System.Data.SqlClient;
using Hale.Core.Handlers;

namespace Hale.Core.Contexts
{
    internal class Users : SqlHandler
    {
        private readonly string encryption = "SHA-512";
        public void Create(User user)
        {
            ConnectToDatabase();
            try
            {
                connection.Execute("exec uspAddUser @name, @email, @password, @salt, @encryption, @createdby",
                    new
                    {
                        name = user.UserName,
                        email = user.Email,
                        password = user.Password,
                        salt = user.Salt,
                        encryption,
                        createdby = user.CreatedBy
                    }
                );
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public void Update(User user)
        {
            ConnectToDatabase();
            try
            {
                connection.Execute("exec uspChangeUser @id, @name, @email, @password, @salt",
                    new
                    {
                        id = user.Id,
                        name = user.UserName,
                        email = user.Email,
                        password = user.Password,
                        salt = user.Salt
                    }
                );
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public void Delete(User user)
        {
            ConnectToDatabase();
            try
            {
                connection.Execute("exec uspDeleteUser @id", new { id = user.Id });
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public User Get(User user)
        {
            ConnectToDatabase();
            try
            {
                if (user.Id != 0)
                    return GetById(user.Id);
                else if (user.UserName != null)
                    return GetByName(user.UserName);
                else
                    throw new ArgumentException("Cannot get user from database without Id or Username.");
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        private User GetByName(string username)
        {
            return connection.Query<User>("exec uspGetUserByName @username", new { username }).First();
        }

        private User GetById(int id)
        {
            return connection.Query<User>("exec uspGetUserById @id", new { id }).First();
        }

        public List<User> List()
        {
            ConnectToDatabase();
            try
            {
                return connection.Query<User>("exec uspListUsers").ToList();
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
