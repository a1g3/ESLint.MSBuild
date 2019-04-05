using ESLint.MSBuild.FileSystem;
using ESLint.MSBuild.Linting;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Linq;

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
        private IFileCollector FileCollector { get; set; }

        public ESLintTask() : this(new LintController(), new FileCollector()) { }
        internal ESLintTask(ILintController lintController, IFileCollector fileCollector)
        {
            this.LintController = lintController;
            this.FileCollector = fileCollector;
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

        private void AddErrorToErrorList(BaseResult output)
        {
            if(output is ESLintResult eslintResult)
            {
                foreach (var message in eslintResult.Messages)
                {
                    if (message.IsFatal || message.Severity is SeverityLevel.ERROR)
                    {
                        BuildErrorEventArgs errorEvent = new BuildErrorEventArgs("", message.RuleId, eslintResult.FilePath, message.Line, message.Column, message.EndLine, message.EndColumn, message.Message, "", "");
                        BuildEngine.LogErrorEvent(errorEvent);
                    }
                    else if(message.Severity is SeverityLevel.WARNING)
                    {
                        BuildWarningEventArgs warningEvent = new BuildWarningEventArgs("", message.RuleId, eslintResult.FilePath, message.Line, message.Column, message.EndLine, message.EndColumn, message.Message, "", "");
                        BuildEngine.LogWarningEvent(warningEvent);
                    }
                }
            } else if(output is ESLintError eslintError)
                this.Log.LogError(eslintError.Message);
        }
    }
}
