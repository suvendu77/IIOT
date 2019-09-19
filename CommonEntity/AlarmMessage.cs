using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public class AlarmMessage
    {
        public AlarmMessage(string context, LimitAlarmDesc alarm, DataQualityTimestamp dtq)
        {
            Context = context;
            Value = dtq.Value;
            Categoty = alarm.AlarmCategoty;
            Piority = alarm.Piority;
            Message = alarm.Message;
            Timestamp = dtq.Timestamp;
        }
        public string Context { get; set; }
        public double Value { get; set; }
        public AlarmCategoty Categoty { get; set; }
        public int Piority { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
