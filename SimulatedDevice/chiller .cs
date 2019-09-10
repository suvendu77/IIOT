using CommonEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SimulatedDevice
{
    public class Chiller
    {
        private bool   online = true;
        private double temperature = 75.0;
        private string temperature_unit = "F";
        private double humidity = 70.0;
        private string humidity_unit = "%";
        private double pressure = 150.0;
        private string pressure_unit = "psig";

        private Dictionary<string, AttributeRuntimeData> attributeDic = new Dictionary<string, AttributeRuntimeData>();



        public Chiller(string id, string name)
        {
            Id = id;
            Name = name;            
            attributeDic.Add("Temperature", new AttributeRuntimeData() { Name = "Temperature" });
            attributeDic.Add("Humidity", new AttributeRuntimeData() { Name = "Humidity" });
            attributeDic.Add("Pressure", new AttributeRuntimeData() { Name = "Pressure" });
        }

        
        [JsonProperty(PropertyName = "Id", Order = 1)]
        public string Id
        {
            get;           
        }

        [JsonProperty(PropertyName = "Name", Order = 2)]
        public string Name
        {
            get;           
        }     

        [JsonProperty(PropertyName = "IsOnline", Order = 3)]
        public bool IsOnline {
            get
            {
                return online;
            }
            set
            {
                online = value;
            }
        }

        [JsonProperty(PropertyName = "AttributeRuntimeDatas", Order = 3)]
        public Dictionary<string, AttributeRuntimeData> AttributeRuntimeDatas
        {
            get
            {
                return attributeDic;
            }
        }


        private double Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                temperature = value;
            }
        }


        private string Temperature_unit
        {
            get
            {
                return temperature_unit;
            }
            set
            {
                temperature_unit = value;
            }
        }


        private double Humidity
        {
            get
            {
                return humidity;
            }
            set
            {
                humidity = value;
            }
        }


        private string Humidity_unit
        {
            get
            {
                return humidity_unit;
            }
            set
            {
                humidity_unit = value;
            }
        }


        private double Pressure
        {
            get
            {
                return pressure;
            }
            set
            {
                pressure = value;
            }
        }


        private string Pressure_unit
        {
            get
            {
                return pressure_unit;
            }
            set
            {
                pressure_unit = value;
            }
        }

        public void SimulateData()
        {
            Random random = new Random();
            temperature = random.Next(0, 90);
            DataQualityTimestamp dtq = new DataQualityTimestamp()
            {
                Value = temperature,
                Status = "Good",
                Timestamp = DateTime.UtcNow
            };
            attributeDic["Temperature"].dqt = dtq;

            humidity = random.Next(0, 100);
            dtq = new DataQualityTimestamp()
            {
                Value = humidity,
                Status = "Good",
                Timestamp = DateTime.UtcNow
            };

            attributeDic["Humidity"].dqt = dtq;

            pressure = random.Next(100, 300);

            dtq = new DataQualityTimestamp()
            {
                Value = pressure,
                Status = "Good",
                Timestamp = DateTime.UtcNow
            };

            attributeDic["Pressure"].dqt = dtq;
        }
    }
}
