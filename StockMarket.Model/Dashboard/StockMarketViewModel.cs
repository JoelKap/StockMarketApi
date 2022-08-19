using System.Collections.Generic;

namespace StockMarket.Model
{
    public class StockMarketViewModel
    {
        public MetaDataViewModel MetaDataViewModel { get; set; } = new MetaDataViewModel();

        public List<TimeSeriesDailyViewModel> TimeSeriesDailyViewModels { get; set; } = new List<TimeSeriesDailyViewModel>();
    }
} 
