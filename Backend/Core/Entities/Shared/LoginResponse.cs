using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale.Core.Entities.Shared
{
    /// <summary>
    /// Message for returning login responses to the GUI.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Returns the user id for the authenticated user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Returns any errors during the authentication process.
        /// </summary>
        public string Error { get; set; }
    }
}
