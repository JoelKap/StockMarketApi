using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace StockMarket.Model
{
    public class StockMarketViewModel
    {
        [JsonProperty("Meta Data")]
        public MetaData MetaData { get; set; }
         
        [JsonProperty("Time Series (Daily)")]
        public JObject TimeSeriesDto { get; set ; }
         
        public List<TimeSeriesDailyViewModel> TimeSeriesDailyViewModels { get; set; } = new List<TimeSeriesDailyViewModel>();
    }
} 
