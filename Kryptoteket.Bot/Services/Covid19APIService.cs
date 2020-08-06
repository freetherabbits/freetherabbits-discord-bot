﻿using Kryptoteket.Bot.Configurations;
using Kryptoteket.Bot.Exceptions;
using Kryptoteket.Bot.Interfaces;
using Kryptoteket.Bot.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.Services
{
    public class Covid19APIService : ICovid19APIService
    {
        private readonly HttpResponseService _httpResponseService;
        private readonly CovidAPIConfiguration _covidOptions;

        public Covid19APIService(HttpResponseService httpResponseService, IOptions<CovidAPIConfiguration> covidOptions)
        {
            _httpResponseService = httpResponseService;
            _covidOptions = covidOptions.Value;

            if (string.IsNullOrEmpty(_covidOptions.Uri)) throw new ArgumentNullException(nameof(_covidOptions.Uri));
        }

        public async Task<CovidCountryStats> GetCountryStats(string countryCode)
        {
            using (var client = new HttpClient())
            using (var requets = new HttpRequestMessage(HttpMethod.Get, $"{_covidOptions.Uri}free-api?countryTotal={countryCode.ToUpper()}"))
            {
                using (var response = await client.SendAsync(requets, HttpCompletionOption.ResponseHeadersRead))
                {
                    if (response.IsSuccessStatusCode)
                        return await _httpResponseService.DeserializeJsonFromStream<CovidCountryStats>(response);

                    var content = await _httpResponseService.StreamToStringAsync(await response.Content.ReadAsStreamAsync());

                    throw new ApiException(message: content)
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }
            }
        }

    }
}
