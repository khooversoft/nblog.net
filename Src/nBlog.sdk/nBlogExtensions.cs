using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nBlog.sdk.Actors;
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

                return host;
            });

            return services;
        }

        public static IServiceCollection AddArticleServices(this IServiceCollection services)
        {
            services.VerifyNotNull(nameof(services));

            services.AddTransient<IArticlePackageActor, ArticlePackageActor>();
            services.AddSingleton<IArticleStoreService, ArticleStoreService>();
            services.AddSingleton<IArticleStore, ArticleStore>();

            return services;
        }
    }
}