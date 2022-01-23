using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotesAPI.Exceptions
{
    public class NoMatchingQuoteException : Exception
    {
        public NoMatchingQuoteException() : base("No matching quote/s")
        {

        }
    }
}
