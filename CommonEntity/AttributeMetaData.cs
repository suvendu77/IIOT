using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public class AttributeMetaData
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DataType DataType { get; set; }

        public string InitialValue { get; set; }

        public string EngUnit { get; set; }

        public bool IsIO { get; set; }

        public bool IsHistory { get; set; }

        public bool IsLimitAlarms { get; set; }

        public bool IsROCAlarms { get; set; }

        public bool IsDeviationAlarms { get; set; }

        public bool IsBadValueAlarms { get; set; }

        public bool IsStatistics { get; set; }

        public bool IsLogChange { get; set; }
    }
}
