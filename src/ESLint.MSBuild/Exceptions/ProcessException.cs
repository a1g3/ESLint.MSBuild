using System;

namespace ESLint.MSBuild.Exceptions
{

    [Serializable]
    internal class ProcessException : Exception
    {
        internal ProcessException() { }
        internal ProcessException(string message) : base(message) { }
        internal ProcessException(string message, Exception inner) : base(message, inner) { }
        protected ProcessException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
