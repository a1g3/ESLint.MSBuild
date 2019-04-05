using ESLint.MSBuild.FileSystem;
using ESLint.MSBuild.Linting;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ESLint.MSBuild.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void Test_ESLintTask_Execute_Basic()
        {
            //ARRANGE
            var projectPath = "C:\\Users\\Bob";
            var eslintPath = "C:\\Users\\Bob\\eslint.cmd";
            var collectorResult = new FileCollectorResult()
            {
                ConfigPath = "C:\\Users\\Bob\\.eslintrc",
                FilePaths = new[] { "C:\\Users\\Bob\\scripts.js" }.ToList(),
            };
            var filePathAssert = string.Empty;
            var fileExtensionsAssert = new List<string>();
            var buildErrorEventArgsAssert = new List<BuildErrorEventArgs>();

            var messages = new List<ESLintResultMessage>() {
                new ESLintResultMessage(){ RuleId = "eqeqeq", Column = 10, EndColumn = 12, Line = 10, EndLine = 10, IsFatal = false, RawSeverity = "2", Message = "USE 3 EQUALS!" }
            };
            var linterResult = new List<BaseResult>()
            {
                new ESLintResult(){ FilePath = collectorResult.FilePaths[0], Messages = messages }
            };

            var fileCollector = new Mock<IFileCollector>(MockBehavior.Strict);
            fileCollector.Setup(x => x.GetFiles(It.IsAny<DirectoryInfo>(), It.IsAny<List<string>>())).Returns(collectorResult).Callback((DirectoryInfo di, List<string> ext) =>
            {
                filePathAssert = di.FullName;
                fileExtensionsAssert = ext;
            });
            fileCollector.Setup(x => x.GetESLintPath()).Returns(eslintPath);

            var lintController = new Mock<ILintController>(MockBehavior.Strict);
            lintController.Setup(x => x.LintFilesAsync(eslintPath, collectorResult)).Returns(Task.FromResult(linterResult));

            var buildEngine = new Mock<IBuildEngine>(MockBehavior.Strict);
            buildEngine.Setup(x => x.LogErrorEvent(It.IsAny<BuildErrorEventArgs>())).Callback((BuildErrorEventArgs eventArgs) => { buildErrorEventArgsAssert.Add(eventArgs); });

            var eslintTask = new ESLintTask(lintController.Object, fileCollector.Object) { ProjectPath = projectPath };
            eslintTask.BuildEngine = buildEngine.Object;

            //ACT
            bool result = eslintTask.Execute();

            //ASSERT
            Assert.IsFalse(result);
            Assert.IsTrue(filePathAssert.Contains("C:\\Users\\Bob"));
            Assert.AreEqual(1, fileExtensionsAssert.Count);
            Assert.IsTrue(fileExtensionsAssert.Contains("js"));
            Assert.AreEqual(1, buildErrorEventArgsAssert.Count);
            AssertBuildErrorArgs(buildErrorEventArgsAssert[0], "eqeqeq", "C:\\Users\\Bob\\scripts.js", 10, 10, 10, 12, "USE 3 EQUALS!");
        }

        private void AssertBuildErrorArgs(BuildErrorEventArgs buildErrorEventArgs, string code, string filePath, int line, int column, int endLine, int endColumn, string message)
        {
            Assert.AreEqual(code, buildErrorEventArgs.Code);
            Assert.AreEqual(filePath, buildErrorEventArgs.File);
            Assert.AreEqual(line, buildErrorEventArgs.LineNumber);
            Assert.AreEqual(column, buildErrorEventArgs.ColumnNumber);
            Assert.AreEqual(endLine, buildErrorEventArgs.EndLineNumber);
            Assert.AreEqual(endColumn, buildErrorEventArgs.EndColumnNumber);
            Assert.AreEqual(message, buildErrorEventArgs.Message);
        }
    }
}
