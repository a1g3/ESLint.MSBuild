using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ESLint.MSBuild.Linting
{
    internal interface ILinter
    {
        Task<string> RunLinterAsync(string filePath, CancellationToken token);
    }
}
