﻿using Discord.Commands;
using Discord.WebSocket;
using Kryptoteket.Bot.Interfaces;
using Kryptoteket.Bot.Models;
using Kryptoteket.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.Modules
{
    [Name("CovidCommands")]
    public class ReflinksCommands : ModuleBase<SocketCommandContext>
    {
        private readonly IReflinkRepository _reflinkRepository;
        private readonly EmbedService _embedService;

        public ReflinksCommands(IReflinkRepository reflinkRepository, EmbedService embedService)
        {
            _reflinkRepository = reflinkRepository;
            _embedService = embedService;
        }

        [Command("reflink", RunMode = RunMode.Async)]
        [Summary("Get random reflink")]
        public async Task GetReflink()
        {
            var reflinks = await _reflinkRepository.GetReflinks();

            var random = new Random();
            var index = random.Next(reflinks.Count);

            var luckyLink = reflinks[index];
            await ReplyAsync($"<{luckyLink.Link}>");
        }

        [Command("addref", RunMode = RunMode.Async)]
        [Summary("Add reflink")]
        public async Task RequestReflink(string reflink)
        {
            var user = Context.User as SocketGuildUser;

            if (!reflink.Contains("https://miraiex.com/affiliate/?referral=")){ await ReplyAsync($"Reflink is wrong format");return;}
            if (await _reflinkRepository.Exists(user.Id)) { await ReplyAsync($"Reflink is already in list"); return;}

            await _reflinkRepository.AddReflink(user.Id, user.Username, reflink.Trim());
            await ReplyAsync($"Reflink was submitted for approval");
        }

        [Command("getrefs", RunMode = RunMode.Async)]
        [Summary("Request all reflinks")]
        public async Task GetReflinks()
        {
            //var user = Context.User as SocketGuildUser;
            //var allowed = user.Roles.Any(x => x.Id == 344896780657491968 || x.Id == 344896529707958272);

            //if(!allowed) { await ReplyAsync($"Insufficient permissions"); return; }

            var reflinks = await _reflinkRepository.GetReflinks();
            await ReplyAsync(null,false, _embedService.EmbedAllReflinks(reflinks).Build());
        }

        [Command("getref", RunMode = RunMode.Async)]
        [Summary("Request your reflink")]
        public async Task GetYourReflink()
        {
            var user = Context.User as SocketGuildUser;

            var reflink = await _reflinkRepository.GetReflink(user.Id);
            if(reflink != null)
            {
                await ReplyAsync($"**{reflink.Name}**: <{reflink.Link}>");
            }
            else
            {
                await ReplyAsync($"Could not find your reflink");
            }
        }

        [Command("updateref", RunMode = RunMode.Async)]
        [Summary("Update your reflink")]
        public async Task UpdateYourReflink(string reflink)
        {
            var user = Context.User as SocketGuildUser;
            if (!reflink.Contains("https://miraiex.com/affiliate/?referral=")) { await ReplyAsync($"Reflink is wrong format"); return; }

            await _reflinkRepository.UpdateReflink(user.Id, reflink);

            await ReplyAsync($"Reflink was updated");
        }

        [Command("deleteref", RunMode = RunMode.Async)]
        [Summary("Delete your reflink")]
        public async Task DeleteYourReflink()
        {
            var user = Context.User as SocketGuildUser;

            await _reflinkRepository.DeleteReflink(user.Id);

            await ReplyAsync($"Reflink was deleted");
        }


    }
}
