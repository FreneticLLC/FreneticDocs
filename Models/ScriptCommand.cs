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

namespace FreneticDocs.Models
{
    public class ScriptCommand
    {
        public string Name;

        public string SourceLocation;

        public ScriptCommand(Dictionary<string, StringBuilder> opts, string source)
        {
            Name = opts["name"].ToString();
            SourceLocation = source;
        }
    }
}
