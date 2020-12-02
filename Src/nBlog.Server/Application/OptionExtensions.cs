using NBlog.Server.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Toolbox.Model;
using Toolbox.Tools;

namespace nBlogCmd.Application
{
    internal static class OptionExtensions
    {
        private const string baseId = "NBlog.Server.Configs";

        private static IList<Action<Option>> verifications = new List<Action<Option>>
        {
            x => x.Environment.ToEnvironment(),
            x => x.BlogStoreUrl.VerifyNotEmpty($"{nameof(x.BlogStoreUrl)} is required for upload"),
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
    }
}
