using System;

namespace QuotesAPI.Exceptions
{
    public class PageOutOfRangeException : Exception
    {
        public PageOutOfRangeException(int pages) : base("You're asking for nonexisting page, Enter valid page within [1,"+pages+"]")
        {

        }
    }
}
