using Microsoft.Data.SqlClient;
using System;

namespace ExceptionLibrary
{
    public class DataConnectionException : Exception
    {
        public DataConnectionException(string message) : base(message) { }
    }
}
