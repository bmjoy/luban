using Luban.Common.Utils;
using Luban.Job.Cfg.Datas;
using Luban.Job.Cfg.DataSources;
using Luban.Job.Cfg.Defs;
using Luban.Job.Cfg.TypeVisitors;
using Luban.Job.Common.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Luban.Job.Cfg.Utils
{
    static class DataUtil
    {
        public static string[] SplitVectorString(string x)
        {
            return x.Split(',', '_', ';');
        }

        public static string[] SplitStringByAnySepChar(string x, string sep)
        {
            return x.Split(sep.ToCharArray());
        }

        public static DType CreateVector(TVector2 type, string x)
        {
            var values = SplitVectorString(x);

            return new DVector2(new System.Numerics.Vector2(float.Parse(values[0]), float.Parse(values[1])));

        }

        public static DType CreateVector(TVector3 type, string x)
        {
            var values = SplitVectorString(x);

            return new DVector3(new System.Numerics.Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2])));

        }

        public static DType CreateVector(TVector4 type, string x)
        {
            var values = SplitVectorString(x);
            return new DVector4(new System.Numerics.Vector4(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
        }

        //public static DDateTime CreateDateTime(string x, TimeZoneInfo timeZoneInfo)
        //{

        //    DateTime dateTime = DateTime.ParseExact(x,
        //        new string[] {
        //            "yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd HH", "yyyy-MM-dd",
        //            //"yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH", "yyyy/MM/dd",
        //        },
        //        System.Globalization.CultureInfo.InvariantCulture);
        //    return new DDateTime(TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInfo));
        //}
        private static readonly string[] dateTimeFormats = new string[] {
                    "yyyy-M-d HH:mm:ss", "yyyy-M-d HH:mm", "yyyy-M-d HH", "yyyy-M-d",
                    //"yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm", "yyyy/MM/dd HH", "yyyy/MM/dd",
                };
        public static DDateTime CreateDateTime(string x)
        {
            DateTime dateTime = DateTime.ParseExact(x, dateTimeFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
            //return new DDateTime(TimeZoneInfo.ConvertTime(dateTime, TimeZoneUtil.DefaultTimeZone, TimeZoneInfo.Utc));
            return new DDateTime(dateTime);
        }

        public static string FormatDateTime(DateTime datetime)
        {
            return datetime.ToString("yyyy-M-d HH:mm:ss");
        }

        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public static string UnEscapeRawString(string s)
        {
            switch (s)
            {
                case "null": return null;
                case "\"\"": return string.Empty;
                default: return s;
            }
        }

        public static string EscapeString(string s)
        {
            return s.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        //public static string EscapeStringWithQuote(string s)
        //{
        //    return "\"" + s.Replace("\\", "\\\\") + "\"";
        //}

        public static (string Key, string Text) ExtractText(string rawKeyAndText)
        {
            string[] keyAndText = rawKeyAndText.Split('|');
            if (keyAndText.Length != 2)
            {
                throw new Exception("text data should like <key>|<text>");
            }
            return (keyAndText[0], keyAndText[1]);
        }

        public static void ValidateText(string key, string text)
        {
            if (key == null || text == null)
            {
                throw new Exception("text的key或text属性不能为null");
            }
            if (key == "" && text != "")
            {
                throw new Exception($"text  key为空, 但text:'{text}'不为空");
            }
        }


        public static bool IsIgnoreTag(string tagName)
        {
            return tagName == "##";
        }

        public static List<string> ParseTags(string rawTagStr)
        {
            if (string.IsNullOrWhiteSpace(rawTagStr))
            {
                return null;
            }
            var tags = new List<string>(rawTagStr.Split(',').Select(t => t.Trim().ToLower()).Where(t => !string.IsNullOrEmpty(t)));
            return tags.Count > 0 ? tags : null;
        }


        private const string TAG_UNCHECKED = "unchecked";

        public static bool IsUnchecked(Record rec)
        {
            return rec.Tags != null && rec.Tags.Count > 0 && rec.Tags.Contains(TAG_UNCHECKED);
        }

        public const string SimpleContainerSep = ",;|";

        public static string GetBeanSep(TBean type)
        {
            if (type.Tags != null && type.Tags.TryGetValue("sep", out var s) && !string.IsNullOrWhiteSpace(s))
            {
                return s;
            }
            return ((DefBean)type.Bean).Sep;
        }

        public static string GetTypeSep(TType type)
        {
            if (type.Tags != null && type.Tags.TryGetValue("sep", out var s) && !string.IsNullOrWhiteSpace(s))
            {
                return s;
            }

            return type.Apply(IsNotSepTypeVisitor.Ins) ? SimpleContainerSep : "";

        }

        public static bool IsCollectionEqual(List<DType> a, List<DType> b)
        {
            if (a.Count == b.Count)
            {
                for (int i = 0, n = a.Count; i < n; i++)
                {
                    if (!object.Equals(a[i], b[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        //public static string Data2String(DType data)
        //{
        //    var s = new StringBuilder();
        //    data.Apply(VisitorToString.Ins, s);
        //    return s.ToString();
        //}
    }
}
