using CITS.EduSuite.Business.Models.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CITS.EduSuite.Business
{
    public static class StringExtension
    {
        public static T Deserialize<T>(this string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringReader textReader = new StringReader(toDeserialize);
            return (T)xmlSerializer.Deserialize(textReader);
        }

        public static string Serialize<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            StringWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, toSerialize);
            return textWriter.ToString();
        }

        public static string VerifyData(this string Data)
        {
            if (Data != null && Data != "")
            {
                Data = "%" + Data + "%";
            }
            else
            {
                Data = "%";
            }
            return Data;
        }
        public static decimal roundTo(decimal n, int digits)
        {
            bool negative = false;
            if (n < 0)
            {
                negative = true;
                n = n * -1;
            }
            decimal multiplicator = Convert.ToDecimal(Math.Pow(10, digits));
            n = Convert.ToDecimal((n * multiplicator));
            n = (Math.Round(n) / multiplicator);
            if (negative)
            {
                n = (n * -1);
            }
            return Convert.ToDecimal(n);
        }

        public static IEnumerable<IEnumerable<T>> FindConsecutiveGroups<T>(this IEnumerable<T> sequence, Predicate<T> predicate, int count)
        {
            IEnumerable<T> current = sequence;

            while (current.Count() > count)
            {
                IEnumerable<T> window = current.Take(count);

                if (window.Where(x => predicate(x)).Count() >= count)
                    yield return window;

                current = current.Skip(1);
            }
        }

        public static string ListToXmlString<T>(List<T> dataList, string[] cols)
        {
            StringBuilder sbXml = new StringBuilder();
            sbXml.Append("<ROOT>");
            foreach (Object data in dataList)
            {
                Type t = data.GetType();

                sbXml.Append("<DATA ");
                foreach (PropertyInfo pi in t.GetProperties())
                {
                    if (cols.Length == 0 || cols.Contains(pi.Name))
                    {
                        var value = (pi.GetValue(data, null)??"").ToString();
                        value = (pi.PropertyType == typeof(DateTime?) || pi.PropertyType == typeof(DateTime)) && pi.GetValue(data, null) != null ? ((DateTime)(pi.GetValue(data, null))).ToString("yyyy-MM-dd HH:mm:ss.fff") : value;
                        sbXml.Append(pi.Name + "= '" + value + "' ");
                    }
                }
                sbXml.Append(" />");
            }


            sbXml.Append("</ROOT>");
            return sbXml.ToString();
        }

        public static string NumberToPositionString(int n)
        {
            string text = "";
            text = n.ToString();
            switch (n)
            {
                case 1:
                    text = text + EduSuiteUIResources.BlankSpace + "st";
                    break;
                case 2:
                    text = text + EduSuiteUIResources.BlankSpace + "nd";
                    break;
                case 3:
                    text = text + EduSuiteUIResources.BlankSpace + "rd";
                    break;
                default:
                    text = text + EduSuiteUIResources.BlankSpace + "th";
                    break;
            }
            return text;
        }

        public static int GetWeekNumberOfMonth(DateTime date)
        {
            DateTime minDate = new DateTime(1900, 1, 1);
            int MonthCount = Math.Abs((date.Month - minDate.Month) + 12 * (date.Year - minDate.Year));
            DateTime newDate = minDate.AddMonths(MonthCount);
            int WeekCount = ((int)((date - newDate).TotalDays / 7)) + 1;
            return WeekCount;
        }

    }
}
