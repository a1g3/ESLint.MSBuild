using System;

namespace ESLint.MSBuild.Exceptions
{

    [Serializable]
    internal class ESLintException : Exception
    {
        internal ESLintException() { }
        internal ESLintException(string message) : base(message) { }
        internal ESLintException(string message, Exception inner) : base(message, inner) { }
        protected ESLintException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
