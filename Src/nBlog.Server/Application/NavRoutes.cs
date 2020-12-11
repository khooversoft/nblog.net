using nBlog.sdk.ArticlePackage;
using nBlog.sdk.Model;
using System;
using System.Text;

namespace NBlog.Server.Application
{
    public static class NavRoutes
    {
        public static string GotoAboutMe() => "/about-me";

        public static string GotoArticle(string id) => $"/article/{id}";

        public static string GotoArticleArea(ArticleArea articleArea) => "/index/" + articleArea.ToString().ToLower();

        public static string GotoByTag(string id) => $"/tag/{Convert.ToBase64String(Encoding.UTF8.GetBytes(id))}";

        public static string GotoContact() => "/contact";

        public static string GotoIndex() => "/";
        public static string GotoSearch(string searchLine) => $"/search/{Convert.ToBase64String(Encoding.UTF8.GetBytes(searchLine))}";

        public static class ArticleIds
        {
            public static ArticleId AboutMe { get; } = new ArticleId("site/about-me");
            public static ArticleId LandingPage { get; } = new ArticleId("site/landing-page");
        }
    }
}