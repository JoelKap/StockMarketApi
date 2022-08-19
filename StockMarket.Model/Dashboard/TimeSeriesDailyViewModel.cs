namespace StockMarket.Model
{
    public class TimeSeriesDailyViewModel  
    {
        public string Date { get; set; }
        public TimeSeriesViewModel TimeSeriesViewModel { get; set; } = new TimeSeriesViewModel();
    }
}