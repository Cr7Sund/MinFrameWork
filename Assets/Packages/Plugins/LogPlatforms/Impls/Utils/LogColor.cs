using UnityEngine;

namespace Cr7Sund.Logger
{
    internal static class LogColorHelp
    {
        /// <summary>
        ///     Convert ARGB hexadecimal value to Color32.
        ///     Example: 0xFF8428D9
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static Color32 HexToColor(string hexColor)
        {
            if (!ColorUtility.TryParseHtmlString(hexColor, out var color))
            {
                color = Color.white;
            }

            return color;
        }

        /// <summary>
        ///     Convert Color32 to ARGB hexadecimal value.
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static string ColorToHex(Color32 color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }
    }
}
