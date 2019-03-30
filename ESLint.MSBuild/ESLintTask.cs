using ESLint.MSBuild.FileSystem;
using ESLint.MSBuild.Linting;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#if DEBUG
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("ESLint.MSBuild.Tests")]
#endif

namespace ESLint.MSBuild
{
    public class ESLintTask : Task
    {
        [Required]
        public string ProjectPath { get; set; }
        private ILintController LintController { get; set; }
        private readonly Regex ErrorRegex = new Regex(@"(.*)\((\d+),(\d+)\): (error|warning) (.*) : (.*)");
        private readonly Regex ProblemRegex = new Regex(@"\d+ problem");

        public ESLintTask() : this(new LintController()) { }
        internal ESLintTask(ILintController lintController)
        {
            this.LintController = lintController;
        }

        public override bool Execute()
        {
            var directory = new DirectoryInfo(ProjectPath);
            var files = FileCollector.GetFiles(directory, new[] { "js" }.ToList());
            var eslintPath = FileCollector.GetESLintPath();
            var errors = LintController.LintFilesAsync(eslintPath, files);
            errors.Wait();

            foreach (var error in errors.Result)
                AddErrorToErrorList(error);

            return !errors.Result.Any();
        }

        private void AddErrorToErrorList(string outputError)
        {
            if (string.IsNullOrEmpty(outputError)) return;

            if (ErrorRegex.IsMatch(outputError))
            {
                var errorParts = ErrorRegex.Split(outputError);

                if(errorParts[4].Equals("error", StringComparison.OrdinalIgnoreCase))
                {
                    BuildErrorEventArgs errorEvent = new BuildErrorEventArgs("", errorParts[5], errorParts[1], int.Parse(errorParts[2]), int.Parse(errorParts[3]), 0, 0, errorParts[6], "", "");
                    BuildEngine.LogErrorEvent(errorEvent);
                }
                else if(errorParts[4].Equals("warning", StringComparison.OrdinalIgnoreCase))
                {
                    BuildWarningEventArgs warningEvent = new BuildWarningEventArgs("", errorParts[5], errorParts[1], int.Parse(errorParts[2]), int.Parse(errorParts[3]), 0, 0, errorParts[6], "", "");
                    BuildEngine.LogWarningEvent(warningEvent);
                }
            } else
            {
                if (ProblemRegex.IsMatch(outputError)) return;
                this.Log.LogError(outputError);
            }
        }
    }
}
