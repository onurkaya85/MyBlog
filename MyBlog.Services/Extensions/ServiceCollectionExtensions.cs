using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyBlog.Data.Abstract;
using MyBlog.Data.Concrete;
using MyBlog.Data.Concrete.EntityFramework.Contexts;
using MyBlog.Entities.Concrete;
using MyBlog.Services.Abstract;
using MyBlog.Services.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection LoadServices(this IServiceCollection serviceCollection,string connectionString)
        {
            serviceCollection.AddDbContext<MyBlogContext>(options => options.UseSqlServer(connectionString)
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));  //AsNoTracking in işini yapar. ister burda kullan ister sorgulara ekle

            serviceCollection.AddIdentity<User, Role>(opt=> 
            {
                opt.Password.RequireDigit = false; //şifrede rakam zorunlu mu?
                opt.Password.RequiredLength = 5;
                opt.Password.RequiredUniqueChars = 0; //uniq karakterlerden kaç tane olacak?
                opt.Password.RequireNonAlphanumeric = false; //özel karakterlerin bulunmasını sağlar
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+";
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<MyBlogContext>();

            // Bu ayarın süresi çok önemli.Her kullanıcı için o süre geçtiğinde db ye sorgu atacak
            serviceCollection.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.FromMinutes(15); //Gerçek projelerde uzun süre verilmeli
            });
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddScoped<ICategoryService, CategoryManager>();
            serviceCollection.AddScoped<IArticleService, ArticleManager>();
            serviceCollection.AddScoped<ICommentService, CommentManager>();

            return serviceCollection;
        }
    }
}
