using System.Collections.Generic;
using CoderTom.Mimics.Utilities;

namespace CoderTom.Mimics
{
    internal class EqualityByXmlSerialisation : IEqualityComparer<object>
    {
        public bool Equals(object arg1, object arg2)
        {
            // TODO: requires serialisation to be unique

            var ser1 = arg1 != null ? (string)XmlSerialiser.Serialise(arg1) : "";
            var ser2 = arg2 != null ? (string)XmlSerialiser.Serialise(arg2) : "";
            return ser1 == ser2;
        }

        public int GetHashCode(object arg1)
        {
            return (arg1 != null ? (string)XmlSerialiser.Serialise(arg1) : "").GetHashCode();
        }
    }
}
