using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace nBlogCmd.Application
{
    public class OptionBuilder
    {
        public string[]? Args { get; set; }

        public OptionBuilder SetArgs(params string[] args) => this.Action(x => x.Args = args);

        public Option Build()
        {
            string[] switchNames = typeof(Option).GetProperties()
                .Where(x => x.PropertyType == typeof(bool))
                .Select(x => x.Name)
                .ToArray();

            string[] args = (Args ?? new[] { "help" })
                .Select(x => switchNames.Contains(x, StringComparer.OrdinalIgnoreCase) ? x + "=true" : x)
                .ToArray();

            string? configFile = null;
            string? secretId = null;
            string? accountKey = null;
            Option? option = null;

            while (true)
            {
                option = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .Func(x => configFile.ToNullIfEmpty() switch { string v => x.AddJsonFile(configFile), _ => x })
                        .Func(x => secretId.ToNullIfEmpty() switch { string v => x.AddUserSecrets(v), _ => x })
                        .AddCommandLine(args.Concat(accountKey switch { string v => new[] { createAccountKeyCommand(accountKey) }, _ => Enumerable.Empty<string>() }).ToArray())
                        .Build()
                        .Bind<Option>();

                switch (option)
                {
                    case Option v when v.Help:
                        return new Option { Help = true };

                    case Option v when v.ConfigFile.ToNullIfEmpty() != null && configFile == null:
                        configFile = v.ConfigFile;
                        continue;

                    case Option v when v.SecretId.ToNullIfEmpty() != null && secretId == null:
                        secretId = v.SecretId;
                        continue;
                }

                break;
            }

            return option;

            static string createAccountKeyCommand(string value) => $"{nameof(option.Store)}:{nameof(option.Store.AccountKey)}=" + value.VerifyNotEmpty(nameof(value));
        }
    }
}