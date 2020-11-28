using FluentAssertions;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Model;
using nBlog.Store.Test.Application;
using System;
using System.Collections.Generic;
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
            string id = $"{nameof(GivenFakePackage_WhenFullLifeCycle_ShouldPass)}.package";

            byte[] bytes = Encoding.UTF8.GetBytes(payload);

            ArticlePayload articlePayload = bytes.ToArticlePayload(id);

            await host.BlogClient.Set(articlePayload);

            ArticlePayload? readPayload = await host.BlogClient.Get(id);
            readPayload.Should().NotBeNull();

            (articlePayload == readPayload).Should().BeTrue();

            string payloadText = Encoding.UTF8.GetString(readPayload!.ToBytes());
            payloadText.Should().Be(payload);

            BatchSet<string> searchList = await host.BlogClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x == id).Should().BeTrue();

            bool status = await host.BlogClient.Delete(id);
            status.Should().BeTrue();

            searchList = await host.BlogClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x == id).Should().BeFalse();
        }

        [Fact]
        public async Task GivenFakePackage_WhenFullLifeCycleInFolder_ShouldPass()
        {
            TestWebsiteHost host = TestApplication.GetHost();

            const string payload = "This is a test";
            string id = $"test1/{nameof(GivenFakePackage_WhenFullLifeCycleInFolder_ShouldPass)}.package";

            byte[] bytes = Encoding.UTF8.GetBytes(payload);

            ArticlePayload articlePayload = bytes.ToArticlePayload(id);

            await host.BlogClient.Set(articlePayload);

            ArticlePayload? readPayload = await host.BlogClient.Get(id);
            readPayload.Should().NotBeNull();

            (articlePayload == readPayload).Should().BeTrue();

            string payloadText = Encoding.UTF8.GetString(readPayload!.ToBytes());
            payloadText.Should().Be(payload);

            (await host.BlogClient.Delete(id)).Should().BeTrue();

            BatchSet<string> searchList = await host.BlogClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x == id).Should().BeFalse();
        }
    }
}
