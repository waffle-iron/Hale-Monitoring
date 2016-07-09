using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Description;
using Hale.Core.Contexts;
using Hale.Core.Entities.Nodes;
using NLog;

namespace Hale.Core.API
{
    /// <summary>
    /// API for handling Host entries and related data.
    /// </summary>
    [RoutePrefix("api/v1/hosts")]
    public class HostController : ApiController
    {
        private readonly Hosts _hosts;
        private readonly HostDetails _hostdetails;
        private readonly Logger _log;

        internal HostController()
        {
            _log = LogManager.GetCurrentClassLogger();
            _hosts = new Hosts();
            _hostdetails = new HostDetails();
        }

        /// <summary>
        /// List host entities. (Auth)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route()]
        [ResponseType(typeof(List<Host>))]
        [AcceptVerbs("GET")]
        public IHttpActionResult List()
        {
            var hostList = _hosts.List();
            foreach(var host in hostList)
            {
                host.HostDetails = _hostdetails.List(host);
            }
            return Ok(hostList);
        }

        /// <summary>
        /// Get information on a specific host. (Auth)
        /// </summary>
        /// <param name="id">Host ID of the host in question.</param>
        /// <returns></returns>
        [Authorize]
        [Route("{id}")]
        [ResponseType(typeof (Host))]
        [AcceptVerbs("GET")]
        public IHttpActionResult Get(int id)
        {
            try
            {
                var host = _hosts.Get(new Host() { Id = id });
                host.HostDetails = _hostdetails.List(host);

                return Ok(host);
            }

            catch(Exception x)
            {
                return InternalServerError(x);
            }
        }

    }
}
