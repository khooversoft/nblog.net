using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Model;
using Toolbox.Services;
using Toolbox.Tools;

namespace nBlog.Store.Application
{
    public class OptionBuilder
    {
        public OptionBuilder() { }

        public string[]? Args { get; set; }

        public OptionBuilder SetArgs(params string[] args)
        {
            Args = args.ToArray();
            return this;
        }

        public Option Build()
        {
            // Look for switches in the model
            string[] switchNames = typeof(Option).GetProperties()
                .Where(x => x.PropertyType == typeof(bool))
                .Select(x => x.Name)
                .ToArray();

            // Add "=true" for all switches that don't have this already
            string[] args = (Args ?? Array.Empty<string>())
                .Select(x => switchNames.Contains(x, StringComparer.OrdinalIgnoreCase) ? x + "=true" : x)
                .ToArray();

            string? environment = null;
            string? secretId = null;
            Option? option = null;

            // Because ordering or placement on critical configuration can different, loop through a process
            // of building the correct configuration.  Pattern cases below are in priority order.
            while (true)
            {
                option = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .Func(x => GetEnvironmentConfig(environment) switch { Stream v => x.AddJsonStream(v), _ => x })
                    .Func(x => secretId.ToNullIfEmpty() switch { string v => x.AddUserSecrets(v), _ => x })
                    .AddCommandLine(args ?? Array.Empty<string>())
                    .Build()
                    .Bind<Option>();

                switch (option)
                {
                    case Option v when !v.Environment.IsEmpty() && environment == null:
                        environment = v.Environment;
                        continue;

                    case Option v when !v.SecretId.IsEmpty() && secretId == null:
                        secretId = v.SecretId;
                        continue;
                }

                break;
            };

            option.Verify();
            option = option with { SecretFilter = new SecretFilter(new[] { option.Store.AccountKey }), RunEnvironment = option.Environment.ToEnvironment() };

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
