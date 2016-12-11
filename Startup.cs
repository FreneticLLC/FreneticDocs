using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FreneticDocs.Models;

namespace FreneticDocs
{
    public class Startup
    {
        public DocsMeta LoadMeta()
        {
            Console.WriteLine("Load meta...");
            DocsMeta meta = new DocsMeta();
            DocsStatic.Config.Clear();
            string[] lines = File.ReadAllText("./config/docs.cfg").Replace('\r', '\n').Split('\n');
            foreach (string l in lines)
            {
                if (l.StartsWith("#"))
                {
                    continue;
                }
                string[] dat = l.Split(new char[] { ':' }, 2);
                if (dat.Length == 2)
                {
                    DocsStatic.Config[dat[0].ToLower()] = dat[1].Trim();
                }
            }

            Console.WriteLine("Parse remote docs...");
            string[] urls = DocsStatic.Config["urls"].Split('|');
            List<LoadSequencer> ldss = new List<LoadSequencer>();
            foreach (string URL in urls)
            {
                string u = URL.Trim();
                if (u.Length < 5)
                {
                    continue;
                }
                string[] sd = u.Split(new char[] { '/' }, 2);
                if (sd.Length != 2)
                {
                    continue;
                }
                LoadSequencer lds = new LoadSequencer() { ID = sd[0] };
                Console.WriteLine("Start for " + sd[1]);
                lds.Begin(sd[1]);
                ldss.Add(lds);
            }

            foreach (LoadSequencer lds in ldss)
            {
                lds.MRE.WaitOne();
                meta.AddDocumentationSet(lds.Docs.ToArray(), lds.ID);
            }

            Console.WriteLine("Ready!");
            return meta;
        }

        public Startup(IHostingEnvironment env)
        {
            DocsStatic.Meta = LoadMeta();

            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Logger/Console
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Errors: Development vs. Prod
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/ErrorInternal");
            }

            // 404's
            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/Error404"; 
                    await next();
                }
            });

            // Static Files (wwwroot)
            app.UseStaticFiles();

            // Final launch as MVC.
            app.UseMvc((routes) =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
