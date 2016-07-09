using System;
using System.Collections.Generic;

namespace Hale.Core.Entities.Security
{
    /// <summary>
    /// Corresponds to the database table Security.User
    /// </summary>
    public class User
    {
        /// <summary>
        /// Corresponds to the table column User.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Corresponds to the table column User.UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Corresponds to the table column User.Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Corresponds to the table column User.Password.
        /// Only used to hold a hashed and salted representation of the password.
        /// </summary>
        internal string Password { get; set; }

        /// <summary>
        /// Corresponds to the table column User.Salt
        /// </summary>
        internal string Salt { get; set; }

        /// <summary>
        /// Corresponds to the table column User.Created
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Corresponds to the table column User.CreatedBy (FK Security.Users.Id)
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Corresponds to the table column User.Changed
        /// </summary>
        public DateTime Changed { get; set; }

        /// <summary>
        /// Corresponds to the table column User.ChangedBy (FK Security.Users.Id)
        /// </summary>
        public int ChangedBy { get; set; }

        /// <summary>
        /// Aggregation of available records in the Security.UserDetails table.
        /// Users [1..*] UserDetails
        /// </summary>
        public List<UserDetail> UserDetails { get; set; }


    }
}
