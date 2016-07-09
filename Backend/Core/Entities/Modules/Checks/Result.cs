using System;
using System.Data.SqlTypes;

namespace Hale_Core.Entities.Checks
{
    /// <summary>
    /// Corresponds to the database table Checks.Results
    /// </summary>
    public class Result_
    {
        /// <summary>
        /// Corresponds to the table column Checks.Results.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Results.CheckId (FK: Checks.Checks.Id)
        /// </summary>
        public int CheckId { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Results.CheckDetailId (FK: Checks.CheckDetails.Id)
        /// </summary>
        public int CheckDetailId { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Results.HostId (FK: Nodes.Hosts.Id)
        /// </summary>
        public int HostId { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Results.ResultType
        /// </summary>
        public int ResultType { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Results.ExecutionTime
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Results.Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Results.Exception
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Results.Target
        /// </summary>
        public string Target { get; set; }
    }
}
