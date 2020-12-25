using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toolbox.Azure.DataLake.Model;
using Toolbox.Extensions;
using Toolbox.Model;
using Toolbox.Tools;

namespace nBlog.Store.Application
{
    internal static class OptionExtensions
    {
        private const string baseId = "nBlog.Store.Configs";

        private static IList<Action<Option>> verifications = new List<Action<Option>>
        {
            x => x.Environment.ToEnvironment(),

            x => x.Store.Verify(),

            x => x.ApiKey.VerifyNotEmpty($"{nameof(x.ApiKey)} is required"),
        };

        public static void Verify(this Option option)
        {
            option.VerifyNotNull(nameof(option));

            verifications
                .ForEach(x => x(option));
        }

        public static string ToResourceId(this RunEnvironment subject) => subject switch
        {
            RunEnvironment.Local => $"{baseId}.local-config.json",
            RunEnvironment.Dev => $"{baseId}.dev-config.json",
            RunEnvironment.PPE => $"{baseId}.ppe-config.json",
            RunEnvironment.Prod => $"{baseId}.prod-config.json",

            _ => throw new InvalidOperationException(),
        };

        public static void DumpConfigurations(this Option option, ILogger logger)
        {
            const int maxWidth = 80;

            string line = option.GetConfigValues()
                .Prepend(new string('=', maxWidth))
                .Prepend($"Current configurations - Version {Assembly.GetExecutingAssembly().GetName().Version}")
                .Aggregate(string.Empty, (a, x) => a += option.SecretFilter.FilterSecrets(x) + Environment.NewLine);

            logger.LogInformation(line);
        }
    }
}