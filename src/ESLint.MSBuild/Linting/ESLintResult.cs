using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESLint.MSBuild.Linting
{
    public class BaseResult
    {
    }

    public class ESLintError : BaseResult
    {
        public string Message { get; set; }
    }

    public class ESLintResult : BaseResult
    {
        [JsonProperty("filePath")]
        public string FilePath { get; set; }
        [JsonProperty("messages")]
        public List<ESLintResultMessage> Messages { get; set; }
    }

    public class ESLintResultMessage
    {
        [JsonProperty("ruleId")]
        public string RuleId { get; set; }
        [JsonProperty("line")]
        public int Line { get; set; }
        [JsonProperty("column")]
        public int Column { get; set; }
        [JsonProperty("endLine")]
        public int EndLine { get; set; }
        [JsonProperty("endColumn")]
        public int EndColumn { get; set; }
        [JsonProperty("fatal")]
        public bool IsFatal { get; set; }
        public SeverityLevel Severity {
            get
            {
                switch (this.RawSeverity)
                {
                    case "0": return SeverityLevel.NONE;
                    case "1": return SeverityLevel.WARNING;
                    case "2": return SeverityLevel.ERROR;
                    default: throw new ArgumentException("Not a valid severity level!");
                }
            }
        }
        [JsonProperty("severity")]
        public string RawSeverity { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public enum SeverityLevel {
        NONE,
        WARNING,
        ERROR
    }
}
