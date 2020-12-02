using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage.Extensions
{
    public static class ArticleManifestExtensions
    {
        public static void Verify(this ArticleManifest subject)
        {
            subject.VerifyNotNull(nameof(subject));

            subject.ArticleId.VerifyNotEmpty($"{nameof(subject.ArticleId)} is required");
            ArticleId.VerifyId(subject.ArticleId);

            subject.PackageVersion.VerifyNotEmpty($"{nameof(subject.PackageVersion)} is required");
            subject.Title.VerifyNotEmpty($"{nameof(subject.Title)} is required");
        }

        public static void WriteToFile(this ArticleManifest mlPackageManifest, string filePath)
        {
            filePath.VerifyNotEmpty(nameof(filePath));

            mlPackageManifest.Verify();
            var json = Json.Default.SerializeFormat(mlPackageManifest);
            File.WriteAllText(filePath, json);
        }
    }
}
