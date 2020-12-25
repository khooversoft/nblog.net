using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using nBlog.sdk;
using nBlog.sdk.Store;
using nBlog.Store.Application;
using nBlog.Store.Middleware;
using NSwag;
using System;
using Toolbox.Azure.DataLake;

namespace nBlog.Store
{
    public class Startup
    {
        private const string _policyName = "defaultPolicy";

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Setup ADLS services
            services.AddSingleton<IDataLakeStore, DataLakeStore>();
            services.AddSingleton<IArticleStore, ArticleStore>();

            // Setup Actor Service
            services.AddArticleServiceActorHost();

            // Register Actors
            services.AddArticleServices();

            // Swagger
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Title = "Path Finder - Links and Metadata";
                    document.Info.Description = "API Server for Path Finder services";
                    document.Schemes = new[] { OpenApiSchema.Http | OpenApiSchema.Https };
                };
            });

            // CORS
            services.AddCors(x => x.AddPolicy(_policyName, builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetPreflightMaxAge(TimeSpan.FromHours(1));
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(_policyName);
            app.UseAuthorization();
            app.UseMiddleware<ApiKeyMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}