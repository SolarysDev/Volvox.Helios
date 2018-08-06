﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Volvox.Helios.Domain.Discord;

namespace Volvox.Helios.Service.Discord.User
{
    public interface IDiscordUserGuildService
    {
        /// <summary>
        /// Get all of the logged in users guilds.
        /// </summary>
        /// <returns>List of all of the logged in users guilds.</returns>
        Task<List<UserGuild>> GetUserGuilds();
    }
}