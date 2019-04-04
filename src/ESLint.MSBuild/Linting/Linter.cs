using ESLint.MSBuild.Exceptions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESLint.MSBuild.Linting
{
    internal class Linter : ILinter
    {
        internal string EslintPath { get; set; }
        internal string Arguments { get; set; }

        internal Linter(string eslintPath, string arguments)
        {
            this.Arguments = arguments;
            this.EslintPath = eslintPath;
        }

        public async Task<List<string>> RunLinterAsync(string filePath, CancellationToken token)
        {
            var output = await RunEslintAsync(filePath, token);
            return output;
        }

        internal async Task<List<string>> RunEslintAsync(string filePath, CancellationToken token)
        {
            return await Task.Run(() =>
             {
                 var arguments = ($"{filePath} " + Arguments).Trim();
                 var startInfo = new ProcessStartInfo(this.EslintPath, arguments)
                 {
                     CreateNoWindow = true,
                     UseShellExecute = false,
                     RedirectStandardError = true,
                     RedirectStandardOutput = true,
                     StandardErrorEncoding = Encoding.UTF8,
                     StandardOutputEncoding = Encoding.UTF8
                 };

                 var process = new Process { StartInfo = startInfo };

                 List<string> error = new List<string>();
                 List<string> output = new List<string>();

                 process.ErrorDataReceived += ErrorHandler;
                 process.OutputDataReceived += OutputHandler;

                 void ErrorHandler(object sender, DataReceivedEventArgs e)
                 {
                     if (!string.IsNullOrEmpty(e.Data)) error.Add(e.Data);
                 }

                 void OutputHandler(object sender, DataReceivedEventArgs e)
                 {
                     if (!string.IsNullOrEmpty(e.Data)) output.Add(e.Data);
                 }

                 try
                 {
                    if (!process.Start())
                         throw new ProcessException("Unable to start ESLint process.");

                     process.BeginErrorReadLine();
                     process.BeginOutputReadLine();

                     process.WaitForExit();

                     if (error.Any())
                         throw new ESLintException(error.FirstOrDefault());
                 }
                 finally
                 {
                     process.Close();
                 }

                 return output;
             }, token);
        }
    }
}
