using System.Collections.Generic;

namespace Hale.Core.Entities.Nodes
{

    /// <summary>
    /// JsonApiAdapter wrapper not represented in the database domain.
    /// Host [1..*] HostDetail
    /// </summary>
    public class HostResponseWrapper
    {
        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public Host Host { get; set; }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public List<HostDetail> Details { get; set; }
    }
}
