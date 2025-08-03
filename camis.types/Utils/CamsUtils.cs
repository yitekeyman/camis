using System;
using System.Collections.Generic;
using System.Text;

namespace camis.types.Utils
{
    public static class CamisUtils
    {
        class CAMISAssertFailure : Exception
        {
            public CAMISAssertFailure(string message) : base(message)
            {
            }
        }
        public static void Assert(bool condition,String msg)
        {
            if (!condition)
                throw new CAMISAssertFailure(msg);
        }
    }
}
