using System.Collections.Generic;
using System.Numerics;

namespace ComplexUtils
{
    public class ComplexDataHandler
    {
        public List<Complex> Values { get; set; }
        public string Name { get; set; }

        public ComplexDataHandler(List<Complex> values, string name)
        {
            Values = values;
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}