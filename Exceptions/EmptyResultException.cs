using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuotesAPI.Exceptions
{
    public class EmptyResultException : Exception
    {
        public EmptyResultException() : base("Empty Result, 0 quotes matching your specification")
        {

        }
    }
}
