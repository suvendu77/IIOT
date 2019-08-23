using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonEntity
{
    public class DataQualityTimestamp
    {
        /// <summary>
        /// Gets or sets the device value.
        /// </summary>
        [JsonProperty(PropertyName = "value", Order = 3)]
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [JsonProperty(PropertyName = "status", Order = 4)]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        [JsonProperty(PropertyName = "timestamp", Order = 5)]
        public DateTime Timestamp { get; set; }
    }
}
