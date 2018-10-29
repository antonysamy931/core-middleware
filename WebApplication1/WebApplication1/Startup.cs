using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiddlewareHandler;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;
using WebApplication1.CustomSwager;
using Microsoft.Extensions.PlatformAbstractions;

namespace WebApplication1
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.
                AddMvcCore().
                AddJsonFormatters().
                AddXmlSerializerFormatters();

            services.AddMvc();
            //https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/README.md
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.OperationFilter<CustomOperationFilter>();                
                //Determine base path for the application.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                c.DocumentFilter<CustomDocumentFilter>();
                //Set the comments path for the swagger json and ui.
                c.IncludeXmlComments(basePath + "\\WebApplication1.xml");
                //c.OrderActionsBy((apiDesc) => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseVaidateMiddleware(Configuration);

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");               
            });

            app.UseMvc();
        }
    }
}
