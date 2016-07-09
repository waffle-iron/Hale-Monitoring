using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Hale_Core.Contexts;
using Hale_Core.Entities;
using Hale_Core.Entities.Shared;
using Hale_Core.Handlers;
using Hale_Core.Utils;
using NLog;

namespace Hale_Core.API
{

    /// <summary>
    /// Handles metadata templates for core entities.
    /// </summary>
    [RoutePrefix("api/v1/metadata")]
    public class MetadataController : ApiController
    {
        private readonly Logger _log;
        private readonly Metadatum _metadatum;

        internal MetadataController()
        {
            _metadatum = new Metadatum();
            _log = LogManager.GetCurrentClassLogger();   
        }

        /// <summary>
        ///     Returns the template used for the requested types metadata.
        /// </summary>
        /// <param name="type">A type of domain model, for example user, host etc.</param>
        /// <returns>A metadata template</returns>
        [AcceptVerbs("GET")]
        [ResponseType(typeof (Metadata))]
        [Route("{type}")]
        public IHttpActionResult List(string type)
        {
            try
            {
                List<Metadata> template = _metadatum.List(new Metadata() { Type = type });
                return Ok(template);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
        }

        /// <summary>
        ///     Returns a single metadata attribute.
        /// </summary>
        /// <param name="type">A type of domain model, for example user, host etc.</param>
        /// <param name="id">The ID of a single metadata attribute.</param>
        /// <returns></returns>
        [AcceptVerbs("GET")]
        [ResponseType(typeof(Metadata))]
        [Route("{type}/{id}")]
        public IHttpActionResult Get(string type, int id)
        {
            try
            {
                Metadata metadata = _metadatum.Get(new Metadata() { Type = type, Id = id });
                return Ok(metadata);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        /// <summary>
        /// Creates metadata attribute for a specific entity type
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs("PUT")]
        [Route("{type}")]
        public IHttpActionResult Put(string type, [FromBody]Metadata metadata)
        {
            try
            {
                // Use the type provided by the rest URI.
                metadata.Type = type;
                _metadatum.Create(metadata);

                // Refresh
                metadata = _metadatum.Get(metadata);

                return Ok(metadata);
            }
            catch
            {
                return InternalServerError();
            }
            
        }

        /// <summary>
        /// Creates metadata attribute for a specific entity type
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [AcceptVerbs("PUT")]
        [Route("{type}/{id}")]
        public IHttpActionResult Patch(string type, int id, [FromBody]Metadata metadata)
        {
            try
            {
                // Use the type provided by the rest URI.
                metadata.Type = type;
                metadata.Id = id;

                _metadatum.Update(metadata);
                metadata = _metadatum.Get(metadata);

                return Ok(metadata);
            }
            catch
            {
                return InternalServerError();
            }

        }

    }
}
