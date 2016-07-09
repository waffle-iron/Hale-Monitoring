using System;
using System.Net;
using Newtonsoft.Json;

using GenericRequest = System.Collections.Generic.Dictionary<string, object>;

// ReSharper disable once CheckNamespace
namespace Hale.Alert.Pushbullet
{
    public class PushbulletApi
    {
        static readonly string URL_API_BASE = "https://api.pushbullet.com/";

        //private readonly string _accessToken;

        public PushbulletApi(string accessToken)
        {
            //_accessToken = accessToken;
        }

        public PushResponse Push(string title, string body, HaleAlertPushbulletRecipient recipient)
        {
            var request = new GenericRequest()
            {
                {"title", title},
                {"body", body},
                {"type", "note"}
            };
            switch (recipient.TargetType)
            {
                case PushbulletPushTarget.Device:
                    request.Add("device_iden", recipient.Target);
                    break;
                case PushbulletPushTarget.Email:
                    request.Add("email", recipient.Target);
                    break;
                case PushbulletPushTarget.Channel:
                    request.Add("channel_tag", recipient.Target);
                    break;
                case PushbulletPushTarget.Client:
                    request.Add("client_iden", recipient.Target);
                    break;

            }
            return MakeRequest<PushResponse>("pushes", recipient.AccessToken, request);
        }

        public object MakeRequest(string route, string accessToken, object request = null, string version = "v2")
        {
            return MakeRequest<object>(route, accessToken, request, version);
        }

        public T MakeRequest<T>(string route, string accessToken, object request = null, string version = "v2")
        {
            WebClient wc = new WebClient();

            wc.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + accessToken);
            wc.Headers.Add(HttpRequestHeader.ContentType, "application/json");

            var uri = new Uri(string.Concat(URL_API_BASE, version, "/", route));

            try
            {
                string requestJson = request == null ? "" : JsonConvert.SerializeObject(request);

                var responseJson = string.IsNullOrEmpty(requestJson) ? 
                    wc.DownloadString(uri) : 
                    wc.UploadString(uri, requestJson);

                //var responseJson = Encoding.UTF8.GetString(result);

                return JsonConvert.DeserializeObject<T>(responseJson);
            }
            catch (WebException x)
            {
                /*
                  HTTP Responses from Pushbullet API documentation
                    200 OK - Everything worked as expected.
                    400 Bad Request - Usually this results from missing a required parameter.
                    401 Unauthorized - No valid access token provided.
                    403 Forbidden - The access token is not valid for that request.
                    404 Not Found - The requested item doesn't exist.
                    429 Too Many Requests - You have been ratelimited for making too many requests to the server.
                    5XX Server Error - Something went wrong on Pushbullet's side.
                 */

                switch (((HttpWebResponse) x.Response).StatusCode)
                {
                    case HttpStatusCode.OK:
                        throw new Exception("Internal error. Success treated as exception.");

                    case HttpStatusCode.BadRequest:
                        throw new Exception("Invalid request or missing parameter.");

                    case HttpStatusCode.Unauthorized:
                        throw new Exception("Invalid access token.");

                    case HttpStatusCode.Forbidden:
                        throw new Exception("The access token is not valid for that request.");

                    case HttpStatusCode.NotFound:
                        throw new Exception("The requested item doesn't exist.");

                    case (HttpStatusCode)429:
                        throw new Exception("Too many requests.");

                    case HttpStatusCode.InternalServerError:
                        throw new Exception("Pushbullet internal error.");

                    default:
                        throw;
                }
            }

        }
    }

    public class PushResponse
    {
        /*
          "iden": "ubdpj29aOK0sKG",
          "type": "note",
          "title": "Note Title",
          "body": "Note Body",
          "created": 1399253701.9744401,
          "modified": 1399253701.9746201,
          "active": true,
          "dismissed": false,
          "sender_iden": "ubd",
          "sender_email": "ryan@pushbullet.com",
          "sender_email_normalized": "ryan@pushbullet.com",
          "receiver_iden": "ubd",
          "receiver_email": "ryan@pushbullet.com",
          "receiver_email_normalized": "ryan@pushbullet.com"
         */
        public string Iden;
        public string Type;
        public string Title;
        public string Body;
        public double Created;
        public double Modified;
        public bool Active;
        public bool Dismissed;
        public string SenderIden;
        public string SenderEmail;
        public string SenderEmailNormalized;
        public string ReceiverIden;
        public string ReceiverEmail;
        public string ReceiverEmailNormalized;
    }
}
