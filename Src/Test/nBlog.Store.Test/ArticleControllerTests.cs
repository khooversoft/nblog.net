using FluentAssertions;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Model;
using nBlog.Store.Test.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace nBlog.Store.Test
{
    public class ArticleControllerTests
    {
        [Fact]
        public async Task GivenFakePackage_WhenFullLifeCycle_ShouldPass()
        {
            TestWebsiteHost host = TestApplication.GetHost();

            const string payload = "This is a test";
            string id = "fake1";

            byte[] bytes = Encoding.UTF8.GetBytes(payload);

            ArticlePayload articlePayload = bytes.ToArticlePayload(id);

            await host.ArticleClient.Set(articlePayload);

            ArticlePayload? readPayload = await host.ArticleClient.Get((ArticleId)id);
            readPayload.Should().NotBeNull();

            (articlePayload == readPayload).Should().BeTrue();

            string payloadText = Encoding.UTF8.GetString(readPayload!.ToBytes());
            payloadText.Should().Be(payload);

            BatchSet<string> searchList = await host.ArticleClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x.StartsWith(id)).Should().BeTrue();

            (await host.ArticleClient.Delete((ArticleId)id)).Should().BeTrue();

            searchList = await host.ArticleClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x.StartsWith(id)).Should().BeFalse();
        }

        [Fact]
        public async Task GivenRealPackage_WhenFullLifeCycleInFolder_ShouldPass()
        {
            TestWebsiteHost host = TestApplication.GetHost();

            string specFile = new TestResources().WriteTestData_1();
            string buildFolder = Path.Combine(Path.GetTempPath(), nameof(nBlog), "build", Guid.NewGuid().ToString());

            string packageFile = new ArticlePackageBuilder()
                .SetSpecFile(specFile)
                .SetBuildFolder(buildFolder)
                .Build();

            byte[] packageBytes = File.ReadAllBytes(packageFile);
            ArticlePayload articlePayload = packageBytes.ToArticlePayload();
            ArticleManifest articleManifest = packageBytes.ReadManifest();

            await host.ArticleClient.Set(articlePayload);

            ArticlePayload? readPayload = await host.ArticleClient.Get((ArticleId)articlePayload.Id);
            readPayload.Should().NotBeNull();

            ArticleManifest readArticleManifest = articlePayload.ReadManifest();
            articleManifest.ArticleId.Should().Be(readArticleManifest.ArticleId);
            articleManifest.PackageVersion.Should().Be(readArticleManifest.PackageVersion);
            articleManifest.Title.Should().Be(readArticleManifest.Title);
            articleManifest.Summary.Should().Be(readArticleManifest.Summary);
            articleManifest.Author.Should().Be(readArticleManifest.Author);
            articleManifest.ImageFile.Should().Be(readArticleManifest.ImageFile);
            articleManifest.Date.Should().Be(readArticleManifest.Date);
            Enumerable.SequenceEqual(articleManifest.Tags!, readArticleManifest.Tags!).Should().BeTrue();

            (articlePayload == readPayload).Should().BeTrue();

            BatchSet<string> searchList = await host.ArticleClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();


            searchList.Records.Any(x => x.StartsWith(articlePayload.Id)).Should().BeTrue();

            (await host.ArticleClient.Delete((ArticleId)articlePayload.Id)).Should().BeTrue();

            searchList = await host.ArticleClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x.StartsWith(articlePayload.Id)).Should().BeFalse();
        }
    }
}
