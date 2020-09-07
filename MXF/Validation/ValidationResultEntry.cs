using System;
using System.Collections.Generic;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{

    public class ValidationResultEntry
    {
        public string Name { get; set; }
        public object State { get; set; }
        public bool? Passed { get; set; }
        public bool HasBeenExecuted { get; set; }
        public object ActualValue { get; set; }
        public IEnumerable<object> ExpectedValues { get; set; }

        public override string ToString()
        {
            return $"{Name}, {Passed}, {ActualValue}";
        }
    }
}
