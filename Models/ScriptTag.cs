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
    public class ScriptTag
    {
        public string Name;

        public string Updated;

        public string Group;

        public string ReturnType;

        public string Returns;

        public List<string> Examples;

        public string SourceLocation;

        public string Addon;

        public List<string> Warnings;

        public List<string> Notes;

        public static string[] TAGBits = new string[] {
            "name", "updated", "group", "returntype", "returns", "addon", "example", "warning", "note"
        };

        public ScriptTag(Dictionary<string, List<StringBuilder>> opts, string source)
        {
            Name = opts["name"][0].ToString();
            Updated = opts["updated"][0].ToString();
            Group = opts["group"][0].ToString();
            Addon = opts.ContainsKey("addon") ? opts["addon"][0].ToString() : "None";
            ReturnType = opts["returntype"][0].ToString();
            Returns = opts["returns"][0].ToString();
            Examples = new List<string>();
            if (opts.ContainsKey("example"))
            {
                foreach (StringBuilder sb in opts["example"])
                {
                    Examples.Add(sb.ToString());
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
                    Console.WriteLine("Bad tag meta part: " + key + " for " + Name + " in " + source);
                }
            }
        }
    }
}
