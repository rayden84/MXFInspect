using System;

namespace Myriadbits.MXF
{
    public record TaskReport
    {
        public string Description { get; init; }
        public int Percent { get; init; }

        public TaskReport(int percent, string desc)
        {
            if(percent > 100 || percent < 0)
            {
                throw new ArgumentOutOfRangeException("Percent must be between 0 and 100");
            }
            Description = desc;
            Percent = percent;
        }
    }
}