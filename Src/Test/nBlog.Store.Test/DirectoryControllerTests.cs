using FluentAssertions;
using nBlog.sdk.Model;
using nBlog.Store.Test.Application;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace nBlog.Store.Test
{
    public class DirectoryControllerTests
    {
        [Fact]
        public async Task GivenArticleManifest_WhenFullLifeCycle_ShouldPass()
        {
            TestWebsiteHost host = TestApplication.GetHost();

            const int max = 10;

            var directory = new ArticleDirectory
            {
                Articles = Enumerable.Range(0, max)
                    .Select(x => new ArticleManifest
                    {
                        ArticleId = $"ArticleId-{x}",
                        Title = $"Title_{x}",
                        Date = new DateTime(2020, 1, 5),
                    }).ToList()
            };

            await host.DirectoryClient.Set(directory);

            ArticleDirectory? readDirectory = await host.DirectoryClient.Get();
            readDirectory.Should().NotBeNull();

            directory.Articles.Count.Should().Be(readDirectory!.Articles.Count);

            var list = directory.Articles
                .Zip(readDirectory!.Articles, (o, i) => (o, i))
                .ToList();

            foreach (var item in list)
            {
                (item.o == item.i).Should().BeTrue();
            }

            await host.DirectoryClient.Delete();

            readDirectory = await host.DirectoryClient.Get();
            readDirectory.Should().BeNull();
        }
    }
}