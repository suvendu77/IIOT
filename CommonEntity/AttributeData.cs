using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public enum DataType
    {
        Boolean,
        Integer,
        Float,
        Double,
        String,
        Time,
        ElapsedTime,
        InternationalizedString,
        BooleanArray,
        IntegerArray,
        FloatArray,
        DoubleArray,
        StringArray,
        TimeArray,
        ElapsedTimeArray,
    }
    public class AttributeData
    {
        public AttributeMetaData AttributeMetaData { get; set; }
        public LimitAlarmData AlamLimitData { get; set; }
        public IOExtentionData IOExtentionData { get; set; }
    }
}
