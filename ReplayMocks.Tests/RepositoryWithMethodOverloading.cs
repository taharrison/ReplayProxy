using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReplayMocks.Tests
{
    public interface IRepositoryWithMethodOverloading
    {
        string OverloadedMethod();
        string OverloadedMethod(int i);
        string OverloadedMethod(string s);
        string OverloadedMethod(int i, string s);
    }

    public class RepositoryWithMethodOverloading : IRepositoryWithMethodOverloading
    {
        public string OverloadedMethod()
        {
            return "Parameterless";
        }
        public string OverloadedMethod(int i)
        {
            return "int " + i;
        }
        public string OverloadedMethod(string s)
        {
            return "string " + (s ?? "<null>");
        }
        public string OverloadedMethod(int i, string s)
        {
            return "int " + i + " and string " + (s ?? "<null>");
        }
    }
}
