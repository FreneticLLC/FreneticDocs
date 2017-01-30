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

        public static DocsMeta Meta = new DocsMeta();
    }

    public class DocsMeta
    {
        public List<ScriptCommand> Commands = new List<ScriptCommand>();

        public List<ScriptTag> Tags = new List<ScriptTag>();

        public List<ScriptTagBase> TagBases = new List<ScriptTagBase>();

        public List<ScriptEvent> Events = new List<ScriptEvent>();

        public List<ScriptExplanation> Explanations = new List<ScriptExplanation>();

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
                                    Commands.Add(new ScriptCommand(opts, linesource));
                                }
                                else if (type == "tag")
                                {
                                    Tags.Add(new ScriptTag(opts, linesource));
                                }
                                else if (type == "tagbase")
                                {
                                    TagBases.Add(new ScriptTagBase(opts, linesource));
                                }
                                else if (type == "event")
                                {
                                    Events.Add(new ScriptEvent(opts, linesource));
                                }
                                else if (type == "explanation")
                                {
                                    Explanations.Add(new ScriptExplanation(opts, linesource));
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
