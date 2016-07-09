using System;
using System.Runtime.Remoting.Proxies;

namespace Hale_Core.Entities.Checks
{
    /// <summary>
    /// Corresponds to the database table Checks.Metrics
    /// </summary>
    public class Metric
    {
        /// <summary>
        /// Corresponds to the table column Checks.Metrics.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Metrics.ResultId
        /// </summary>
        public int ResultId { get; set; }
        
        /// <summary>
        /// Corresponds to the table column Checks.Metrics.Target
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Metrics.RawValue
        /// </summary>
        public float RawValue { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.Metrics.Weight
        /// </summary>
        public double Weight { get; set; }
        
    }
}