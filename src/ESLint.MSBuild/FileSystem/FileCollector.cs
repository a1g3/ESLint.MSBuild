using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ESLint.MSBuild.FileSystem
{
    public class FileCollector : IFileCollector
    {
        public FileCollectorResult GetFiles(DirectoryInfo projectDirectory, List<string> fileExtensions)
        {
            var fileCollectorResult = new FileCollectorResult() { FilePaths = new List<string>() };
            fileCollectorResult.IgnorePath = RecursiveParentFoldersForFile(projectDirectory, ".eslintignore");
            fileCollectorResult.ConfigPath = RecursiveParentFoldersForFile(projectDirectory, ".eslintrc");

            foreach (var fileExtension in fileExtensions)
                fileCollectorResult.FilePaths.AddRange(projectDirectory.GetFiles($"*.{fileExtension}", SearchOption.AllDirectories).Select(x => x.FullName));

            return fileCollectorResult;
        }

        public string RecursiveParentFoldersForFile(DirectoryInfo projectDirectory, string filename)
        {
            var filePath = projectDirectory.EnumerateFiles(filename).FirstOrDefault()?.FullName;
            if (!string.IsNullOrEmpty(filePath)) return filePath;

            var directory = projectDirectory.Parent;
            do
            {
                filePath = directory.EnumerateFiles(filename).FirstOrDefault()?.FullName;
                if (!string.IsNullOrEmpty(filePath)) return filePath;
                directory = directory.Parent;
            } while (directory != null);

            return null;
        }

        public string GetESLintPath()
        {
            var npmPath = Environment.GetEnvironmentVariable("PATH").Split(';').Where(x => x.Contains("npm")).First();
            return Path.Combine(npmPath, "eslint.cmd");
        }
    }

    public class FileCollectorResult
    {
        public List<string> FilePaths { get; set; }
        public string IgnorePath { get; set; }
        public string ConfigPath { get; set; }
    }
}
