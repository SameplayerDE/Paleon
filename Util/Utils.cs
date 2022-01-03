using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Paleon
{

    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    public static class Utils
    {
        public static Color PURPLE = new Color(56, 65, 107, 64);
        public static Color ORANGE = new Color(120, 40, 0, 64);
        public static Color GRAY = new Color(32, 32, 32, 64);
        public static Color BLACK = new Color(0, 0, 0, 64);

        public static string EnumToString(Enum enumeration)
        {
            // Переводим текст на нижний регистр и заменяем '_' на ' ' 
            return enumeration.ToString().Replace("_", " ");
        }

        public static XmlDocument LoadContentXML(string filename)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(TitleContainer.OpenStream(Path.Combine(Engine.Instance.Content.RootDirectory, filename)));
            return xml;
        }

        public static XmlDocument LoadXML(string filename)
        {
            XmlDocument xml = new XmlDocument();
            using (var stream = File.OpenRead(filename))
                xml.Load(stream);
            return xml;
        }

        public static string ReadNullTerminatedString(this BinaryReader stream)
        {
            string str = "";
            char ch;
            while ((int)(ch = stream.ReadChar()) != 0)
                str = str + ch;
            return str;
        }

        public static Vector2 ClosestPointOnLine(Vector2 lineA, Vector2 lineB, Vector2 closestTo)
        {
            Vector2 v = lineB - lineA;
            Vector2 w = closestTo - lineA;
            float t = Vector2.Dot(w, v) / Vector2.Dot(v, v);
            t = MathHelper.Clamp(t, 0, 1);

            return lineA + v * t;
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float Clamp(float value, float min, float max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static float Angle(this Vector2 vector)
        {
            return (float)Math.Atan2(vector.Y, vector.X);
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }

        public static Vector2 Perpendicular(this Vector2 vector)
        {
            return new Vector2(-vector.Y, vector.X);
        }

        public static Vector2 AngleToVector(float angleRadians, float length)
        {
            return new Vector2((float)Math.Cos(angleRadians) * length, (float)Math.Sin(angleRadians) * length);
        }

        public static Color GetRandomColor(float alpha = 1.0f)
        {
            byte red = (byte)MyRandom.Range(255);
            byte green = (byte)MyRandom.Range(255);
            byte blue = (byte)MyRandom.Range(255);
            return new Color(red, green, blue) * alpha;
        }

        #region XML Attributes

        public static bool HasAttr(this XmlElement xml, string attributeName)
        {
            return xml.Attributes[attributeName] != null;
        }

        public static string Attr(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return xml.Attributes[attributeName].InnerText;
        }

        public static string Attr(this XmlElement xml, string attributeName, string defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return xml.Attributes[attributeName].InnerText;
        }

        public static int AttrInt(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
        }

        public static int AttrInt(this XmlElement xml, string attributeName, int defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return Convert.ToInt32(xml.Attributes[attributeName].InnerText);
        }

        public static float AttrFloat(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
        }

        public static float AttrFloat(this XmlElement xml, string attributeName, float defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return Convert.ToSingle(xml.Attributes[attributeName].InnerText, CultureInfo.InvariantCulture);
        }

        public static Vector3 AttrVector3(this XmlElement xml, string attributeName)
        {
            var attr = xml.Attr(attributeName).Split(',');
            var x = float.Parse(attr[0].Trim(), CultureInfo.InvariantCulture);
            var y = float.Parse(attr[1].Trim(), CultureInfo.InvariantCulture);
            var z = float.Parse(attr[2].Trim(), CultureInfo.InvariantCulture);

            return new Vector3(x, y, z);
        }

        public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName)
        {
            return new Vector2(xml.AttrFloat(xAttributeName), xml.AttrFloat(yAttributeName));
        }

        public static Vector2 AttrVector2(this XmlElement xml, string xAttributeName, string yAttributeName, Vector2 defaultValue)
        {
            return new Vector2(xml.AttrFloat(xAttributeName, defaultValue.X), xml.AttrFloat(yAttributeName, defaultValue.Y));
        }

        public static bool AttrBool(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToBoolean(xml.Attributes[attributeName].InnerText);
        }

        public static bool AttrBool(this XmlElement xml, string attributeName, bool defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrBool(xml, attributeName);
        }

        public static char AttrChar(this XmlElement xml, string attributeName)
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            return Convert.ToChar(xml.Attributes[attributeName].InnerText);
        }

        public static char AttrChar(this XmlElement xml, string attributeName, char defaultValue)
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return AttrChar(xml, attributeName);
        }

        public static T AttrEnum<T>(this XmlElement xml, string attributeName) where T : struct
        {
#if DEBUG
            if (!xml.HasAttr(attributeName))
                throw new Exception("Element does not contain the attribute \"" + attributeName + "\"");
#endif
            if (Enum.IsDefined(typeof(T), xml.Attributes[attributeName].InnerText))
                return (T)Enum.Parse(typeof(T), xml.Attributes[attributeName].InnerText);
            else
                throw new Exception("The attribute value cannot be converted to the enum type.");
        }

        public static T AttrEnum<T>(this XmlElement xml, string attributeName, T defaultValue) where T : struct
        {
            if (!xml.HasAttr(attributeName))
                return defaultValue;
            else
                return xml.AttrEnum<T>(attributeName);
        }

        public static Vector2 Position(this XmlElement xml)
        {
            return new Vector2(xml.AttrFloat("x"), xml.AttrFloat("y"));
        }

        public static Vector2 Position(this XmlElement xml, Vector2 defaultPosition)
        {
            return new Vector2(xml.AttrFloat("x", defaultPosition.X), xml.AttrFloat("y", defaultPosition.Y));
        }

        public static int X(this XmlElement xml)
        {
            return xml.AttrInt("x");
        }

        public static int X(this XmlElement xml, int defaultX)
        {
            return xml.AttrInt("x", defaultX);
        }

        public static int Y(this XmlElement xml)
        {
            return xml.AttrInt("y");
        }

        public static int Y(this XmlElement xml, int defaultY)
        {
            return xml.AttrInt("y", defaultY);
        }

        public static int Width(this XmlElement xml)
        {
            return xml.AttrInt("width");
        }

        public static int Width(this XmlElement xml, int defaultWidth)
        {
            return xml.AttrInt("width", defaultWidth);
        }

        public static int Height(this XmlElement xml)
        {
            return xml.AttrInt("height");
        }

        public static int Height(this XmlElement xml, int defaultHeight)
        {
            return xml.AttrInt("height", defaultHeight);
        }

        public static Rectangle Rect(this XmlElement xml)
        {
            return new Rectangle(xml.X(), xml.Y(), xml.Width(), xml.Height());
        }

        public static int ID(this XmlElement xml)
        {
            return xml.AttrInt("id");
        }

        #endregion
    }
}

