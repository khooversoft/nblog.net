using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nBlog.sdk.Actors;
using nBlog.sdk.Actors.Directory;
using nBlog.sdk.Store;
using Toolbox.Actor.Host;
using Toolbox.Tools;

namespace nBlog.sdk
{
    public static class nBlogExtensions
    {
        public static IServiceCollection AddArticleServiceActorHost(this IServiceCollection services, int capacity = 10000)
        {
            services.VerifyNotNull(nameof(services));

            services.AddSingleton<IActorHost>(x =>
            {
                ILoggerFactory loggerFactory = x.GetRequiredService<ILoggerFactory>();

                IActorHost host = new ActorHost(capacity, loggerFactory);
                host.Register<IArticlePackageActor>(() => x.GetRequiredService<IArticlePackageActor>());
                host.Register<IDirectoryActor>(() => x.GetRequiredService<IDirectoryActor>());

                return host;
            });

            return services;
        }

        public static IServiceCollection AddArticleServices(this IServiceCollection services)
        {
            services.VerifyNotNull(nameof(services));

            services.AddTransient<IArticlePackageActor, ArticlePackageActor>();
            services.AddTransient<IDirectoryActor, DirectoryActor>();

            services.AddSingleton<IArticleStoreService, ArticleStoreService>();
            services.AddSingleton<IDirectoryService, DirectoryService>();

            services.AddSingleton<IArticleStore, ArticleStore>();
            services.AddSingleton<IDirectoryStore, DirectoryStore>();
            services.AddSingleton<IContactRequestStore, ContactRequestStore>();

            return services;
        }
    }
}