using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaysonIntegration.Utils
{
    internal static class Extensions
    {
        static internal string GetValueOrNull(this IDictionary<string,string> dictionary, string key)
        {
            string tmp;
            if (dictionary.TryGetValue(key, out tmp))
                return tmp;
            return null;
        }
    }
}
