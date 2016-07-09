using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;

namespace Hale_Core.Utils
{
    /// <summary>
    /// Simple way to return a general error string (eg. file not found), title,
    /// an optional descriptive error(file "XX" not found), detail
    /// using a corresponding http result code
    /// Made to comply with the JSON API specification errors
    /// </summary>
    public class StringResult : IHttpActionResult
    {
        

        readonly string _content;
        readonly HttpRequestMessage _request;
        readonly HttpStatusCode _statusCode;


        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public StringResult(HttpStatusCode statusCode, string title, HttpRequestMessage request) : this(statusCode, title, "", request) { }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public StringResult(HttpStatusCode statusCode, string title, string detail, HttpRequestMessage request)
        {
            var errors = new List<object>
            {
                new
                {
                    status = ((int) statusCode).ToString(),
                    title,
                    detail,
                }
            };
            _content = JsonConvert.SerializeObject(new
            {
               errors
            });
            _request = request;
            _statusCode = statusCode;
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_content),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
}
