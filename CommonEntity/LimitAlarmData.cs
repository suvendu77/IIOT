using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CommonEntity
{
    public class LimitAlarmData
    {

        [JsonProperty(PropertyName = "LimitAlarms", Order = 1)]
        public Dictionary<AlarmCategoty, LimitAlarm> LimitAlarms = null;

        [JsonProperty(PropertyName = "AlarmDeadband", Order = 2)]
        public double AlarmDeadband;

        [JsonProperty(PropertyName = "TimeDeadband", Order = 3)]
        public DateTime TimeDeadband;


        public void AddLimitAlarm (LimitAlarm limitAlarm)
        {
            if (LimitAlarms == null)
            {
                LimitAlarms = new Dictionary<AlarmCategoty, LimitAlarm>(4);
            }

            LimitAlarms.TryAdd(limitAlarm.AlarmCategoty, limitAlarm);
        }

        public void RemoveLimitAlarm(LimitAlarm limitAlarm)
        {
            if (LimitAlarms == null)
            {
                return;
            }

            LimitAlarms.Remove(limitAlarm.AlarmCategoty);
        }
    }
  
}
