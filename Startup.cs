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
        public Startup(IHostingEnvironment env)
        {
            Console.WriteLine("Load system...");
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
                DocsStatic.AddDocumentationSet(lds.Docs.ToArray(), lds.ID);
            }

            Console.WriteLine("Ready!");

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

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
