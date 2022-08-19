namespace StockMarket.Dal
{
    public class TimeSeriesDailyDal  
    { 
        public string Date { get; set; }
        public TimeSeriesDal TimeSeries { get; set; } = new TimeSeriesDal();
    }
}