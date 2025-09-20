using System;
using System.IO;
using intapscamis.camis.data.Entities;
using intapscamis.camis.domain.Admin;
using intapscamis.camis.domain.Documents;
using intapscamis.camis.domain.Farms;
using intapscamis.camis.domain.Infrastructure;
using intapscamis.camis.domain.LandBank;
using intapscamis.camis.domain.Projects;
using intapscamis.camis.domain.Projects.Workflows;
using intapscamis.camis.domain.Report;
using intapscamis.camis.domain.System.Addresses;
using intapscamis.camis.domain.Workflows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Hosting; 
using Microsoft.ApplicationInsights.AspNetCore;

namespace intapscamis.camis
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; } // Use modern interface

        public void ConfigureServices(IServiceCollection services)
        {
            // Add CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });
            // Updated for .NET 8 compatibility
            services.AddControllersWithViews(); // Replaces AddMvc()

            // AddSpaStaticFiles configuration remains the same
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            services.AddDistributedMemoryCache();
            services.AddDataProtection();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(180);
                options.Cookie.Name = ".ASPNetCoreSession";
                options.Cookie.Path = "/";
            });

            services.AddAntiforgery(opts =>
            {
                opts.Cookie.Name = ".ASPNetCoreSession";
                opts.Cookie.Path = "/";
            });

            // Updated JSON configuration for System.Text.Json
            services.AddControllers()
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });

            // Newtonsoft.Json for compatibility if needed
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                });

            services.AddApplicationInsightsTelemetry(Configuration);

            // Database configuration
            var connString = Configuration.GetConnectionString("camis_context");
            services.AddDbContext<CamisContext>(options =>
                options.UseNpgsql(connString,o=>o.UseNetTopologySuite())); // Simplified for .NET 8

            // Dependency injection
            InjectDependencies(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) // Updated parameter type
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // app.UseHsts();
            }

            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            
            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseDefaultFiles(options);
            app.UseCookiePolicy();
           
            app.UseRouting(); // Required for endpoint routing
            app.UseCors("AllowAll");
            app.UseSession();
            app.UseEndpoints(endpoints => // Updated endpoint configuration
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        private void InjectDependencies(IServiceCollection services)
        {
            services.AddTransient<ILookupService, LookupService>();

            services.AddTransient<IUserActionService, UserActionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserFacade, UserFacade>();

            services.AddTransient<IAddressFacade, AddressFacade>();
            services.AddTransient<IAddressService, AddressService>();

            services.AddTransient<IWorkflowFacade, WorkflowFacade>();
            services.AddTransient<IWorkflowService, WorkflowService>();

            services.AddTransient<IFarmsFacade, FarmsFacade>();
            services.AddTransient<IFarmsService, FarmsService>();

            services.AddTransient<ILandBankFacade, LandBankFacade>();
            services.AddTransient<ILandBankService, LandBankService>();

            services.AddTransient<IProjectFacade, ProjectFacade>();
            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<IProgressReportWorkflow, ProgressReportWorkflow>();

            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IDocumentFacade, DocumentFacade>();

            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<IReportFacade, ReportFacade>();
        }
    }
}