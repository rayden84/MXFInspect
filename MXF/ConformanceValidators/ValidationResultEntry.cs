using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class MyValidationResult
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public IList<ValidationResultEntry> Entries { get; set; }

        public bool IsValid { get { return !GetFailedRules().Any(); } }

        public MyValidationResult(string name, IEnumerable<ValidationResultEntry> entries)
        {
            Name = name;
            Entries = entries.ToList();
        }

        public IList<ValidationResultEntry> GetFailedRules()
        {
            return Entries.Where(r => r.Passed.HasValue && !r.Passed.Value).ToList();
        }

        public IList<ValidationResultEntry> GetPassedRules()
        {
            return Entries.Where(r => r.Passed.HasValue && r.Passed.Value).ToList();
        }

        public IList<ValidationResultEntry> GetNotExecutedRules()
        {
            return Entries.Where(r => !r.HasBeenExecuted).ToList();
        }

        public IList<ValidationResultEntry> GetExecutedRules()
        {
            return Entries.Where(r => r.HasBeenExecuted).ToList();
        }

        public override string ToString()
        {
            return $"{Name} (Total: {Entries.Count}, Failed: {GetFailedRules().Count}, Passed: {GetPassedRules().Count()})";
        }
    }

    public class ValidationResultEntry
    {
        public string Name { get; set; }
        public object State { get; set; }
        public bool? Passed { get; set; }
        public bool HasBeenExecuted { get; set; }
        public object ActualValue { get; set; }
        public IEnumerable<object> ExpectedValues { get; set; }
    }
}
