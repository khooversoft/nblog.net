using nBlog.sdk.Model;
using System.IO;

namespace nBlog.sdk.ArticlePackage
{
    public static class ArticleConstants
    {
        public static string DirectoryFileName { get; } = "directory.json";
        public static string ManifestFileName { get; } = "articlePackage.manifest.json";
        public static string ObjFolderName { get; } = "obj";
        public static string PackageFolderName { get; } = "packages";

        public static class Folders
        {
            public static string GetObjFolder(string buildFolder) => Path.Combine(buildFolder, ArticleConstants.ObjFolderName);

            public static string GetPackageFolder(string buildFolder) => Path.Combine(buildFolder, ArticleConstants.PackageFolderName);
        }

        public static class Files
        {
            public static string GetDirectoryFile(string buildFolder) => Path.Combine(buildFolder, DirectoryFileName);
        }
    }
}