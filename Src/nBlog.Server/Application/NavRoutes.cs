using nBlog.sdk.ArticlePackage;
using nBlog.sdk.ArticlePackage.Extensions;
using nBlog.sdk.Model;
using System;
using System.Text;

namespace NBlog.Server.Application
{
    public static class NavRoutes
    {
        public static string GotoArticleArea(ArticleArea articleArea) => "/index/" + articleArea.ToString().ToLower();
        public static string GotoIndex() => "/";
        public static string GotoArticle(string id) => $"/article/{id}";
        public static string GotoByTag(string id) => $"/tag/{Convert.ToBase64String(Encoding.UTF8.GetBytes(id))}";
        public static string GotoSearch(string searchLine) => $"/search/{Convert.ToBase64String(Encoding.UTF8.GetBytes(searchLine))}";
        public static string GotoAboutMe() => "/about-me";
        public static string GotoContact() => "/contact";

        public static class ArticleIds
        {
            public static ArticleId LandingPage { get; } = new ArticleId("site/landing-page");
            public static ArticleId AboutMe { get; } = new ArticleId("site/about-me");
            public static ArticleId Customers { get; } = new ArticleId("site/customers");
        }
    }
}