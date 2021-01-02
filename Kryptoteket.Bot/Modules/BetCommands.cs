﻿using Discord.Commands;
using Discord.WebSocket;
using Kryptoteket.Bot.Interfaces;
using Kryptoteket.Bot.Models;
using Kryptoteket.Bot.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.Modules
{
    [Name("BetCommands")]
    public class BetCommands : ModuleBase<SocketCommandContext>
    {
        private readonly IBetRepository _betRepository;
        private readonly IUserBetRepository _userBetRepository;
        private readonly EmbedService _embedService;

        public BetCommands(IBetRepository betRepository, IUserBetRepository userBetRepository, EmbedService embedService)
        {
            _betRepository = betRepository;
            _userBetRepository = userBetRepository;
            _embedService = embedService;
        }

        [Command("addbet", RunMode = RunMode.Async)]
        [Summary("add a bet")]
        public async Task Addbet(string shortName, string date)
        {
            var approver = Context.User as SocketGuildUser;
            if (!approver.GuildPermissions.KickMembers && approver.Id != 396311377247207434) { await ReplyAsync($"You don't have permissions to do that"); return; }

            DateTimeOffset dateDTO;
            if (!DateTimeOffset.TryParse(date, CultureInfo.GetCultureInfo("nb-No"), DateTimeStyles.None, out dateDTO)) { await ReplyAsync($"Dateformat is incorrect"); return; }

            var bet = new Bet
            {
                id = shortName,
                Date = dateDTO,
                ShortName = shortName,
                AddedBy = approver.Username,
                Users = new List<UserBet>()
            };

            try
            {
                await _betRepository.CreateBet(bet);
            }
            catch (Exception)
            {
                await ReplyAsync($"Bet already exists");
                return;
            }
            await ReplyAsync($"{shortName} added");
        }

        [Command("deletebet", RunMode = RunMode.Async)]
        [Summary("Delete a bet")]
        public async Task DeleteBet(string shortName)
        {
            var approver = Context.User as SocketGuildUser;
            if (!approver.GuildPermissions.KickMembers && approver.Id != 396311377247207434) { await ReplyAsync($"You don't have permissions to do that"); return; }

            await _betRepository.DeleteBet(shortName);

            await ReplyAsync($"{shortName} deleted");
        }

        [Command("bet", RunMode = RunMode.Async)]
        [Summary("Bet on a bet")]
        public async Task Bet(string shortName, string price)
        {
            var user = Context.User as SocketGuildUser;
            var bet = await _betRepository.Getbet(shortName);
            if (bet == null) { await ReplyAsync($"Bet Doesn't exist"); return; }

            if (!int.TryParse(price, out int priceDTO)) { await ReplyAsync($"Price is incorrect"); return; }

            var userBet = new UserBet
            {
                Price = price,
                id = bet.id + user.Id.ToString(),
                Name = user.Username,
                BetId = bet.id
            };

            try
            {
                await _userBetRepository.AddUserBet(userBet);
            }
            catch (Exception)
            {
                await ReplyAsync($"Bet already exists");
                return;
            }
            await ReplyAsync($"**{userBet.Name}** | Price: ${price} at {bet.Date.ToString("dd/M/yyyy", CultureInfo.GetCultureInfo("nb-No"))}");
        }

        [Command("getbet", RunMode = RunMode.Async)]
        [Summary("Get a bet")]
        public async Task GetBet(string shortName)
        {
            var bet = await _betRepository.Getbet(shortName);
            if (bet == null) { await ReplyAsync($"Bet doesn't exist"); return; }
            var userbets = await _userBetRepository.GetUserBets(bet.id);

            if (userbets.Count == 0) { await ReplyAsync($"No bets are placed"); return; }

            await ReplyAsync(null, false, _embedService.EmbedBets(bet).Build());
        }
    }
}
