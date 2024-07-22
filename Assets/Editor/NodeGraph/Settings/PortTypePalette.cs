using System;
using Cr7Sund.FrameWork.Util;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cr7Sund.Editor
{
    [System.Serializable]
    public class PortTypePalette
    {
        [HexDrawer] public string IntHex = "#d9ed92";     // Old: "#0A1900"
        [HexDrawer] public string ShortHex = "#b5e48c";   // Old: "#FF0AAD"
        [HexDrawer] public string LongHex = "#99d98c";    // Old: "#5B5B5B"
        [HexDrawer] public string FloatHex = "#76c893";   // Old: "#FA230F"
        [HexDrawer] public string DoubleHex = "#52b69a";  // Old: "#5C5C47"
        [HexDrawer] public string CharHex = "#34a0a4";    // Old: "#FF8914"
        [HexDrawer] public string StringHex = "#168aad";  // Old: "#B37A80"
        [HexDrawer] public string ColorHex = "#1a759f";   // Old: "#6243EE"
        [HexDrawer] public string Vector2Hex = "#1e6091"; // Old: "#3b0d11"
        [HexDrawer] public string Vector3Hex = "#184e77"; // Old: "#45B8FF"
        [HexDrawer] public string QuaternionHex = "#184e77"; // Old: "#45B8FF"
        [HexDrawer] public string CollectionHex = "#D4D3D2"; // Kept unchanged
        [HexDrawer] public string DefaultHex = "#93DF29";    // Kept unchanged

        internal string[] PaletteA = new string[] { "d9ed92", "b5e48c", "99d98c", "76c893", "52b69a", "34a0a4", "168aad", "1a759f", "1e6091", "184e77" };
        internal string[] PaletteB = new string[] { "03071e", "370617", "6a040f", "9d0208", "d00000", "dc2f02", "e85d04", "f48c06", "faa307", "ffba08" };

        public string GetHex(Type type)
        {
            if (type == typeof(int))
            {
                return IntHex;
            }
            if (type == typeof(short))
            {
                return ShortHex;
            }
            if (type == typeof(long))
            {
                return LongHex;
            }
            if (type == typeof(float))
            {
                return FloatHex;
            }
            if (type == typeof(double))
            {
                return DoubleHex;
            }
            if (type == typeof(char))
            {
                return CharHex;
            }
            if (type == typeof(string))
            {
                return StringHex;
            }
            if (type == typeof(Color))
            {
                return ColorHex;
            }
            if (type == typeof(Vector2))
            {
                return Vector2Hex;
            }
            if (type == typeof(Vector3))
            {
                return Vector3Hex;
            }
            if (type == typeof(Quaternion))
            {
                return QuaternionHex;
            }
            if (ReflectUtil.IsCollection(type))
            {
                return CollectionHex;
            }
            return DefaultHex;
        }

        public Color GetColor(Type type)
        {
            var hex = GetHex(type);
            ColorUtility.TryParseHtmlString(hex, out var color);
            return color;
        }

        public void CopyFromPalette(string[] colors)
        {
            var fields = typeof(PortTypePalette).GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            for (int i = 0; i < fields.Length; i++)
            {
                System.Reflection.FieldInfo field = fields[i];
                if (colors.Length > i)
                {
                    string color = colors[i];
                    color = $"#{color}";
                    field.SetValue(this, color);
                }
            }
        }
    }

}