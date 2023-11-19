using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Helpers;
using WebApi.Services;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace WebApi
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
            
            var appConnectionstr = Configuration.GetSection("Connectionstr");
            services.Configure<Connection>(appConnectionstr);
            var con1 = appConnectionstr.Get<Connection>();

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            });

          
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            services.AddControllers();          
            services.AddScoped<IBlogsService, BlogsService>();           
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
           
            // Configure the HTTP request pipeline.           

            app.UseRouting();

            app.UseAuthorization();
            app.UseEndpoints(x => x.MapControllers());
        }
    }
}
