using System;

namespace ExceptionLibrary
{
    public class DataRetrievalException : Exception
    {
        public DataRetrievalException(string message) : base(message) { }
    }
}
