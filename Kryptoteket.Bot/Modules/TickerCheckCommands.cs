﻿using Discord.Commands;
using Kryptoteket.Bot.Configurations;
using Kryptoteket.Bot.Exceptions;
using Kryptoteket.Bot.Interfaces;
using Kryptoteket.Bot.Models;
using Kryptoteket.Bot.Services;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.Modules
{
    [Name("TickerCheckCommands")]
    public class TickerCheckCommands : ModuleBase<SocketCommandContext>
    {
        private readonly IMiraiexAPIService _miraiexService;
        private readonly EmbedService _embedService;
        private readonly INBXAPIService _nBXAPIService;
        private readonly IBitmyntAPIService _bitmyntAPIService;
        private readonly ExchangesConfiguration _options;

        public TickerCheckCommands(IMiraiexAPIService miraiexService, EmbedService embedService, INBXAPIService nBXAPIService, IBitmyntAPIService bitmyntAPIService, IOptions<ExchangesConfiguration> options)
        {
            _miraiexService = miraiexService;
            _embedService = embedService;
            _nBXAPIService = nBXAPIService;
            _bitmyntAPIService = bitmyntAPIService;
            _options = options.Value;
        }

        [Command("ticker", RunMode = RunMode.Async)]
        [Summary("Get ticker for pair from Miraiex and NBX")]
        public async Task GetTicker(string pair, string exchange)
        {
            if (string.IsNullOrEmpty(pair)) { await ReplyAsync($"Pair cannot be empty", false); return; }
            if (string.IsNullOrEmpty(exchange)) { await ReplyAsync($"Exchange cannot be empty", false); return; }

            var ticker = new Ticker();
            string source = "";
            string thumbnail = "";

            try
            {
                if (exchange.Trim().ToLower() == "mx")
                {
                    source = "MiraiEx";
                    thumbnail = _options.MiraiexIMG;
                    ticker = await _miraiexService.GetTicker(pair.Trim().ToLower());
                }
                else if (exchange.Trim().ToLower() == "nbx")
                {
                    source = "NBX";
                    thumbnail = _options.NBXIMG;
                    ticker = await _nBXAPIService.GetTicker(pair.Trim().ToLower());
                }
                else if(exchange.Trim().ToLower() == "bitmynt")
                {
                    source = "Bitmynt";
                    thumbnail = _options.BitmyntIMG;
                    ticker = await _bitmyntAPIService.GetTicker();
                }
                else
                {
                    await ReplyAsync($"WTF is {exchange}?", false);
                    return;
                }
            }
            catch (ApiException e)
            {
                await ReplyAsync($"API failed with statuscode: {e.StatusCode}", false); return;
            }
            catch (Exception e)
            {
                await ReplyAsync($"LOL: {e.Message}", false); return;
            }

            if (ticker == null) { await ReplyAsync($"The market {pair} is not supported", false); return; }

            var builder = _embedService.EmbedTicker(pair.Trim().ToUpper(), ticker, source, thumbnail);
            await ReplyAsync(null, false, builder.Build());
        }
    }
}
