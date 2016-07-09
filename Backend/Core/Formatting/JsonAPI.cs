using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Links = System.Collections.Generic.Dictionary<string, string>;
using Meta = System.Collections.Generic.Dictionary<string, object>;

namespace Hale_Core.Formatting.JsonAPI
{

    // Todo: Add list initializers inside constructors for all classes -NM
    /// <summary>
    /// JSON API Header. Contains the version number as a string.
    /// </summary>
    public class JsonApiHeader
    {
        /// <summary>
        /// The version number of the JSON API.
        /// </summary>
        public string Version
        {
            get
            {
                return "1.0";
            }
        }
    }

    /// <summary>
    /// Base class. Holds an instance of JsonApiHeader.
    /// </summary>
    public class JsonApiBase
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public JsonApiHeader Jsonapi { get; set; }
    }

    /// <summary>
    /// A derived class, based on JsonApiBase. Includes resources and links.
    /// </summary>
    public class JsonApiResourceBase: JsonApiBase
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public List<Resource> Included { get; set; }
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Links Links { get; set; }
    }

    /// <summary>
    /// Used for holding single data resources.
    /// </summary>
    public class SingleResourceBase: JsonApiResourceBase
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public DataResource Data { get; set; }
    }

    /// <summary>
    /// Used for holding multiple data resources.
    /// </summary>
    public class MultiResourceBase : JsonApiResourceBase
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public List<DataResource> Data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ResourceIdentifier
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public object Id { get; set; }
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Resource instance containing attributes and links.
    /// </summary>
    public class Resource: ResourceIdentifier
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Dictionary<string, object> Attributes { get; set; }
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Links Links { get; set; }
    }


    /// <summary>
    /// Resource instance derived from Resource. Contains Relationships.
    /// </summary>
    public class DataResource: Resource
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Dictionary<string, Relationship> Relationships { get; set; }
    }

    /// <summary>
    /// TODO: Add a usage description.
    /// </summary>
    public class Relationship
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Links Links { get; set; }
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public List<ResourceIdentifier> Data { get; set; }
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Meta Meta { get; set; }
    }

    /// <summary>
    /// Contains a list of error objects.
    /// </summary>
    public class ErrorRoot: JsonApiBase
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public List<Error> Errors { get; set; }
    }

    /// <summary>
    /// Single error objects.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Source Source { get; set; }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Meta Meta { get; set; }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Links Links { get; set; }

    }

    /// <summary>
    /// Contains a pointer and a parameter.
    /// </summary>
    public class Source
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string Pointer { get; set; }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string Parameter { get; set; }
    }

    /// <summary>
    /// Utility classes for the JSON API.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Custom exception type, generated from a generic exception.
        /// </summary>
        /// <param name="x">The original exception</param>
        /// <returns>A dictionary containing the format exception.</returns>
        public static Dictionary<string, object> FormatException(Exception x)
        {
            Dictionary<string, object> d = new Dictionary<string, object>
            {
                {"Type", x.GetType().FullName},
                {"Message", x.Message},
                {"StackTrace", x.StackTrace},
                {"TargetSite", x.TargetSite},
                {"Data", x.Data},
                {"Source", x.Source}
            };

            if(x.InnerException != null)
            {
                d.Add("InnerException", FormatException(x.InnerException));
            }
            return d;
        }

        /// <summary>
        /// Custom exception type, generated from a http error.
        /// </summary>
        /// <param name="e">The original exception</param>
        /// <returns>A dictionary containing the http error exception.</returns>
        public static Dictionary<string, object> FormatException(HttpError e)
        {
            Dictionary<string, object> d = new Dictionary<string, object>
            {
                {"StackTrace", e.StackTrace},
                {"Message", e.ExceptionMessage},
                {"Type", e.ExceptionType}
            };

            if(e.InnerException != null)
            {
                d.Add("InnerException", FormatException(e.InnerException));
            }

            return d;

        }

        internal static bool HasIgnoreAttribute(PropertyInfo propInfo)
        {
            var attr = propInfo.GetCustomAttribute<Newtonsoft.Json.JsonIgnoreAttribute>();
            return attr != null;
        }
    }

}
