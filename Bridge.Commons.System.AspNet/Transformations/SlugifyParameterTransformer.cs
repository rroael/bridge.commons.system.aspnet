using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace Bridge.Commons.System.AspNet.Transformations
{
    /// <summary>
    ///     Tranformador por parâmetro (Slugify)
    /// </summary>
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        /// <summary>
        ///     Transformar saída
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string TransformOutbound(object value)
        {
            // Slugify value
            return value == null ? null : Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
        }
    }
}