﻿using Kryptoteket.Bot.Interfaces;
using Kryptoteket.Bot.Models;
using Kryptoteket.Bot.Models.Bets;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kryptoteket.Bot.CosmosDB.Repositories
{
    public class FinishedBetPlacementsRepository : IBetWinnersRepository
    {
        private readonly KryptoteketContext _context;
        private readonly DbSet<FinishedBetPlacement> _set;
        public FinishedBetPlacementsRepository(KryptoteketContext context)
        {
            _context = context;
            _set = _context.FinishedBetPlacements;
        }

        public async Task<List<FinishedBetPlacement>> GetBetWins(ulong userId)
        {
            return await _set.AsQueryable().Where(x => x.BetUserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<FinishedBetPlacement>> GetWins()
        {
            return await _set.AsQueryable().ToListAsync();
        }

    }
}
