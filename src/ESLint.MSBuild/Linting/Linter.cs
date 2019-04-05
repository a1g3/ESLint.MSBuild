using ESLint.MSBuild.Exceptions;
using System.Diagnostics;
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

        public async Task<string> RunLinterAsync(string filePath, CancellationToken token)
        {
            var output = await RunEslintAsync(filePath, token);
            return output;
        }

        internal async Task<string> RunEslintAsync(string filePath, CancellationToken token)
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

                 string error = string.Empty;
                 string output = string.Empty;

                 process.ErrorDataReceived += ErrorHandler;
                 process.OutputDataReceived += OutputHandler;

                 void ErrorHandler(object sender, DataReceivedEventArgs e)
                 {
                     if (!string.IsNullOrEmpty(e.Data)) error += e.Data;
                 }

                 void OutputHandler(object sender, DataReceivedEventArgs e)
                 {
                     if (!string.IsNullOrEmpty(e.Data)) error += e.Data;
                 }

                 try
                 {
                    if (!process.Start())
                         throw new ProcessException("Unable to start ESLint process.");

                     process.BeginErrorReadLine();
                     process.BeginOutputReadLine();

                     process.WaitForExit();
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
