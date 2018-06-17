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
        public static DiscordBot DiscBot;

        public static Dictionary<string, string> Config = new Dictionary<string, string>();

        public static HtmlString SECTION_SEPARATOR = new HtmlString("</div><div class=\"section\">");

        public static HtmlString BULLET = new HtmlString("&#9899;");

        public static HtmlString CHECKMARK = new HtmlString("&#10003;");

        public static HtmlString BIG_X = new HtmlString("&#10008;");

        public static HtmlString INTRO_PAGE = new HtmlString("Docs need a home page! ./config/docs_homepage.html");

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

        
        public static int LevenshteinDistance(string s, string t) {
            int n = s.Length;
            int m = t.Length;
            if (n == 0) {
                return m;
            }
            else if (m == 0) {
                return n;
            }
            int[] p = new int[n + 1]; // 'previous' cost array, horizontally
            int[] d = new int[n + 1]; // cost array, horizontally
            int[] _d; // placeholder to assist in swapping p and d
            // indexes into strings s and t
            int i; // iterates through s
            int j; // iterates through t
            char t_j; // jth character of t
            int cost; // cost
            for (i = 0; i <= n; i++) {
                p[i] = i;
            }
            for (j = 1; j <= m; j++) {
                t_j = t[j - 1];
                d[0] = j;
                for (i = 1; i <= n; i++) {
                    cost = s[i - 1] == t_j ? 0 : 1;
                    // minimum of cell to the left+1, to the top+1, diagonally left
                    // and up +cost
                    d[i] = Math.Min(Math.Max(d[i - 1] + 1, p[i] + 1), p[i - 1] + cost);
                }
                // copy current distance counts to 'previous row' distance counts
                _d = p;
                p = d;
                d = _d;
            }
            // our last action in the above loop was to switch d and p, so p now
            // actually has the most recent cost counts
            return p[n];
        }

        public static Encoding Enc = new UTF8Encoding(false);

        public static DocsMeta MetaInternal = new DocsMeta();

        public static DocsMeta Meta
        {
            get
            {
                lock (MetaInternal)
                {
                    return MetaInternal;
                }
            }
        }
    }

    public class DocsMeta
    {
        public List<ScriptCommand> Commands = new List<ScriptCommand>();

        public List<ScriptTag> Tags = new List<ScriptTag>();

        public List<ScriptObject> Objects = new List<ScriptObject>();

        public List<ScriptTagBase> TagBases = new List<ScriptTagBase>();

        public List<ScriptEvent> Events = new List<ScriptEvent>();

        public List<ScriptExplanation> Explanations = new List<ScriptExplanation>();

        public List<ScriptData> Searchable = new List<ScriptData>();

        public string Loaded = "?";

        public List<KeyValuePair<string, List<ScriptData>>> Search(string s)
        {
            s = s.ToLowerInvariant();
            List<KeyValuePair<string, List<ScriptData>>> toret = new List<KeyValuePair<string, List<ScriptData>>>();
            List<ScriptData> exact = new List<ScriptData>();
            toret.Add(new KeyValuePair<string, List<ScriptData>>("Exact", exact));
            List<ScriptData> strong = new List<ScriptData>();
            toret.Add(new KeyValuePair<string, List<ScriptData>>("Strong", strong));
            List<ScriptData> good = new List<ScriptData>();
            toret.Add(new KeyValuePair<string, List<ScriptData>>("Good", good));
            List<ScriptData> okay = new List<ScriptData>();
            toret.Add(new KeyValuePair<string, List<ScriptData>>("Okay", okay));
            List<ScriptData> backup = new List<ScriptData>();
            toret.Add(new KeyValuePair<string, List<ScriptData>>("Backup", backup));
            foreach (ScriptData sd in Searchable)
            {
                string sdName = sd.GetName().ToLowerInvariant();
                if (sdName == s)
                {
                    exact.Add(sd);
                    continue;
                }
                if (sdName.Contains(s))
                {
                    strong.Add(sd);
                    continue;
                }
                int ndist = DocsStatic.LevenshteinDistance(sdName, s);
                if (ndist < 1)
                {
                    strong.Add(sd);
                    continue;
                }
                string sdDesc = sd.GetSearchables().ToLowerInvariant();
                if (sdDesc.Contains(s))
                {
                    strong.Add(sd);
                    continue;
                }
                if (ndist < 2)
                {
                    good.Add(sd);
                    continue;
                }
                string[] descSplit = sdDesc.Replace("\r", "").Split('\n');
                int[] dists = new int[descSplit.Length];
                bool breakMe = false;
                for (int i = 0; i < descSplit.Length; i++)
                {
                    dists[i] = DocsStatic.LevenshteinDistance(s, descSplit[i]);
                    if (dists[i] < 3)
                    {
                        good.Add(sd);
                        breakMe = true;
                        break;
                    }
                }
                if (breakMe)
                {
                    continue;
                }
                if (ndist < 4)
                {
                    okay.Add(sd);
                    continue;
                }
                for (int i = 0; i < descSplit.Length; i++)
                {
                    if (dists[i] < 6)
                    {
                        okay.Add(sd);
                        breakMe = true;
                        break;
                    }
                }
                if (breakMe)
                {
                    continue;
                }
                 if (ndist < 7)
                {
                    backup.Add(sd);
                    continue;
                }
                for (int i = 0; i < descSplit.Length; i++)
                {
                    if (dists[i] < 10)
                    {
                        backup.Add(sd);
                        breakMe = true;
                        break;
                    }
                }
                if (breakMe)
                {
                    continue;
                }
            }
            return toret;
        }

        public void AddDocumentationSet(string[] docs, string source)
        {
            Console.WriteLine("Begin parsing set: " + source);
            for (int i = 0; i < docs.Length; i++)
            {
                string[] split = docs[i].Split(new char[] { '?' }, 2);
                string linesource = source + " at: " + split[0];
                string cline = split[1];
                if (cline.StartsWith("// <--[") && cline.EndsWith("]"))
                {
                    string type = cline.Substring("// <--[".Length, cline.Length - "// <--[]".Length);
                    Dictionary<string, List<StringBuilder>> opts = new Dictionary<string, List<StringBuilder>>();
                    StringBuilder current = new StringBuilder();
                    while ((++i) < docs.Length)
                    {
                        string[] sub_split = docs[i].Split(new char[] { '?' }, 2);
                        string sub_linesource = source + " at: " + sub_split[0];
                        string sub_cline = sub_split[1];
                        if (sub_cline.StartsWith("// <--[") && sub_cline.EndsWith("]"))
                        {
                            Console.WriteLine("Meta-within-meta in " + sub_linesource + " competing with meta in " + linesource);
                            break;
                        }
                        if (sub_cline.Equals("// -->"))
                        {
                            try
                            {
                                if (type == "command")
                                {
                                    ScriptCommand sd = new ScriptCommand(opts, linesource);
                                    Commands.Add(sd);
                                    Searchable.Add(sd);
                                }
                                else if (type == "tag")
                                {
                                    ScriptTag sd = new ScriptTag(opts, linesource);
                                    Tags.Add(sd);
                                    Searchable.Add(sd);
                                }
                                else if (type == "object")
                                {
                                    ScriptObject sd = new ScriptObject(opts, linesource);
                                    Objects.Add(sd);
                                    Searchable.Add(sd);
                                }
                                else if (type == "tagbase")
                                {
                                    ScriptTagBase sd = new ScriptTagBase(opts, linesource);
                                    TagBases.Add(sd);
                                    Searchable.Add(sd);
                                }
                                else if (type == "event")
                                {
                                    ScriptEvent sd = new ScriptEvent(opts, linesource);
                                    Events.Add(sd);
                                    Searchable.Add(sd);
                                }
                                else if (type == "explanation")
                                {
                                    ScriptExplanation sd = new ScriptExplanation(opts, linesource);
                                    Explanations.Add(sd);
                                    Searchable.Add(sd);
                                }
                                else
                                {
                                    Console.WriteLine("Presently unknown meta type: '" + type + "' in " + linesource);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Meta can't parse right in " + linesource + "::: " + ex.ToString());
                            }
                            break;
                        }
                        if (sub_cline.StartsWith("// @"))
                        {
                            string[] d = sub_cline.Substring("// @".Length).Split(new char[] { ' ' }, 2);
                            current = new StringBuilder();
                            if (opts.ContainsKey(d[0].ToLowerInvariant()))
                            {
                                opts[d[0].ToLowerInvariant()].Add(current);
                            }
                            else
                            {
                                opts[d[0].ToLowerInvariant()] = new List<StringBuilder>() { current };
                            }
                            if (d.Length > 1)
                            {
                                current.Append(d[1]);
                            }
                        }
                        else 
                        {
                            current.Append("\n" + sub_cline.Substring("// ".Length));
                        }
                    }
                }
            }
        }
    }
}
