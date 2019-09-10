using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CommonEntity
{
    public class LimitAlarmData
    {

        [JsonProperty(PropertyName = "LimitAlarms", Order = 1)]
        public Dictionary<AlarmCategoty, LimitAlarmDesc> LimitAlarms = null;

        [JsonProperty(PropertyName = "AlarmDeadband", Order = 2)]
        public double AlarmDeadband;

        [JsonProperty(PropertyName = "TimeDeadband", Order = 3)]
        public DateTime TimeDeadband;


        public void AddLimitAlarm (LimitAlarmDesc limitAlarm)
        {
            if (LimitAlarms == null)
            {
                LimitAlarms = new Dictionary<AlarmCategoty, LimitAlarmDesc>(4);
            }

            LimitAlarms.TryAdd(limitAlarm.AlarmCategoty, limitAlarm);
        }

        public void RemoveLimitAlarm(LimitAlarmDesc limitAlarm)
        {
            if (LimitAlarms == null)
            {
                return;
            }

            LimitAlarms.Remove(limitAlarm.AlarmCategoty);
        }
    }
  
}
