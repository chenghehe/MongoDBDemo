using Identity.MongoDBCore.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoPractice.Host.Models;
using MongoPractice.Host.Services;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MongoPractice.Host
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region Identity for Mongo Configure

            var settings = Configuration.GetSection("ConnectionStrings").Get<MongoDbSettings>();
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });
            services.AddSingleton(settings);
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>(settings.MongoConnection, settings.DataBase)
                    .AddDefaultTokenProviders();

            services.AddAuthorization(option =>
            {
                option.AddPolicy("管理员", polic => polic.RequireRole("管理员", "admin"));//添加基于policy策略的权限授权
            });

            #endregion

            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            //注入swagger
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info
                {
                    Title = "用户",
                    Description = "文章系统的前台站点接口：\n<h3>注册---》登录---》发布文章---》点赞/评论文章</h3>",
                    Version = "v1",
                });
                s.SwaggerDoc("admin", new Info
                {
                    Title = "管理员",
                    Description = "管理员接口：\n<h3>分配用户角色、管理文章、管理文章分类</h3>",
                    Version = "v1",
                });

                var basePath = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                var xmlPath = System.IO.Path.Combine(basePath, "MongoPractice.Host.xml");
                s.IncludeXmlComments(xmlPath);
            });

            #region 服务注入
            services.AddScoped<ProductService>();
            services.AddScoped<CategoryService>();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "用户");
                c.SwaggerEndpoint("/swagger/admin/swagger.json", "管理员");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}