using ForgeSample.Application.Hubs;
using ForgeSample.Application.Interfaces;
using ForgeSample.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection; 

namespace ForgeSample
{

    /// <summary>
    /// Class Startup.
    /// </summary>
    public class Startup
    {

        /// <summary>
        /// This method gets called by the runtime. 
        /// Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc(options => options.EnableEndpointRouting = false)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0).AddNewtonsoftJson();
            services.AddSignalR().AddNewtonsoftJsonProtocol(opt =>
            {
                opt.PayloadSerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            });
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient<I2leggedAuthenticationService, TwoLeggedAuthenticationService>();
            services.AddTransient<I3leggedAuthenticationService, ThreeLeggedAuthenticationService>();
            services.AddTransient<ModelDerivativeHub>();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }


        /// <summary>
        ///This method gets called by the runtime. Use this method \
        ///to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseSession();
                
            app.UseEndpoints(routes =>
            {
                routes.MapHub<ModelDerivativeHub>("/api/signalr/modelderivative");
            });
            app.UseCors(options =>
                options.WithOrigins(Utils.Utils.GetAppSetting("FORGE_WEBHOOK_URL")).AllowAnyMethod()
            );
            app.UseMvc();
        }
    }
}
