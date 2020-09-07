using System.Collections.Generic;
using System.Linq;

namespace Myriadbits.MXF.ConformanceValidators
{
    public class ValidationResult
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public List<ValidationResultEntry> Entries { get; set; }

        public bool IsValid { get { return !GetFailedRules().Any(); } }

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

        public void Join(IEnumerable<ValidationResultEntry> entries)
        {
            Entries.AddRange(entries);
        }
    }
}
