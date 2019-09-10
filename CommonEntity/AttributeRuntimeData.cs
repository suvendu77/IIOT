using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public class AttributeRuntimeData
    {
        [JsonProperty(PropertyName = "Name", Order = 1)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "DQT", Order = 1)]
        public DataQualityTimestamp dqt { get; set;}
    }
}
