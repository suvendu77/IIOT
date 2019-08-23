using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public enum ConvertionMode
    {
        Linear,
        SquareRoot
    }
    public class IOScalingData
    {
        public ConvertionMode ConvertionMode { get; set; }

        public double RawValueMax { get; set; }

        public double RawValueMin { get; set; }

        public double EngValueMax { get; set; }

        public double EngValueMin { get; set; }
    }
}
