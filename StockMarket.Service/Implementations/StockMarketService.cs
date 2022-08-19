using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StockMarket.Dal;
using StockMarket.Model;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace StockMarket.Service
{
    public class StockMarketService : IStockMarketService
    {
        public readonly RapidApiGatewayOptions _rapidGateway;

        public StockMarketService(IOptions<RapidApiGatewayOptions> rapidGatewaySettings)
        {
            _rapidGateway = rapidGatewaySettings.Value;
        }

        public StockMarketViewModel GetStockMarket(int skip, int take)
        {
            var response = GetStockMarketsFromRapidApi();

            var viewModel = DeserializeResponseMessage(response);

            var stockMarketViewModel  = FilterRecordsPerParams(skip, take, viewModel);

            return stockMarketViewModel;
        } 

        private static StockMarketViewModel FilterRecordsPerParams(int skip, int take, StockMarketViewModel viewModel)
        {
            var stockMarketViewModel = new StockMarketViewModel();

            for (int i = 0; i < viewModel.TimeSeriesDailyViewModels.Count;)
            {
                var remainingViewModelItems = viewModel.TimeSeriesDailyViewModels.Skip(skip).Take(take).ToList();
                stockMarketViewModel.MetaDataViewModel = viewModel.MetaDataViewModel;
                stockMarketViewModel.TimeSeriesDailyViewModels.AddRange(remainingViewModelItems);
                break;
            }

            return stockMarketViewModel;
        }

        private StockMarketViewModel DeserializeResponseMessage(HttpResponseMessage response)
        {
            var stockMarketDal = new StockMarketDal();
            var viewModel = new StockMarketViewModel();

            var strResponse = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<StockMarketDal>(strResponse);

            foreach (var item in result.TimeSeries)
            {
                var key = item.Key;
                var values = JsonConvert.DeserializeObject<TimeSeriesDal>(item.Value.ToString());

                stockMarketDal.TimeSeriesDailies.Add(CreateAndAddTimeSeriesDaily(key, values));
            }
            stockMarketDal.MetaData = result.MetaData;

            MapToViewModel(stockMarketDal, viewModel);

            return viewModel;
        }

        private static void MapToViewModel(StockMarketDal stockMarketDal, StockMarketViewModel viewModel)
        {
            for (int i = 0; i < stockMarketDal.TimeSeriesDailies.Count; i++)
            {
                var item = stockMarketDal.TimeSeriesDailies[i];

                viewModel.MetaDataViewModel.Information = stockMarketDal.MetaData?.Information;
                viewModel.MetaDataViewModel.LastRefreshed = stockMarketDal.MetaData.LastRefreshed;
                viewModel.MetaDataViewModel.OutputSize = stockMarketDal.MetaData?.OutputSize;
                viewModel.MetaDataViewModel.Symbol = stockMarketDal.MetaData?.Symbol;
                viewModel.MetaDataViewModel.Timezone = stockMarketDal.MetaData?.Timezone;

                viewModel.TimeSeriesDailyViewModels.Add(new TimeSeriesDailyViewModel()
                {
                    Date = item.Date,
                    TimeSeriesViewModel = new TimeSeriesViewModel()
                    {
                        Close = item.TimeSeries?.Close,
                        High = item.TimeSeries?.High,
                        Low = item.TimeSeries?.Low,
                        Open = item.TimeSeries?.Open,
                        Volume = item.TimeSeries?.Volume
                    }
                });
            }
        }

        private HttpResponseMessage GetStockMarketsFromRapidApi()
        {
            HttpResponseMessage response;

            try
            {
                using (var client = new HttpClient())
                {
                    SetUpClientHeaders(client);
                    response = client.GetAsync(_rapidGateway.ApiUrl).Result;

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return response;
                    }
                    else
                    {
                        response = new HttpResponseMessage(HttpStatusCode.NotFound)
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(new
                            {
                                StatusCode = (int)HttpStatusCode.NotFound
                            }, Formatting.Indented))
                        };
                    }
                }
            }
            catch
            {
                response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(
                        JsonConvert.SerializeObject(new
                        {
                            StatusCode = (int)HttpStatusCode.InternalServerError
                        }, Formatting.Indented))
                };
            }

            return response;
        }

        private void SetUpClientHeaders(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("X-RapidAPI-Host", _rapidGateway.ApiHost);
            client.DefaultRequestHeaders.Add("X-RapidAPI-Key", _rapidGateway.ApiKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        private static TimeSeriesDailyDal CreateAndAddTimeSeriesDaily(string key, TimeSeriesDal values)
        {
            return new TimeSeriesDailyDal()
            {
                Date = key,
                TimeSeries = new TimeSeriesDal()
                {
                    Close = values.Close,
                    High = values.High,
                    Low = values.Low,
                    Open = values.Open,
                    Volume = values.Volume
                }
            };
        }
    }
}
