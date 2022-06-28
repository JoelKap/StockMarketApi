using StockMarket.Model;

namespace StockMarket.Service
{
    public interface IStockMarketService
    {
        StockMarketViewModel GetStockMarket(int skip, int take);
    }
}
 