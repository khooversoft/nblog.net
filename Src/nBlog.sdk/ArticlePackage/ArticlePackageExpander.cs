using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage
{
    public class ArticlePackageExpander
    {
        private readonly string _packageFile;

        public ArticlePackageExpander(string packageFile) => _packageFile = packageFile.VerifyNotNull(nameof(packageFile));

        public ArticlePackageDetails Expand(CancellationToken token = default)
        {
            string folder = new ZipFile(_packageFile).ExpandToTempFile(token);

            return new ArticlePackageDetails
            {
                ArticleManifest = new ArticleManifestFile(Path.Combine(folder, ArticleConstants.ManifestFileName)).Read(),
                BasePath = folder,
                Files = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories)
                    .Select(x => x[(folder.Length + 1)..])
                    .ToList(),
            };
        }
    }
}
