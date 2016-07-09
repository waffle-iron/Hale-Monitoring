using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JsonApiNet;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;
using Humanizer;
using System.Web.Http;

namespace Hale.Core.Formatting
{
    /// <summary>
    /// TODO: Add a usage description.
    /// </summary>
    public class JsonApiMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly string _apiBaseUrl;

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public JsonApiMediaTypeFormatter(string apiBaseUrl)
        {
            _apiBaseUrl = apiBaseUrl;
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.api+json"));
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            try
            {
                var memoryStream = new MemoryStream();
                readStream.CopyTo(memoryStream);
                var s = Encoding.UTF8.GetString(memoryStream.ToArray());
                object r = JsonApi.ResourceFromDocument(s, type);
                //object r = JsonApiNet.JsonApi.ResourceFromDocument(s, type);
                taskCompletionSource.SetResult(r);
            }
            catch (Exception x)
            {
                taskCompletionSource.SetException(x);
            }
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {

            var taskCompletionSource = new TaskCompletionSource<object>();


            if(type == typeof(HttpError))
            {
                return WriteErrorObject(type, value, writeStream, taskCompletionSource);
            }
            if((typeof(IEnumerable)).IsAssignableFrom(type))
            {
                return WriteMultiResource(type, value, writeStream, taskCompletionSource);
            }
            else
            {
                // Todo: Add more tests for actual suitability
                return WriteSingleResource(type, value, writeStream, taskCompletionSource);
            }


        }

        private Task WriteErrorObject(Type type, object value, Stream writeStream, TaskCompletionSource<object> taskCompletionSource)
        {
            var httpError = value as HttpError;

            var jao = new JsonAPI.ErrorRoot();
            jao.Errors = new List<JsonAPI.Error>();
            jao.Errors.Add(new JsonAPI.Error()
            {
                Title = httpError.Message,
                Detail = httpError.MessageDetail,
                Meta = new Dictionary<string, object>()
                {
                    { "Exception", JsonAPI.Utilities.FormatException(httpError) }
                }
            });

            SerializeJsonApiObject(jao, writeStream);

            taskCompletionSource.SetResult(null);
            return taskCompletionSource.Task;
        }

        private Task WriteMultiResource(Type type, object value, Stream writeStream, TaskCompletionSource<object> taskCompletionSource)
        {
            var jao = new JsonAPI.MultiResourceBase();
            jao.Data = new List<JsonAPI.DataResource>();
            jao.Included = new List<JsonAPI.Resource>();

            var dataItems = value as IEnumerable;

            foreach (var dataItem in dataItems)
            {
                var itemType = dataItem.GetType();

                var itemId = itemType.GetProperty("Id");
                var itemIdValue = itemId.GetValue(dataItem);

                var data = new JsonAPI.DataResource();
                data.Id = itemIdValue;
                data.Type = FormatType(itemType);
                data.Attributes = new Dictionary<string, object>();
                data.Relationships = new Dictionary<string, JsonAPI.Relationship>();

                var properties = itemType.GetProperties();
                foreach (var property in properties)
                {
                    if (JsonAPI.Utilities.HasIgnoreAttribute(property)) continue;

                    // Hack: This is an ugly way to filter out primitives and non-hash objects
                    if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string)
                        || property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(Guid))
                    {
                        if (itemId == null || property != itemId)
                            data.Attributes.Add(property.Name, property.GetValue(dataItem));
                    }
                    else if ((typeof(IEnumerable)).IsAssignableFrom(property.PropertyType))
                    {
                        try
                        {
                            var _v = property.GetValue(dataItem);
                            var relList = _v as IEnumerable;
                            //var relType = relObject.

                            var relationship = new JsonAPI.Relationship();
                            relationship.Data = new List<JsonAPI.ResourceIdentifier>();
                            relationship.Links = new Dictionary<string, string>();

                            Type relType = null;
                            string relTypeName = "";

                            foreach (var item in relList)
                            {
                                if (String.IsNullOrEmpty(relTypeName))
                                {
                                    relType = item.GetType();
                                    relTypeName = FormatType(relType);
                                }

                                var relIdProperty = item.GetType().GetProperty("Id");
                                var relIdValue = relIdProperty.GetValue(item);

                                relationship.Data.Add(new JsonAPI.ResourceIdentifier()
                                {
                                    Id = relIdValue,
                                    Type = relTypeName
                                });

                                var includedAttributes = new Dictionary<string, object>();
                                var includedProperties = relType.GetProperties();
                                foreach (var includedProperty in includedProperties)
                                {
                                    if (JsonAPI.Utilities.HasIgnoreAttribute(includedProperty)) continue;

                                    if (includedProperty.PropertyType.IsPrimitive || includedProperty.PropertyType == typeof(string)
                                        || includedProperty.PropertyType == typeof(DateTime) || includedProperty.PropertyType == typeof(Guid))
                                    {
                                        if (relIdProperty == null || includedProperty != relIdProperty)
                                            includedAttributes.Add(includedProperty.Name, includedProperty.GetValue(item));
                                    }
                                }


                                var includedResouce = new JsonAPI.Resource()
                                {
                                    Id = relIdValue,
                                    Type = relTypeName,
                                    Attributes = includedAttributes,
                                    Links = new Dictionary<string, string>()
                                    {
                                        { "self",  BuildLink(type, itemIdValue, relType, relIdValue) }
                                    }
                                };

                                jao.Included.Add(includedResouce);
                            }


                            data.Relationships.Add(property.Name, relationship);

                        }
                        catch (Exception x)
                        {
                            throw new Exception("Cannot cast non-primitive type to IEnumerable.", x);
                        }
                    }
                    else
                    {
                        throw new Exception($"Cannot serialize property of type {property.PropertyType.ToString()}");
                    }


                }

                jao.Data.Add(data);
            }

