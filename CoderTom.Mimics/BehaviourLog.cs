using System;
using System.Collections.Generic;
using System.Linq;

namespace CoderTom.Mimics
{
    public class BehaviourLog : List<BehaviourLogItem>
    {
        public override string ToString()
        {
            return String.Concat(this.Select(x => x.ToString()));
        }
    }
}
