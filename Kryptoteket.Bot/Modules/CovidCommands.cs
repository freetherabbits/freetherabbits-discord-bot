﻿using Discord.Commands;
using Kryptoteket.Bot.Interfaces;
using Kryptoteket.Bot.Services;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.Modules
{
    [Name("CovidCommands")]
    public class CovidCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ICovid19APIService _covid19APIService;
        private readonly EmbedService _embedService;

        public CovidCommands(ICovid19APIService covid19APIService, EmbedService embedService)
        {
            _covid19APIService = covid19APIService;
            _embedService = embedService;
        }

        [Command("covid", RunMode = RunMode.Async)]
        [Summary("Get covid statistics")]
        public async Task GetCovidInfoByCountry(string countryCode)
        {
            var countryData = await _covid19APIService.GetCountryStats(countryCode.Trim());
            if (countryData == null) await ReplyAsync($"Could not find any data with parameter {countryCode}", false);

            var yesterdayData = await _covid19APIService.GetCountryStatsYesterday(countryCode.Trim());
            long? yesterdayNewCases = null;
            if (yesterdayData != null) yesterdayNewCases = yesterdayData.TotalNewCasesToday;

            var builder = _embedService.EmbedCovidStats(
                countryData.Title,
                countryData.TotalCases,
                countryData.TotalNewCasesToday,
                countryData.TotalDeaths,
                countryData.TotalNewDeathsToday,
                countryData.TotalRecovered,
                countryData.Updated,
                yesterdayNewCases);

            await ReplyAsync(null, false, builder.Build());
        }

    }
}
