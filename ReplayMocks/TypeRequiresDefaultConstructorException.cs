using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayMocks
{
    public class TypeRequiresDefaultConstructorException : ReplayMocksException
    {
        public TypeRequiresDefaultConstructorException()
        {
        }

        public TypeRequiresDefaultConstructorException(string message)
            : base(message)
        {
        }
    }
}
