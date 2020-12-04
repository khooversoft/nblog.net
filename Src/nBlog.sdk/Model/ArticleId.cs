using nBlog.sdk.ArticlePackage;
using nBlog.sdk.ArticlePackage.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox.Extensions;
using Toolbox.Tools;

namespace nBlog.sdk.Model
{
    public class ArticleId
    {
        public ArticleId(string id)
        {
            Id = id.ToLower();
            VerifyId();
        }

        public string Id { get; }

        public string ToBase64() => Convert.ToBase64String(Encoding.UTF8.GetBytes(Id));

        public override string ToString() => Id;

        public override int GetHashCode() => HashCode.Combine(Id);

        public override bool Equals(object? obj) => obj is ArticleId id && Id == id.Id;

        public static bool operator !=(ArticleId? left, ArticleId? right) => !(left == right);

        public static bool operator ==(ArticleId? left, ArticleId? right) => EqualityComparer<ArticleId>.Default.Equals(left!, right!);

        public static explicit operator string(ArticleId articleId) => articleId.ToString();

        public static explicit operator ArticleId(string id) => new ArticleId(id);

        public static ArticleId FromBase64(string base64) => new ArticleId(Encoding.UTF8.GetString(Convert.FromBase64String(base64)));

        public void VerifyId()
        {
            Id.VerifyNotEmpty(nameof(Id));
            Id.VerifyAssert(x => char.IsLetter(x[0]), "Must start with letter");
            Id.VerifyAssert(x => char.IsLetterOrDigit(x[^1]), "Must end with letter or number");
            Id.VerifyAssert(x => x.All(y => char.IsLetterOrDigit(y) || y == '.' || y == '/' || y == '-'), "Valid Id must be letter, number, '.', '/', or '-'");
            Id.VerifyAssert(x => x.Split('/').Length > 0, "Missing area + id (ex: area/subject)");
        }
    }
}