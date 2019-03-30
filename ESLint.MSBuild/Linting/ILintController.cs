using ESLint.MSBuild.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESLint.MSBuild.Linting
{
    public interface ILintController
    {
        Task<List<string>> LintFilesAsync(string eslintPath, FileCollectorResult files);
    }
}