            SerializeJsonApiObject(jao, writeStream);

            taskCompletionSource.SetResult(null);
            return taskCompletionSource.Task;
        }

        private Task WriteSingleResource(Type type, object value, Stream writeStream, TaskCompletionSource<object> taskCompletionSource)
        {
            try
            {

                object idValue = null;
                PropertyInfo idPropertyInfo = null;
                FieldInfo idFieldInfo = null;

                try
                {
                    idFieldInfo = type.GetField("Id", BindingFlags.Public);
                    if (idFieldInfo != null)
                    {
                        idValue = idFieldInfo.GetValue(value);
                    }
                    else
                    {
                        idPropertyInfo = type.GetProperty("Id");
                        idValue = idPropertyInfo.GetValue(value);
                    }
                }
                catch (Exception x)
                {
                    throw new Exception("Canot find the Id property of the input type.", x);
                }


                var jao = new JsonAPI.SingleResourceBase();
                jao.Data = new JsonAPI.DataResource();
                jao.Data.Id = idValue;
                jao.Data.Type = FormatType(type);
                jao.Data.Attributes = new Dictionary<string, object>();

                jao.Included = new List<JsonAPI.Resource>();
                jao.Data.Relationships = new Dictionary<string, JsonAPI.Relationship>();

                var fields = type.GetFields(BindingFlags.Public);
                foreach (var field in fields)
                {
                    jao.Data.Attributes.Add(field.Name, field.GetValue(value));
                }
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    // Hack: This is an ugly way to filter out primitives and non-hash objects
                    if (property.PropertyType.IsPrimitive || property.PropertyType == typeof(string) || property.PropertyType == typeof(DateTime))
                    {
                        if (idPropertyInfo == null || property != idPropertyInfo)
                            jao.Data.Attributes.Add(property.Name, property.GetValue(value));
                    }
                    else if ((typeof(IEnumerable)).IsAssignableFrom(property.PropertyType))
                    {
                        try
                        {
                            var relList = property.GetValue(value) as IEnumerable;
                            //var relType = relObject.

                            var relationship = new JsonAPI.Relationship();
                            relationship.Data = new List<JsonAPI.ResourceIdentifier>();
                            relationship.Links = new Dictionary<string, string>();


                            var included = new List<JsonAPI.Resource>();

                            Type relType = null;
                            string relTypeName = "";

                            foreach (var item in relList)
                            {
                                if (String.IsNullOrEmpty(relTypeName))
                                {
                                    relType = item.GetType();
                                    relTypeName = FormatType(relType);
                                }

                                var relIdProperty = item.GetType().GetProperty("Id");
                                var relIdValue = relIdProperty.GetValue(item);

                                relationship.Data.Add(new JsonAPI.ResourceIdentifier()
                                {
                                    Id = relIdValue,
                                    Type = relTypeName
                                });

                                var includedAttributes = new Dictionary<string, object>();
                                var includedProperties = relType.GetProperties();
                                foreach (var includedProperty in includedProperties)
                                {
                                    if (JsonAPI.Utilities.HasIgnoreAttribute(includedProperty)) continue;

                                    if (includedProperty.PropertyType.IsPrimitive || includedProperty.PropertyType == typeof(string)
                                        || includedProperty.PropertyType == typeof(DateTime) || includedProperty.PropertyType == typeof(Guid))
                                    {
                                        if (relIdProperty == null || includedProperty != relIdProperty)
                                            includedAttributes.Add(includedProperty.Name, includedProperty.GetValue(item));
                                    }
                                }

                                var includedResouce = new JsonAPI.Resource()
                                {
                                    Type = relTypeName,
                                    Id = relIdValue,
                                    Attributes = includedAttributes,
                                    Links = new Dictionary<string, string>()
                                    {
                                        {"self",  BuildLink(type, idValue, relType, relIdValue) }
                                    }
                                };

                                jao.Included.Add(includedResouce);
                            }


                            jao.Data.Relationships.Add(property.Name, relationship);
                        }
                        catch (Exception x)
                        {
                            throw new Exception("Cannot cast non-primitive type to IEnumerable.", x);
                        }
                    }
                    else
                    {
                        throw new Exception($"Cannot serialize property of type {property.PropertyType.ToString()}");
                    }
                }

                SerializeJsonApiObject(jao, writeStream);

                taskCompletionSource.SetResult(null);
                return taskCompletionSource.Task;
            }
            catch (Exception x)
            {
                taskCompletionSource.SetException(x);
                return taskCompletionSource.Task;
            }
        }

        private void SerializeJsonApiObject(JsonAPI.JsonApiBase jao, Stream writeStream)
        {
            var sw = new StreamWriter(writeStream);

            var js = new Newtonsoft.Json.JsonSerializer
            {
                ContractResolver = new JsonAPIContractResolver(),
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
            };

            js.Serialize(sw, jao);
            sw.Flush();
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public override bool CanReadType(Type type)
        {
            return true;// type == typeof(string);
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public override bool CanWriteType(Type type)
        {
            return (type.GetProperty("Id") != null ||  ((typeof(IEnumerable)).IsAssignableFrom(type)));
            //return true;
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string FormatType(Type type)
        {
            return JsonAPIContractResolver.FormatName(type.Name).Pluralize(false);
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string BuildLink(Type type, object id)
        {
            return $"{_apiBaseUrl}/{FormatType(type)}/{id.ToString()}";
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public string BuildLink(Type parentType, object parentId, Type type, object id)
        {
            var childTypeName = FormatType(type);
            var parentTypeName = FormatType(parentType);
            if(childTypeName.IndexOf(parentTypeName, StringComparison.Ordinal)==0)
            {
                childTypeName = childTypeName.Substring(parentTypeName.Length + 1);
            }
            return $"{_apiBaseUrl}/{parentTypeName}/{parentId.ToString()}/{childTypeName}/{id.ToString()}";
        }




    }
}
