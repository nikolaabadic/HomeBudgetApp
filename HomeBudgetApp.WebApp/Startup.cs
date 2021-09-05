using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeBudgetApp.Data.UnitOfWork;
using HomeBudgetApp.Data.UnitOfWork.Implementation;
using HomeBudgetApp.Domain;
using HomeBudgetApp.WebApp.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeBudgetApp.WebApp
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
            services.AddDistributedMemoryCache();
            services.AddSession(opt => opt.IdleTimeout = TimeSpan.FromMinutes(10));
            services.AddControllersWithViews();
            services.AddScoped<IUnitOfWork, HomeBudgetUnitOfWork>();
            services.AddScoped<IAdminUnitOfWork, AdminUnitOfWork>();
            services.AddScoped<LoggedInUser>();
            services.AddScoped<NotLoggedIn>();
            services.AddScoped<AdminLoggedIn>();
            services.AddScoped<AdminNotLoggedIn>();
            services.AddDbContext<HomeBudgetContext>();
            services.AddDbContext<AdminContext>();
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
            app.UseSession();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
