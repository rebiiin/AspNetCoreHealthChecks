using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using WebApp_1.Infrastructure;

namespace WebApp_1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllersWithViews();

             
            //Health Checks for sqlserver connection


            services.AddHealthChecks().AddSqlServer(
              connectionString: Configuration["ConnectionStrings:DefaultConnection"],
              healthQuery: "SELECT 1;",
              name: "Database WebApp1",
              tags: new string[] { "db", "sql", "sqlserver" });


            //Health Checks for Azure storage

            //services.AddHealthChecks()
            //    .AddAzureBlobStorage(connectionString: Configuration["AzureStorage:DefaultConnection"], name:"Azure Storage", tags: new string[] { "Storage", "blob" });

            services.AddHealthChecks()
                .AddProcessAllocatedMemoryHealthCheck(1024, name: "Memory", tags: new string[] { "memory" });


            //Health Checks for an external endpoint

            services.AddHealthChecks().AddUrlGroup(new Uri("https://localhost:3001/weatherforecast"), name: "Api endpoint", tags: new string[] { "endpoint", "api", "uri" })
                .AddUrlGroup(new Uri("https://randomuser.me/api/"), name: "Api endpoint 2", tags: new string[] { "endpoint", "api", "uri" });


            services.AddHealthChecks()
                .AddCheck(name: "CustomHealthChecks",new CustomHealthCheck("https://api.quotable.io"),tags: new string[] { "customEndpoint", "api", "quot" });




            //services.AddHealthChecks()

            //  .AddCheck("Database WebApp2", () => HealthCheckResult.Healthy("Database WebApp1 working fine"), tags: new[] { "db", "sql", "sqlserver" })

            //  .AddCheck("Azure Storage", () => HealthCheckResult.Unhealthy("Azure Storage is unhealthy!"), tags: new[] { "Storage", "blob" })

            //  .AddCheck("Api endpoint", () => HealthCheckResult.Degraded("Api endpoint is very slow or unstable."), tags: new[] { "endpoint", "api", "uri" });





        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();



           


            app.UseEndpoints(endpoints =>
            {
               

                endpoints.MapHealthChecks("/webapp1_health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                      
                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
