﻿using UnityEngine;
namespace Cr7Sund.Logger
{
    internal static class LogColorHelp
    {
        /// <summary>
        ///     ARGB的16进制数值转Color32
        ///     如:0xFF8428D9
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static Color32 HexToColor(uint hexColor)
        {
            uint byteLimit = 256;
            uint temp = hexColor;
            uint b = temp % byteLimit;
            temp /= byteLimit;
            uint g = temp % byteLimit;
            temp /= byteLimit;
            uint r = temp % byteLimit;
            temp /= byteLimit;
            uint a = temp % byteLimit;
            return new Color32((byte)r, (byte)g, (byte)b, (byte)a);
        }

        /// <summary>
        ///     Color32转ARGB的16进制数值
        /// </summary>
        /// <param name="hexColor"></param>
        /// <returns></returns>
        public static string ColorToHex(Color32 color)
        {
            return string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", color.r, color.g, color.b, color.a);
        }
    }
}
