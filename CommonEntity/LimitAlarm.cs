using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public enum AlarmCategoty
    {
        Lolo,
        Lo,
        HiHi,
        Hi, 
        None
    }

    public class LimitAlarmDesc
    {     
        [JsonProperty(PropertyName = "AlarmCategoty", Order = 1)]      
        public AlarmCategoty AlarmCategoty;

        [JsonProperty(PropertyName = "Value", Order = 2)]
        public double Value;

        [JsonProperty(PropertyName = "Piority", Order = 3)]
        public int Piority;

        [JsonProperty(PropertyName = "Message", Order = 4)]
        public string Message;
    }
}
