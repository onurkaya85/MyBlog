using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyBlog.Entities.Concrete;
using MyBlog.Services.AutoMapper.Profiles;
using MyBlog.Services.Extensions;
using MyBlog.Shared.Utilities.Extensions;
using MyBlog.Web.AutoMapper.Profiles;
using MyBlog.Web.Filters;
using MyBlog.Web.Helpers.Abstract;
using MyBlog.Web.Helpers.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyBlog.Web
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
            services.AddRazorPages();
            services.Configure<AboutUsPageInfo>(Configuration.GetSection("AboutUsPageInfo"));
            services.Configure<WebsiteInfo>(Configuration.GetSection("WebsiteInfo"));
            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.Configure<ArticleRightSideBarWidgetOptions>(Configuration.GetSection("ArticleRightSideBarWidgetOptions"));

            services.ConfigureWritable<AboutUsPageInfo>(Configuration.GetSection("AboutUsPageInfo"));
            services.ConfigureWritable<WebsiteInfo>(Configuration.GetSection("WebsiteInfo"));
            services.ConfigureWritable<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.ConfigureWritable<ArticleRightSideBarWidgetOptions>(Configuration.GetSection("ArticleRightSideBarWidgetOptions"));

            services.AddControllersWithViews(options => 
            {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(v=> "Bu alan bo� ge�ilmemelidir"); //Select2 kullan�l�rken verdi�i mesaj� t�rk�eye �evirmi� olduk
                options.Filters.Add<MvcExceptionFilter>();
            })
             .AddRazorRuntimeCompilation()
             .AddJsonOptions(opt=> 
             {
                 opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                 opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve; //relation nesneler i�in
             }).AddNToastNotifyToastr();

            services.AddSession();
            services.AddAutoMapper(typeof(CategoryProfile),typeof(ArticleProfile),typeof(UserProfile),typeof(ViewModelsProfile),typeof(CommentProfile));
            
            services.LoadServices(connectionString:Configuration.GetConnectionString("LocalDb"));
            services.AddScoped<IImageHelper, ImageHelper>();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = new PathString("/Admin/Auth/Login");
                options.LogoutPath = new PathString("/Admin/Auth/Logout");
                options.Cookie = new CookieBuilder
                {
                    Name = "MyBlog",
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    SecurePolicy = CookieSecurePolicy.SameAsRequest //CookieSecurePolicy.Always, //ger�ek projelerde Always olmal�
                };
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = System.TimeSpan.FromDays(7);
                options.AccessDeniedPath = new PathString("/Admin/Auth/AccessDenied");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); 
            app.UseAuthorization();
            app.UseNToastNotify();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                        name: "Admin",
                        areaName: "Admin",
                        pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapRazorPages();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
