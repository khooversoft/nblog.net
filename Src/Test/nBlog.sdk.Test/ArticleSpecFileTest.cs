using FluentAssertions;
using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Model;
using nBlog.sdk.Test.Application;
using System;
using System.Linq;
using Xunit;

namespace nBlog.sdk.Test
{
    public class ArticleSpecFileTest
    {
        [Fact]
        public void GivenTestData_WhenBuildPackage_ShouldVerify()
        {
            string specFile = new TestResources().WriteTestData_1();

            ArticleSpec articleSpec = new ArticleSpecFile(specFile).Read();

            articleSpec.PackageFile.Should().Be("article.contact.articlePackage");
            articleSpec.Manifest.ArticleId.Should().Be("article/contact");
            articleSpec.Manifest.Title.Should().Be("Contact");
            articleSpec.Manifest.Author.Should().Be("Ghost Writer");
            articleSpec.Manifest.Summary.Should().Be("Summary text");
            articleSpec.Manifest.ImageFile.Should().Be("ImageFile text");
            articleSpec.Manifest.Date.Should().Be(new DateTime(2020, 1, 2));

            Enumerable.SequenceEqual(articleSpec.Manifest.Tags, new[] { "Tag1", "Tag2" }).Should().BeTrue();
            Enumerable.SequenceEqual(articleSpec.Manifest.Categories, new[] { "Category1", "Category2" }).Should().BeTrue();
        }
    }
}