using System;

namespace ExceptionLibrary
{
    public class ApplicationDeadlineException : Exception
    {
        public ApplicationDeadlineException(string message) : base(message) { }
    }
}
