using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace WebApp_2
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

            services.AddHealthChecks()

              .AddCheck("Database WebApp2", () => HealthCheckResult.Healthy("Database WebApp1 working fine"), tags: new[] { "db", "sql", "sqlserver" })

              .AddCheck("Azure Storage", () => HealthCheckResult.Unhealthy("Azure Storage is unhealthy!"), tags: new[] { "Storage", "blob" })

              .AddCheck("Api endpoint", () => HealthCheckResult.Degraded("Api endpoint is very slow or unstable."), tags: new[] { "endpoint", "api", "uri" });

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

                endpoints.MapHealthChecks("/webapp2_health", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    // Predicate = (hc) => hc.Tags.Contains("Storage") || hc.Tags.Contains("api") || hc.Tags.Contains("sql"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,

                });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
