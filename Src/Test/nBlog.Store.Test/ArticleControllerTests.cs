﻿using FluentAssertions;
using nBlog.sdk.ArticlePackage;
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

            await host.BlogClient.Set(articlePayload);

            ArticlePayload? readPayload = await host.BlogClient.Get((ArticleId)id);
            readPayload.Should().NotBeNull();

            (articlePayload == readPayload).Should().BeTrue();

            string payloadText = Encoding.UTF8.GetString(readPayload!.ToBytes());
            payloadText.Should().Be(payload);

            BatchSet<string> searchList = await host.BlogClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x.StartsWith(id)).Should().BeTrue();

            (await host.BlogClient.Delete((ArticleId)id)).Should().BeTrue();

            searchList = await host.BlogClient.List(QueryParameters.Default).ReadNext();
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

            await host.BlogClient.Set(articlePayload);

            ArticlePayload? readPayload = await host.BlogClient.Get((ArticleId)articlePayload.Id);
            readPayload.Should().NotBeNull();

            (articlePayload == readPayload).Should().BeTrue();

            BatchSet<string> searchList = await host.BlogClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x.StartsWith(articlePayload.Id)).Should().BeTrue();

            (await host.BlogClient.Delete((ArticleId)articlePayload.Id)).Should().BeTrue();

            searchList = await host.BlogClient.List(QueryParameters.Default).ReadNext();
            searchList.Should().NotBeNull();
            searchList.Records.Any(x => x.StartsWith(articlePayload.Id)).Should().BeFalse();
        }
    }
}