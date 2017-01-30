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
    public class ScriptEvent : ScriptData
    {
        public override string GetName()
        {
            return EventNames[0];
        }

        public override string GetDataType()
        {
            return "Events";
        }

        public override string GetSearchables()
        {
            String t = "";
            foreach (string s in EventNames)
            {
                t += s + "\n";
            }
            t += Group + "\n" + Triggers + "\n" + Context + "\n" + Determinations + "\n";
            foreach (string s in Switches)
            {
                t += s + "\n";
            }
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

        public string[] EventNames;

        public string Updated;

        public string Group;

        public bool Cancellable;

        public string Triggers;

        public string Context;

        public string Determinations;

        public List<string> Switches;

        public string Addon;

        public List<string> Warnings;

        public List<string> Notes;

        public string SourceLocation;

        public string Escape(string input)
        {
            return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }

        public HtmlString GetContext()
        {
            return new HtmlString(Escape(Context).Replace("\n", "\n<br>"));
        }

        public HtmlString GetDeterminations()
        {
            return new HtmlString(Escape(Determinations).Replace("\n", "\n<br>"));
        }

        public static string[] TAGBits = new string[] {
            "events", "updated", "group", "cancellable", "triggers", "context", "determinations", "switch", "addon", "warning", "note"
        };

        public ScriptEvent(Dictionary<string, List<StringBuilder>> opts, string source)
        {
            EventNames = opts["events"][0].ToString().Trim().Split('\n');
            Updated = opts["updated"][0].ToString();
            Group = opts["group"][0].ToString();
            Cancellable = opts.ContainsKey("cancellable") ? opts["cancellable"][0].ToString().ToLowerInvariant() == "true" : false;
            Triggers = opts["triggers"][0].ToString();
            Context = opts["context"][0].ToString().Trim();
            Determinations = opts["determinations"][0].ToString().Trim();
            Addon = opts.ContainsKey("addon") ? opts["addon"][0].ToString() : "None";
            Switches = new List<string>();
            if (opts.ContainsKey("switch"))
            {
                foreach (StringBuilder sb in opts["switch"])
                {
                    Switches.Add(sb.ToString());
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
            SourceLocation = source;
            foreach (string key in opts.Keys)
            {
                if (!TAGBits.Contains(key))
                {
                    Console.WriteLine("Bad event meta part: " + key + " for " + EventNames[0] + " in " + source);
                }
            }
        }
    }
}
