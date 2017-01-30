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
    public class ScriptObject
    {
        public string Type;

        public string Group;

        public string SubType;

        public string Description;

        public string SourceLocation;

        public string Addon;

        public List<string> Warnings;

        public List<string> Notes;

        public static string[] TAGBits = new string[] {
            "type", "group", "subtype", "description", "addon", "warning", "note"
        };

        public ScriptObject(Dictionary<string, List<StringBuilder>> opts, string source)
        {
            Type = opts["type"][0].ToString();
            Group = opts["group"][0].ToString();
            Addon = opts.ContainsKey("addon") ? opts["addon"][0].ToString() : "None";
            SubType = opts["subtype"][0].ToString();
            Description = opts["description"][0].ToString();
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
                    Console.WriteLine("Bad object meta part: " + key + " for " + Type + " in " + source);
                }
            }
        }
    }
}
