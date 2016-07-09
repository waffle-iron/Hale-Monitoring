using Newtonsoft.Json.Serialization;
using Humanizer;

namespace Hale_Core.Formatting
{
    /// <summary>
    /// TODO: Add a usage description.
    /// </summary>
    public class JsonAPIContractResolver:  DefaultContractResolver
    {

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        protected override string ResolvePropertyName(string propertyName)
        {
            return FormatName(propertyName);
        }

        /// <summary>
        /// TODO: Add a usage description.
        /// </summary>
        public static string FormatName(string input)
        {
            // Todo: Make sure this does not throw the exception "input cannot be null" -NM
            return input.Humanize(LetterCasing.LowerCase).Underscore().Dasherize();
        }
    }
}
