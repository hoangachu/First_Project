using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoang_Project_one.Hubs;
namespace Hoang_Project_one
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment { get; set; }


        public static string connectionString = "";
        /*"Server=ADMIN;Database=ChatBot;Trusted_Connection=True;MultipleActiveResultSets=true"*/
        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
            connectionString = Configuration.GetConnectionString("DefaultConnection").ToString();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();

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
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapControllerRoute(
                    name: "Login",
                    pattern: "{controller=Home}/{action=Index}/{id}");
                endpoints.MapControllerRoute("Login", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
