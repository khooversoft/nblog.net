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
        private const string baseId = "nBlogCmd.Configs";

        private static IList<Action<Option>> verifications = new List<Action<Option>>
        {
            x => (x.Build | x.Upload)
                .VerifyAssert(x => x = true, "Command not specified (build and/or upload)."),

            x => x.Environment.ToEnvironment(),

            x =>
            {
                if( !x.Build ) return;

                x.SourceFolder.VerifyNotEmpty($"{nameof(x.SourceFolder)} is required for build");
                x.BuildFolder.VerifyNotEmpty($"{nameof(x.BuildFolder)} is required for build");
            },

            x =>
            {
                if( !x.Upload ) return;

                x.BlogStoreOption.StoreUrl.VerifyNotEmpty($"{nameof(x.BlogStoreOption)}:{nameof(x.BlogStoreOption.StoreUrl)} is required for upload");
                x.BlogStoreOption.ApiKey.VerifyNotEmpty($"{nameof(x.BlogStoreOption)}:{nameof(x.BlogStoreOption.ApiKey)} is required for upload");
            },
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
