using System;

namespace StockMarket.Model
{
    public class MetaDataViewModel
    {
        public string Information { get; set; }
         
        public string Symbol { get; set; }

        public DateTime LastRefreshed { get; set; }

        public string OutputSize { get; set; }

        public string Timezone { get; set; }
    } 
}