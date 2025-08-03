using System;
using System.Collections.Generic;
using camis.aggregator.data.Entities;
using camis.aggregator.domain.Admin;
using camis.aggregator.domain.Infrastructure;
using camis.aggregator.domain.Report;
using intapscamis.camis.domain.Report;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace camis.aggregator.web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connString = Configuration.GetConnectionString("aggregator_context");
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = ".ASPNetCoreSession";
                options.Cookie.Path = "/";
            });

            services.AddAntiforgery(opts =>
            {
                opts.Cookie.Name = ".ASPNetCoreSession";
                opts.Cookie.Path = "/";
            });

            services.AddDbContext<aggregatorContext>(options => 
                options.UseNpgsql(connString));

            services.AddMemoryCache();
            
            // Use AddNewtonsoftJson instead of AddJsonOptions
            services.AddMvc()
                .AddNewtonsoftJson(opts => 
                {
                    opts.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            services.AddCors();

            services.AddTransient<IUserActionService, UserActionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserFacade, UserFacade>();

            services.AddTransient<domain.Report.IReportService, domain.Report.ReportService>();
            services.AddTransient<domain.Report.IReportFacade, domain.Report.ReportFacade>();

            services.AddTransient<domain.LandBank.ILandBankFacade, domain.LandBank.LandBankFacade>();
            services.AddTransient<domain.LandBank.ILandBankService, domain.LandBank.LandBankService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var origins = this.Configuration.GetSection("API_Origins").Get<String[]>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            
            app.UseCors(builder => builder
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials()
                 .WithOrigins(origins));
                 
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRouting(); // Add routing middleware

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}