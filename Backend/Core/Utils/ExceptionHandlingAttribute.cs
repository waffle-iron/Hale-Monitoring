using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Hale.Core.Utils
{
    class ExceptionHandlingAttribute: ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            // TODO(NM): Make exception handling a bit more sophisticated

            context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(context.Exception.Message.ToString())
            };
        }
    }
}
