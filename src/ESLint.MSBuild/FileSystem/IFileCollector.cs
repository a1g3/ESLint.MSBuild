using System.Collections.Generic;
using System.IO;

namespace ESLint.MSBuild.FileSystem
{
    public interface IFileCollector
    {
        FileCollectorResult GetFiles(DirectoryInfo projectDirectory, List<string> fileExtensions);
        string RecursiveParentFoldersForFile(DirectoryInfo projectDirectory, string filename);
        string GetESLintPath();

    }
}
