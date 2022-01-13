using System;

namespace QuotesAPI.Exceptions
{
    public class MissedAuthorFieldException : Exception
    {
        public MissedAuthorFieldException() : base("Author field is required")
        {

        }
    }
}
