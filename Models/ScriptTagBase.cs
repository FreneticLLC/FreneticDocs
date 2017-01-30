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
    public class ScriptTagBase : ScriptData
    {
        public override string GetName()
        {
            return Base;
        }

        public override string GetDataType()
        {
            return "TagBases";
        }

        public override string GetSearchables()
        {
            String t = Returns + "\n" + ReturnType + "\n";
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

        public string Base;

        public string Group;

        public string ReturnType;

        public string Returns;

        public string SourceLocation;

        public string Addon;

        public List<string> Warnings;

        public List<string> Notes;

        public bool ModifierOptional;

        public static string[] TAGBits = new string[] {
            "base", "group", "returntype", "returns", "addon", "warning", "note", "modifier"
        };

        public ScriptTagBase(Dictionary<string, List<StringBuilder>> opts, string source)
        {
            Base = opts["base"][0].ToString();
            Group = opts["group"][0].ToString();
            Addon = opts.ContainsKey("addon") ? opts["addon"][0].ToString() : "None";
            ReturnType = opts["returntype"][0].ToString();
            Returns = opts["returns"][0].ToString();
            ModifierOptional = opts.ContainsKey("modifier") && opts["modifier"][0].ToString() == "optional";
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
                    Console.WriteLine("Bad tagbase meta part: " + key + " for " + Base + " in " + source);
                }
            }
        }
    }
}
