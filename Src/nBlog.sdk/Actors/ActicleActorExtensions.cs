using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using nBlog.sdk.Services;
using System;
using Toolbox.Actor.Host;
using Toolbox.Tools;

namespace nBlog.sdk.Actors
{
    public static class ActicleActorExtensions
    {
        public static IActorHost AddArticleServiceActors(this IActorHost actorHost, IServiceProvider serviceProvider)
        {
            actorHost.VerifyNotNull(nameof(actorHost));
            serviceProvider.VerifyNotNull(nameof(serviceProvider));

            actorHost.Register<IArticlePackageActor>(() => serviceProvider.GetRequiredService<IArticlePackageActor>());

            return actorHost;
        }

        public static IServiceCollection AddArticleServiceActorHost(this IServiceCollection services, int capacity = 10000)
        {
            services.VerifyNotNull(nameof(services));

            services.AddSingleton<IActorHost>(x =>
            {
                ILoggerFactory loggerFactory = x.GetRequiredService<ILoggerFactory>();

                IActorHost host = new ActorHost(capacity, loggerFactory);
                host.AddArticleServiceActors(x);

                return host;
            });

            return services;
        }

        public static IServiceCollection AddArticleServices(this IServiceCollection services)
        {
            services.VerifyNotNull(nameof(services));

            services.AddSingleton<IArticlePackageActor, ArticlePackageActor>();
            services.AddSingleton<IArticleStore, ArticleStore>();

            return services;
        }
    }
}