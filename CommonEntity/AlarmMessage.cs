using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public class AlarmMessage
    {
        public AlarmMessage(string context, LimitAlarmDesc alarm, double value)
        {
            Context = context;
            Value = value;
            Categoty = alarm.AlarmCategoty;
            Piority = alarm.Piority;
            Message = alarm.Message;
        }
        public string Context { get; set; }
        public double Value { get; set; }
        public AlarmCategoty Categoty { get; set; }
        public int Piority { get; set; }
        public string Message { get; set; }
    }
}
