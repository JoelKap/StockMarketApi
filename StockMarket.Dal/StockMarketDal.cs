using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace StockMarket.Dal
{
    public class StockMarketDal
    {
        [JsonProperty("Meta Data")]
        public MetaDataDal MetaData { get; set; }
         
        [JsonProperty("Time Series (Daily)")]
        public JObject TimeSeries { get; set ; } 
         
        public List<TimeSeriesDailyDal> TimeSeriesDailies { get; set; } = new List<TimeSeriesDailyDal>();
    }
} 
