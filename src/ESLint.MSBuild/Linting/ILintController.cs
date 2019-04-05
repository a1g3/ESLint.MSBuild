using ESLint.MSBuild.FileSystem;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESLint.MSBuild.Linting
{
    public interface ILintController
    {
        Task<List<BaseResult>> LintFilesAsync(string eslintPath, FileCollectorResult files);
    }
}
