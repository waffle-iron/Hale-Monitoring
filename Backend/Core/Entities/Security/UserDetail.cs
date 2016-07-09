using Newtonsoft.Json;

namespace Hale_Core.Entities.Security
{
    /// <summary>
    /// Corresponds to the database table Security.UserDetails
    /// </summary>
    public class UserDetail
    {
        /// <summary>
        /// Corresponds to the table column UserDetails.Id
        /// </summary>
        [JsonProperty("_id")]
        public int Id { get; set; }

        /// <summary>
        /// Corresponds to the table column UserDetails.UserId (FK: Security.Users.Id)
        /// </summary>
        [JsonProperty("userId")]
        public int UserId { get; set; }

        /// <summary>
        /// Corresponds to the table column UserDetails.Key
        /// </summary>
        [JsonProperty("id")]
        public string Key { get; set; }

        /// <summary>
        /// Corresponds to the table column UserDetails.Value
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}