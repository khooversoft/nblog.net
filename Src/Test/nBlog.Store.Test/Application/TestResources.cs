using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Toolbox.Tools;
using Toolbox.Extensions;
using System.Linq;
using System;

namespace nBlog.Store.Test.Application
{
    internal class TestResources
    {
        public string WriteTestData_1()
        {
            string folder = GetTempDirectory("TestData1");

            var files = new[]
            {
                "Application.Contact.json",
                "Application.Contact.md",
            };

            files
                .ForEach(x => WriteFile("nBlog.Store.Test.TestData1." + x, Path.Combine(folder, x)));

            return Path.Combine(folder, files.First());
        }

        private string GetTempDirectory(string suffix)
        {
            string tempFolder = Path.Combine(Path.GetTempPath(), nameof(nBlog), Guid.NewGuid().ToString(), suffix);
            Directory.CreateDirectory(tempFolder);

            return tempFolder;
        }

        private Stream GetResouceStream(string resourceId) =>
            Assembly.GetAssembly(typeof(TestResources))!
            .GetManifestResourceStream(resourceId)
            .VerifyNotNull($"{resourceId} not found");

        private void WriteFile(string streamId, string toFile)
        {
            using Stream readStream = GetResouceStream(streamId);
            using Stream writeStream = new FileStream(toFile, FileMode.Create);
            readStream.CopyTo(writeStream);
        }
    }
}