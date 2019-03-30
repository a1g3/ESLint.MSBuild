using ESLint.MSBuild.FileSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESLint.MSBuild.Linting
{
    public class LintController : ILintController
    {
        public async Task<List<string>> LintFilesAsync(string eslintPath, FileCollectorResult files)
        {
            var errors = new List<string>();
            var linter = new Linter(eslintPath, ParseArguments(new[] { ("format", "visualstudio") }.ToList()));
            CancellationToken token = new CancellationToken();

            foreach (var file in files.FilePaths)
            {
                try
                {
                    var process = await linter.RunLinterAsync(file, token);
                    errors.AddRange(process);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return errors;
        }

        private string ParseArguments(List<(string, string)> arguments)
        {
            var argumentString = string.Empty;
            foreach (var (Name, Value) in arguments)
            {
                if (string.IsNullOrEmpty(Name)) continue;

                argumentString += $"--{Name} ";
                if (!string.IsNullOrEmpty(Value)) argumentString += $"\"{Value}\" ";
            }
            return argumentString;
        }
    }
}
