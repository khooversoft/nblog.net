using FluentAssertions;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Model;
using nBlog.sdk.Test.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace nBlog.sdk.Test
{
    public class ArticlePackageIndexBuilderTest
    {
        [Fact]
        public void GivenTestData_WhenBuildPackage_ShouldVerify()
        {
            string specFile = new TestResources().WriteTestData_1();
            string buildFolder = Path.Combine(Path.GetTempPath(), nameof(nBlog), "build", Guid.NewGuid().ToString());

            string packageFile = new ArticlePackageBuilder()
                .SetSpecFile(specFile)
                .SetBuildFolder(buildFolder)
                .SetObjFolder(Path.Combine(buildFolder, "obj"))
                .Build();

            IReadOnlyList<WordCount> wordCounts = new ArticlePackageIndexBuilder().Build(packageFile);
            wordCounts.Should().NotBeNull();
            wordCounts.Count.Should().BeGreaterThan(0);

            var list = new[]
            {
                new WordCount { Word = "#nblog", Count = 1 },
                new WordCount { Word = "Contact", Count = 2 },
                new WordCount { Word = "Email", Count = 1 },
                new WordCount { Word = "nBlog@domain", Count = 1 },
                new WordCount { Word = "net", Count = 1 },
                new WordCount { Word = "Please", Count = 1 },
                new WordCount { Word = "Twitter", Count = 1 },
                new WordCount { Word = "us", Count = 1 },
            };

            Enumerable.SequenceEqual(wordCounts, list).Should().BeTrue();
        }
    }
}
