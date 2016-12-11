using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO.Compression;

namespace FreneticDocs.Models
{
    public class LoadSequencer
    {
        public ManualResetEvent MRE = new ManualResetEvent(false);

        public List<string> Docs;

        public string URL;

        public string ID;

        public void Begin(string _url)
        {
            URL = _url;
            MRE.Reset();
            Task.Factory.StartNew(() => 
            {
                try
                {
                    Docs = new List<string>();
                    HttpClient client = new HttpClient();
                    client.Timeout = new TimeSpan(0, 2, 0);
                    byte[] t = client.GetByteArrayAsync(URL).Result;
                    MemoryStream mst = new MemoryStream(t);
                    ZipStorer zs = ZipStorer.Open(mst, FileAccess.Read);
                    foreach (ZipStorer.ZipFileEntry zfe in zs.ReadCentralDir())
                    {
                        if (zfe.FilenameInZip.EndsWith(".java") || zfe.FilenameInZip.EndsWith(".cs"))
                        {
                            MemoryStream ms = new MemoryStream();
                            zs.ExtractFile(zfe, ms);
                            byte[] b = ms.ToArray();
                            ms.Dispose();
                            string[] lines = DocsStatic.Enc.GetString(b).Replace("\r", "").Split('\n');
                            for (int l = 0; l < lines.Length; l++)
                            {
                                string trimmed = lines[l].Trim();
                                if (trimmed.StartsWith("// "))
                                {
                                    Docs.Add(zfe.FilenameInZip + " on line " + (l + 1) + "?" + trimmed);
                                }
                            }
                        }
                    }
                    Console.WriteLine("Success for " + URL);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed " + URL + ": " + ex.ToString());
                }
                MRE.Set();
            });
        }
    }
}
