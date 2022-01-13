using System;

namespace QuotesAPI.Exceptions
{
    public class UpdateFailedException : Exception
    {
        public UpdateFailedException() : base("Update Failed")
        {

        }
    }
}
