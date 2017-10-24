//  Copyright 2016 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System.Globalization;
using System.Text.RegularExpressions;

namespace MaterialUI
{
    /// <summary>
    /// Static class to decode icon codes to display as icons.
    /// </summary>
    public static class IconDecoder
    {
        /// <summary>
        /// The reg expression used to decode the icon codes.
        /// </summary>
        private static Regex m_RegExpression = new Regex(@"\\u(?<Value>[a-zA-Z0-9]{4})");

        /// <summary>
        /// Decodes the specified code.
        /// </summary>
        /// <param name="value">The icon code to decode.</param>
        /// <returns>A string that can be displayed as an icon (with the right font).</returns>
        public static string Decode(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            return m_RegExpression.Replace(value, m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString());
        }
    }
}