using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StockMarket.Model;
using System.Collections.Generic;
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

            var stockMarket = DeserializeResponseMessage(response);

            var viewModel = new StockMarketViewModel();
            FilterRecordsPerParams(skip, take, stockMarket, viewModel);

            return viewModel;
        }

        private static void FilterRecordsPerParams(int skip, int take, StockMarketViewModel stockMarket, StockMarketViewModel viewModel)
        {
            for (int i = 0; i < stockMarket.TimeSeriesDailyViewModels.Count; i++)
            {
                var remainingViewModelItems = stockMarket.TimeSeriesDailyViewModels.Skip(skip).Take(take).ToList(); ;
                viewModel.MetaData = stockMarket.MetaData;
                viewModel.TimeSeriesDailyViewModels.AddRange(remainingViewModelItems);
                break;
            }
        }

        private StockMarketViewModel DeserializeResponseMessage(HttpResponseMessage response)
        {
            var viewModel = new StockMarketViewModel();

            var strResponse = response.Content.ReadAsStringAsync().Result;
            var result = JsonConvert.DeserializeObject<StockMarketViewModel>(strResponse);

            foreach (var item in result.TimeSeriesDto)
            {
                var key = item.Key;
                var values = JsonConvert.DeserializeObject<TimeSeriesViewModel>(item.Value.ToString());

                viewModel.MetaData = result.MetaData;
                viewModel.TimeSeriesDailyViewModels.Add(CreateAndAddTimeSeriesDaily(key, values));
            }

            return viewModel;
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

        private static TimeSeriesDailyViewModel CreateAndAddTimeSeriesDaily(string key, TimeSeriesViewModel values)
        {
            return new TimeSeriesDailyViewModel()
            {
                Date = key,
                TimeSeriesViewModel = new TimeSeriesViewModel()
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
