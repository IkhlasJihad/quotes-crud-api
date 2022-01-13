using System;

namespace QuotesAPI.Exceptions
{
    public class MissedTextFieldException : Exception
    {
        public MissedTextFieldException() : base("Text field is required")
        {

        }
    }
}
