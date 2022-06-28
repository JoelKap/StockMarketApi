using Microsoft.AspNetCore.Mvc;
using StockMarket.Model;
using StockMarket.Service;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMarketController : ControllerBase
    {
        public readonly IStockMarketService _stockMarketService;
        public StockMarketController(IStockMarketService stockMarketService)
        {
            _stockMarketService = stockMarketService;
        }

        [HttpGet("{skip}/{take}")]
        public StockMarketViewModel Get(int skip, int take)
        {
            return _stockMarketService.GetStockMarket(skip, take);
        }
    }
}
