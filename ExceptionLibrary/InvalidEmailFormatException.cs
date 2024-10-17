using System.Text.RegularExpressions;

namespace ExceptionLibrary
{
    public class InvalidEmailFormatException : Exception
    {
        public InvalidEmailFormatException(string message) : base(message) { }
    }

}
