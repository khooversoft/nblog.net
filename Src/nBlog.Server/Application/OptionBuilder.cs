using Microsoft.Extensions.Configuration;
using NBlog.Server.Application;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Toolbox.Extensions;
using Toolbox.Model;
using Toolbox.Services;
using Toolbox.Tools;

namespace nBlogCmd.Application
{
    internal class OptionBuilder
    {
        public string[]? Args { get; set; }

        public OptionBuilder SetArgs(params string[] args) => this.Action(x => x.Args = args);

        public Option Build()
        {
            string[] switchNames = typeof(Option).GetProperties()
                .Where(x => x.PropertyType == typeof(bool))
                .Select(x => x.Name)
                .ToArray();

            string[] args = (Args ?? Array.Empty<string>())
                .Select(x => switchNames.Contains(x, StringComparer.OrdinalIgnoreCase) ? x + "=true" : x)
                .ToArray();

            string? environment = null;
            string? secretId = null;
            Option? option = null;

            while (true)
            {
                option = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .Func(x => GetEnvironmentConfig(environment) switch { Stream v => x.AddJsonStream(v), _ => x })
                        .Func(x => secretId.ToNullIfEmpty() switch { string v => x.AddUserSecrets(v), _ => x })
                        .AddCommandLine(Args ?? Array.Empty<string>())
                        .Build()
                        .Bind<Option>();

                switch (option)
                {
                    case Option v when !v.Environment.IsEmpty() && environment == null:
                        environment = v.Environment;
                        continue;

                    case Option v when v.SecretId.ToNullIfEmpty() != null && secretId == null:
                        secretId = v.SecretId;
                        continue;
                }

                break;
            }

            option = option with { RunEnvironment = option.Environment.ToEnvironment() };
            option.Verify();

            return option;
        }

        private Stream? GetEnvironmentConfig(string? environment)
        {
            if (environment.IsEmpty()) return null;

            string resourceId = environment
                .ToEnvironment()
                .ToResourceId();

            return Assembly.GetAssembly(typeof(OptionBuilder))
                !.GetManifestResourceStream(resourceId)
                .VerifyNotNull($"{resourceId} not found");
        }
    }
}
