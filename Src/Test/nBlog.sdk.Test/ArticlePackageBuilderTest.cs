using FluentAssertions;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Model;
using nBlog.sdk.Test.Application;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace nBlog.sdk.Test
{
    public class ArticlePackageBuilderTest
    {
        [Fact]
        public void GivenTestData_WhenBuildPackage_ShouldVerify()
        {
            string specFile = new TestResources().WriteTestData_1();
            string buildFolder = Path.Combine(Path.GetTempPath(), nameof(nBlog), "build", Guid.NewGuid().ToString());

            string packageFile = new ArticlePackageBuilder()
                .SetSpecFile(specFile)
                .SetBuildFolder(buildFolder)
                .Build();

            ArticlePackageDetails articlePackageDetails = new ArticlePackageExpander(packageFile).Expand();

            articlePackageDetails.Should().NotBeNull();
            articlePackageDetails.ArticleManifest.Should().NotBeNull();
            articlePackageDetails.ArticleManifest.ArticleId.Should().Be("articles/contact");
            articlePackageDetails.ArticleManifest.Title.Should().Be("Contact");
            articlePackageDetails.ArticleManifest.Author.Should().Be("Ghost Writer");
            articlePackageDetails.ArticleManifest.Date.Should().Be(new DateTime(2020, 1, 2));

            Enumerable.SequenceEqual(articlePackageDetails.ArticleManifest.Tags, new[] { "Tag1", "Tag2" }).Should().BeTrue();

            var files = new[]
{
                "articlePackage.manifest.json",
                "Application.Contact.md",
            };

            Enumerable.SequenceEqual(articlePackageDetails.Files.OrderBy(x => x), files.OrderBy(x => x)).Should().BeTrue();

            const string mdFileName = "Application.Contact.md";
            byte[] md = GetFileHash(Path.Combine(Path.GetDirectoryName(specFile), mdFileName));
            byte[] restoreMd = GetFileHash(Path.Combine(articlePackageDetails.BasePath, mdFileName));
            Enumerable.SequenceEqual(md, restoreMd).Should().BeTrue();
        }

        [Fact]
        public void GivenTestData_WhenBuildPackage_ShouldBeAbleToReadManifest()
        {
            string specFile = new TestResources().WriteTestData_1();
            string buildFolder = Path.Combine(Path.GetTempPath(), nameof(nBlog), "build", Guid.NewGuid().ToString());

            string packageFile = new ArticlePackageBuilder()
                .SetSpecFile(specFile)
                .SetBuildFolder(buildFolder)
                .Build();

            byte[] fileBytes = File.ReadAllBytes(packageFile);
            ArticlePayload articlePayload = fileBytes.ToArticlePayload((ArticleId)"id");

            ArticleManifest articleManifest = articlePayload.ReadManifest();

            articleManifest.Should().NotBeNull();
            articleManifest.Should().NotBeNull();
            articleManifest.ArticleId.Should().Be("articles/contact");
            articleManifest.Title.Should().Be("Contact");
            articleManifest.Author.Should().Be("Ghost Writer");
            articleManifest.Date.Should().Be(new DateTime(2020, 1, 2));

            Enumerable.SequenceEqual(articleManifest.Tags, new[] { "Tag1", "Tag2" }).Should().BeTrue();
        }

        private static byte[] GetFileHash(string file) => MD5.Create().ComputeHash(new FileStream(file, FileMode.Open));
    }
}
