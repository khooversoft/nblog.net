using FluentAssertions;
using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace nBlog.sdk.Test
{
    public class ArticleIdTest
    {
        [Theory]
        [InlineData("a")]
        [InlineData("ab")]
        [InlineData("a1")]
        [InlineData("a.1")]
        [InlineData("a.a")]
        [InlineData("a/a")]
        [InlineData("a-a")]
        [InlineData("a/a.b")]
        [InlineData("A/A.b")]
        public void GivenValidArticleId_WhenVerified_ShouldPass(string id)
        {
            _ = ((ArticleId)id).Id.Should().Be(id.ToLower());
            _ = ((string)(ArticleId)id).Should().Be(id.ToLower());
            _ = ((string)(new ArticleId(id))).Should().Be(id.ToLower());
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("/")]
        [InlineData("-")]
        [InlineData(".")]
        [InlineData("1b")]
        [InlineData("*")]
        [InlineData("1/")]
        [InlineData("1.")]
        public void GivenBadArticleId_WhenVerified_ShouldFail(string id)
        {
            Action action = () => _ = (ArticleId)id;
            action.Should().Throw<ArgumentException>();

            action = () => _ = (string)(ArticleId)id;
            action.Should().Throw<ArgumentException>();

            action = () => _ = (string)(new ArticleId(id));
            action.Should().Throw<ArgumentException>();
        }
    }
}
