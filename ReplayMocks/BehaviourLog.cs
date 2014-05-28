using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ReplayMocks
{
    public class BehaviourLog : List<BehaviourLogItem>
    {
        public override string ToString()
        {
            return String.Concat(this.Select(x => x.ToString()));
        }
    }
}
