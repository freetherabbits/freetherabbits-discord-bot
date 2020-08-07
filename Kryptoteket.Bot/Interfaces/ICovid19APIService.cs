﻿using Kryptoteket.Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.Interfaces
{
    public interface ICovid19APIService
    {
        Task<CovidCountryStats> GetCountryStats(string countryCode);
        Task<CovidCountryStats> GetCountryStatsYesterday(string countryCode);
    }
}
