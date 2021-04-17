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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

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
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthentication("DemoSecurityScheme")
    .AddCookie("DemoSecurityScheme", options =>
    {
        options.AccessDeniedPath = new PathString("/ChatBot/ChatPreview");
        options.Cookie = new CookieBuilder
        {
            //Domain = "",
            HttpOnly = true,
            Name = ".aspNetCoreDemo.Security.Cookie",
            Path = "/",
            SameSite = SameSiteMode.Lax,
            SecurePolicy = CookieSecurePolicy.SameAsRequest
        };
        options.Events = new CookieAuthenticationEvents
        {
            OnSignedIn = context =>
            {
                Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                    "OnSignedIn", context.Principal.Identity.Name);
                return Task.CompletedTask;
            },
            OnSigningOut = context =>
            {
                Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                    "OnSigningOut", context.HttpContext.User.Identity.Name);
                return Task.CompletedTask;
            },
            OnValidatePrincipal = context =>
            {
                Console.WriteLine("{0} - {1}: {2}", DateTime.Now,
                    "OnValidatePrincipal", context.Principal.Identity.Name);
                return Task.CompletedTask;
            }
        };
        //options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.LoginPath = new PathString("/Home/Login");
        options.ReturnUrlParameter = "RequestPath";
        options.SlidingExpiration = true;
    });
            
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
            app.UseAuthentication();
        
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
