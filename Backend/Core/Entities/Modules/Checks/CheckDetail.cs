using Hale_Core.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hale_Core.Entities.Checks
{
    /// <summary>
    /// Corresponds to the database table Checks.CheckDetails
    /// </summary>
    public class CheckDetail
    {

        /// <summary>
        /// Corresponds to the table column Checks.CheckDetails.Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.CheckDetails.CheckId (FK: Checks.Check.Id)
        /// </summary>
        public int CheckId { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.CheckDetails.Version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.CheckDetails.Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.CheckDetails.Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Corresponds to the table column Checks.CheckDetails.Activated
        /// </summary>
        public bool Activated { get; set; }
    }
}
