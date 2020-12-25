using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using nBlog.sdk.Client;
using nBlog.sdk.Store;
using NBlog.Server.Application;
using NBlog.Server.Services;
using System;
using System.Net.Http;

namespace NBlog.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHealthChecks();

            services.AddSingleton<StateCacheService>();
            services.AddSingleton<DirectoryService>();
            services.AddSingleton<ArticleService>();

            services.AddHttpClient<IDirectoryClient, DirectoryClient>((service, httpClient) => setBlogStoreOptions(service, httpClient));
            services.AddHttpClient<IArticleClient, ArticleClient>((service, httpClient) => setBlogStoreOptions(service, httpClient));
            services.AddHttpClient<IContactRequestClient, ContactRequestClient>((service, httpClient) => setBlogStoreOptions(service, httpClient));



            void setBlogStoreOptions(IServiceProvider service, HttpClient httpClient)
            {
                Option option = service.GetRequiredService<Option>();
                httpClient.BaseAddress = new Uri(option.Store.Url);
                httpClient.DefaultRequestHeaders.Add(StoreConstants.ApiKeyName, option.Store.ApiKey);
            }
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}