﻿using Kryptoteket.Bot.Interfaces;
using Kryptoteket.Bot.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.InMemoryDB
{
    public class ReflinkRepository : IReflinkRepository
    {
        public List<string> links = new List<string>
        {
            "https://miraiex.com/affiliate/?referral=thorshi"
        };

        public async Task AddReflink(string reflink)
        {
            links.Add(reflink);
            await Task.CompletedTask;
        }

        public async Task<List<string>> GetReflinks()
        {
            return await Task.FromResult(links);
        }
    }
}
