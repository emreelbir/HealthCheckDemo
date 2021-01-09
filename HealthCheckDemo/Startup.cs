using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HealthCheckDemo.Data;
using HealthCheckDemo.HealthCheck;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheckDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHealthChecks()
                //.AddCheck("Foo Services", () =>
                //    HealthCheckResult.Degraded("The check of the foo did not work well."),new[]{"service"})
                //.AddCheck("Bar Services", () =>
                //HealthCheckResult.Healthy("The check of the bar service working."),new[] {"service"})
                .AddCheck<ResponseTimeHealthCheck>("Network speed test", null, new[] { "service" })
                .AddCheck("Database", () =>
                HealthCheckResult.Healthy("The check of the database worked."), new[] {"database","sql"});

            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<ResponseTimeHealthCheck>();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/quickhealt", new HealthCheckOptions()
                {
                    Predicate = _ => false
                });
                endpoints.MapHealthChecks("/hc/services", new HealthCheckOptions()
                {
                    Predicate = x => x.Tags.Contains("service"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/hc",new HealthCheckOptions()
                {
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
