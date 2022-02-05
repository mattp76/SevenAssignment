using System;

namespace SevenAssignmentLibrary.Exceptions
{
    public class SevenAssignmentException : ApplicationException
    {
        public SevenAssignmentException(string message) : base(message)
        {
        }
    }
}
