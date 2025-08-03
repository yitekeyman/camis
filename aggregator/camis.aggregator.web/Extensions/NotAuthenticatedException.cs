using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace camis.aggregator.web.Extensions
{
    public class NotAuthenticatedException : Exception
    {
        public NotAuthenticatedException(string exception) : base(exception)
        {

        }
    }
}
