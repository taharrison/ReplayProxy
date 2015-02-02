using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayProxy
{
    public class TypeRequiresDefaultConstructorException : ReplayProxyException
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
