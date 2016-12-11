using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Html;

namespace FreneticDocs.Models
{
    public static class DocsStatic
    {
        public static Dictionary<string, string> Config = new Dictionary<string, string>();

        public static HtmlString SECTION_SEPARATOR = new HtmlString("</div><div class=\"section\">");

        public static HtmlString BULLET = new HtmlString("&#9899;");

        public static HtmlString CHECKMARK = new HtmlString("&#10003;");

        public static HtmlString BIG_X = new HtmlString("&#10008;");

        public static string Pad2Z(int input)
        {
            if (input < 10)
            {
                return "0" + input;
            }
            return input.ToString();
        }

        public static string DateNow()
        {
            DateTime dt = DateTime.Now;
            DateTime utc = dt.ToUniversalTime();
            TimeSpan rel = dt.Subtract(utc);
            string timezone = rel.TotalHours < 0 ? rel.TotalHours.ToString(): ("+" + rel.TotalHours);
            return dt.Year + "/" + Pad2Z(dt.Month) + "/" + Pad2Z(dt.Day) + " " + Pad2Z(dt.Hour) + ":" + Pad2Z(dt.Minute) + ":" + Pad2Z(dt.Second) + " UTC" + timezone + ":00";
        }

        public static Encoding Enc = new UTF8Encoding(false);
    }
}
