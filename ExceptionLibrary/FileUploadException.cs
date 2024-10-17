using System;

namespace ExceptionLibrary
{
    public class FileUploadException : Exception
    {
        public FileUploadException(string message) : base(message) { }
    }

}
