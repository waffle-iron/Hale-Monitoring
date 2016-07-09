namespace Hale_Core.Entities.Shared
{
    /// <summary>
    /// Corresponds to the database table Shared.Metadata
    /// </summary>
    public class Metadata
    {
        /// <summary>
        /// Corresponds to the table column Metadata.Id
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to the table column Metadata.Type
        /// </summary>
        public string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to the table column Metadata.Attribute
        /// </summary>
        public string Attribute
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to the table column Metadata.Label
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to the table column Metadata.Description
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to the table column Metadata.Required
        /// </summary>
        public bool Required
        {
            get;
            set;
        }

        /// <summary>
        /// Corresponds to the table column Metadata.Protected
        /// </summary>
        public bool Protected
        {
            get;
            set;
        }
    }
}
