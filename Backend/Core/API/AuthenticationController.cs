using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using Hale_Core.Entities;
using Hale_Core.Entities.Security;
using Hale_Core.Handlers;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using NLog;
using Microsoft.Owin;
using Hale_Core.Entities.Shared;
using Hale_Core.Contexts;

namespace Hale_Core.API
{
    /// <summary>
    /// API for handling logins, logouts and status checks.
    /// </summary>
    [RoutePrefix("api/v1/authentication")]
    public class AuthenticationController : ApiController
    {

        readonly SecurityHandler _security;
        readonly Logger _log;
        readonly Users _users;

        
        internal AuthenticationController()
        {
            _log = LogManager.GetCurrentClassLogger();
            _security = new SecurityHandler();
            _users = new Users();
        }
        
#if DEBUG
        /// <summary>
        /// Creates claim and session if the authentication succeeds. (Debug only)
        /// </summary>
        /// <param name="username">Username for the login attempt</param>
        /// <param name="password">Password for the login attempt</param>
        /// <param name="persistent">Whether or not the session should persist on exit.</param>
        /// <returns>A custom LoginResponse that will be stored in the local storage for the Hale-GUI ember application.</returns>
        [Route("login")]
        [ResponseType(typeof(LoginResponse))]
        [HttpGet]
        public LoginResponse DevLogin(string username, string password, bool persistent = false)
        {
            return DoLogin(username, password, persistent);
        }
#endif

        private LoginResponse DoLogin(string username, string password, bool persistent = false)
        {

            if (_security.Authenticate(username, password))
            {
                var context = Request.GetOwinContext();
                context.Authentication.SignIn(
                    new AuthenticationProperties() {
                        IsPersistent = persistent,
                    },
                    new ClaimsIdentity(
                        new[] { new Claim(ClaimsIdentity.DefaultNameClaimType, username) },
                        "HaleCoreAuth"
                    )
                );

                User user = _users.Get(new User() { UserName = username});
                return new LoginResponse()
                {
                    UserId = user.Id,
                    Error = ""
                };
            }
            else
            {
                _log.Info("Authorization failed - " + Request.GetOwinContext().Request.RemoteIpAddress + "@'" + username + "'");
                return new LoginResponse()
                {
                    UserId = -1,
                    Error = "Invalid credentials"
                };
            }

        }


        /// <summary>
        /// Creates claim and session if the authentication succeeds. 
        /// </summary>
        /// <param name="auth">A JSON Serialized authentication attempt.</param>
        /// <returns>A custom LoginResponse that will be stored in the local storage for the Hale-GUI ember application.</returns>
        [Route("login")]
        [HttpPost]
        public LoginResponse Login([FromBody]Authentication auth)
        {
            _log.Info(JsonConvert.SerializeObject(auth));
            return DoLogin(auth.Username, auth.Password);
        }


        /// <summary>
        /// Check whether the current session is authenticated or not.
        /// </summary>
        /// <returns>A statuscode and an error message, in case there happens to be one.</returns>
        [Route("status")]
        [Route()]
        [HttpGet]
        public HttpResponseMessage Status()
        {
            IOwinContext context = Request.GetOwinContext();
            bool authenticated = (context.Authentication.User != null);

            return new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    error = "",
                    authenticated = authenticated
                })),
                StatusCode = (authenticated ? HttpStatusCode.OK : HttpStatusCode.Unauthorized)

            };
        }


        /// <summary>
        /// Deletes claim and session if there is any. 
        /// </summary>
        /// <returns>200</returns>
        [Route()]
        [HttpDelete]
        public HttpResponseMessage Logout()
        {
            var context = Request.GetOwinContext();
            context.Authentication.SignOut();
            return new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    error = "",
                    authenticated = false
                })),
                StatusCode = HttpStatusCode.OK

            };
        }


#if DEBUG
        /// <summary>
        /// Deletes claim and session if there is any. (Debug only)
        /// </summary>
        /// <returns></returns>
        [Route("logout")]
        [AcceptVerbs("GET", "POST")]
        public HttpResponseMessage DevLogout()
        {
            return Logout();
        }
#endif
    }
}
