namespace Hale.Core.Entities.Nodes
{
    /// <summary>
    /// Corresponds to the database table Nodes.HostDetails
    /// </summary>
    public class HostDetail
    {
        /// <summary>
        /// Corresponds to the table column HostDetails.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Corresponds to the table column HostDetails.HostId (FK: Nodes.Hosts.Id)
        /// </summary>
        public int HostId { get; set; }

        /// <summary>
        /// Corresponds to the table column HostDetails.Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Corresponds to the table column HostDetails.Value
        /// </summary>
        public string Value { get; set; }
    }
}
