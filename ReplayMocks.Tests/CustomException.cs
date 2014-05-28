using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayMocks.Tests
{
    public class CustomException : ApplicationException
    {
        public CustomException()
        {

        }

        public CustomException(string message)
            : base(message)
        {

        }
    }
}
