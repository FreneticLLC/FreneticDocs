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
    public class ScriptCommand : ScriptData
    {
        public override string GetName()
        {
            return Name;
        }

        public override string GetDataType()
        {
            return "Commands";
        }

        public override string GetSearchables()
        {
            String t = Arguments + "\n" + Short + "\n" + Description + "\n";
            foreach (string s in Notes)
            {
                t += s + "\n";
            }
            foreach (string s in Warnings)
            {
                t += s + "\n";
            }
            t += SourceLocation + "\n" + Addon + "\n" + Group;
            return t;
        }

        public string Name;

        public string Arguments;

        public string Short;

        public string Updated;

        public string Group;

        public string Procedural;

        public string Minimum;

        public string Maximum;

        public string Description;

        public List<string> Nameds;

        public string Escape(string input)
        {
            return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public HtmlString GetDescriptionMultiLine()
        {
            return new HtmlString(Escape(Description.Trim()).Replace("\n", "\n<br>"));
        }

        public List<string> Examples;

        public string SourceLocation;

        public string Addon;

        public List<string> Warnings;

        public List<string> Notes;

        public List<string> Tags;

        public List<string> Saves;

        public static string[] CMDBits = new string[] {
            "name", "arguments", "short", "updated", "group", "procedural", "addon", "minimum", "maximum", "description", "example", "warning", "note", "tag", "save", "named"
        };

        public ScriptCommand(Dictionary<string, List<StringBuilder>> opts, string source)
        {
            Name = opts["name"][0].ToString();
            Arguments = opts["arguments"][0].ToString();
            Short = opts["short"][0].ToString();
            Updated = opts["updated"][0].ToString();
            Group = opts["group"][0].ToString();
            Procedural = opts.ContainsKey("procedural") ? opts["procedural"][0].ToString() : "false";
            Addon = opts.ContainsKey("addon") ? opts["addon"][0].ToString() : "None";
            Minimum = opts["minimum"][0].ToString();
            Maximum = opts["maximum"][0].ToString();
            Description = opts["description"][0].ToString();
            Examples = new List<string>();
            foreach (StringBuilder sb in opts["example"])
            {
                Examples.Add(sb.ToString());
            }
            Nameds = new List<string>();
            if (opts.ContainsKey("named"))
            {
                foreach (StringBuilder sb in opts["named"])
                {
                    Nameds.Add(sb.ToString());
                }
            }
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
            Saves = new List<string>();
            if (opts.ContainsKey("save"))
            {
                foreach (StringBuilder sb in opts["save"])
                {
                    Saves.Add(sb.ToString());
                }
            }
            Tags = new List<string>();
            if (opts.ContainsKey("tag"))
            {
                foreach (StringBuilder sb in opts["tag"])
                {
                    Tags.Add(sb.ToString());
                }
            }
            SourceLocation = source;
            foreach (string key in opts.Keys)
            {
                if (!CMDBits.Contains(key))
                {
                    Console.WriteLine("Bad command meta part: " + key + " for " + Name + " in " + source);
                }
            }
        }
    }
}
