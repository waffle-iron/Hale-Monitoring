using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hale.Core.Entities.Nodes
{
    /// <summary>
    /// Corresponds to the database table Nodes.Hosts
    /// </summary>
    public class Host
    {
        /// <summary>
        /// Corresponds to the table column Hosts.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Corresponds to the table column Hosts.Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Corresponds to the table column Hosts.HostName
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Corresponds to the table column Hosts.Ip
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// Corresponds to the table column Hosts.Status
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        ///  Corresponds to the table column Hosts.LastSeen
        /// </summary>
        public DateTime LastSeen { get; set; }

        /// <summary>
        /// Corresponds to the table column Hosts.Updated
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// Corresponds to the table column Hosts.Added
        /// </summary>
        public DateTime Added { get; set; }

        /// <summary>
        /// Corresponds to the table column Hosts.Guid
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Corresponds to the table column Hosts.RsaKey.
        /// Is not serialized by the JsonApiAdapter.
        /// </summary>
        [JsonIgnore]
        public byte[] RsaKey { get; set; }

        /// <summary>
        /// Wrapper containing data in a one-to-many relationship to Nodes.HostDetails.
        /// </summary>
        public List<HostDetail> HostDetails { get; set; }
    }

    

    
}