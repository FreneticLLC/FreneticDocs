using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace FreneticDocs.Models
{
    public class ScriptExplanation
    {
        public string Name;

        public string Group;

        public string Description;

        public string Addon;

        public List<string> Warnings;

        public List<string> Notes;

        public string SourceLocation;

        public string Escape(string input)
        {
            return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        const string S_START = "&lt;@code&gt;";

        const string S_END = "&lt;@/code&gt;";

        public HtmlString GetDescription()
        {
            string t = Escape(Description).Replace("\n", "\n<br>");
            // .Replace("&lt;@code&gt;", "<pre class=\"brush: " + hl + "\">").Replace("&lt;@/code&gt;", "</pre>")
            int ind = t.IndexOf(S_START);
            while (ind >= 0)
            {
                string pref = t.Substring(0, ind);
                string rest = t.Substring(ind + S_START.Length);
                int endind = rest.IndexOf(S_END);
                if (endind >= 0)
                {
                    string mid = rest.Substring(0, endind);
                    string end = rest.Substring(endind + S_END.Length);
                    t = pref + "<pre class=\"brush: " + DocsStatic.Config["highlight"] + "\">" + mid.Replace("<br>", "") + "</pre>" + end;
                    ind = t.IndexOf("&lt;@code&gt;");
                }
                else
                {
                    break;
                }
            }
            return new HtmlString(t);
        }

        public static string[] TAGBits = new string[] {
            "name", "group", "description", "addon", "warning", "note"
        };

        public ScriptExplanation(Dictionary<string, List<StringBuilder>> opts, string source)
        {
            Name = opts["name"][0].ToString();
            Group = opts["group"][0].ToString();
            Description = opts["description"][0].ToString().Trim();
            Addon = opts.ContainsKey("addon") ? opts["addon"][0].ToString() : "None";
            Warnings = new List<string>();
            if (opts.ContainsKey("warning"))
            {
                foreach (StringBuilder sb in opts["warning"])
                {
                    Warnings.Add(sb.ToString());
                }
            }
            Notes = new List<string>();
            if (opts.ContainsKey("note"))
            {
                foreach (StringBuilder sb in opts["note"])
                {
                    Notes.Add(sb.ToString());
                }
            }
            SourceLocation = source;
            foreach (string key in opts.Keys)
            {
                if (!TAGBits.Contains(key))
                {
                    Console.WriteLine("Bad explanation meta part: " + key + " for " + Name + " in " + source);
                }
            }
        }
    }
}
