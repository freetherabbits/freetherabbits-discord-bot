﻿using Kryptoteket.Bot.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.Interfaces
{
    public interface ICoinGeckoAPIService
    {
        Task<List<CoinGeckoCurrency>> GetCoinsList();
        Task<Price> GetPrice(string pair);
        Task<List<string>> GetSupportedVsCurrenciesList();
        Task<List<Gainers>> GetTopGainers(int top, string timePeriod);
        Task<ChartResult> Get7dChart(string currency);
    }
}
