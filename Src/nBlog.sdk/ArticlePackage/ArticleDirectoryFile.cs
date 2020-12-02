using nBlog.sdk.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Tools;

namespace nBlog.sdk.ArticlePackage
{
    public class ArticleDirectoryFile
    {
        private readonly string _file;

        public ArticleDirectoryFile(string file) => _file = file.VerifyNotEmpty(nameof(file));

        public ArticleDirectory Read()
        {
            _file.VerifyAssert(x => File.Exists(x), $"{_file} does not exist");

            string json = File.ReadAllText(_file);

            return Json.Default.Deserialize<ArticleDirectory>(json)
                .VerifyNotNull($"Failed to deserialize {_file}");
        }
    }
}
