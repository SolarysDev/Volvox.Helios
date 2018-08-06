﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Volvox.Helios.Core.Bot;
using Volvox.Helios.Domain.ModuleSettings;
using Volvox.Helios.Service.Discord.Guild;
using Volvox.Helios.Service.Discord.User;
using Volvox.Helios.Service.Extensions;
using Volvox.Helios.Service.ModuleSettings;
using Volvox.Helios.Web.ViewModels.Settings;

namespace Volvox.Helios.Web.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IModuleSettingsService<StreamAnnouncerSettings> _streamAnnouncerSettingsService;

        public SettingsController(IModuleSettingsService<StreamAnnouncerSettings> streamAnnouncerSettingsService)
        {
            _streamAnnouncerSettingsService = streamAnnouncerSettingsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET
        [HttpGet]
        public async Task<JsonResult> GetGuildChannels([FromServices] IDiscordGuildService guildService, ulong guildId)
        {
            var channels = await guildService.GetChannels(guildId);

            // Format the ulong to string.
            return Json(channels.FilterChannelType(0).Select(c => new {id = c.Id.ToString(), name = c.Name}));
        }

        #region StreamAnnouncer

        // GET
        public async Task<IActionResult> StreamAnnouncerSettings([FromServices] IDiscordUserGuildService userGuildService,
            [FromServices] IBot bot)
        {
            var userGuilds = await userGuildService.GetUserGuilds();

            var botGuilds = bot.GetGuilds();

            var userAdminGuilds = userGuilds.FilterAdministrator().Select(g => g.Guild);

            var viewModel = new StreamAnnouncerSettingsViewModel
            {
                Guilds = new SelectList(userAdminGuilds.FilterGuildsByIds(botGuilds.Select(b => b.Id).ToList()), "Id", "Name")
            };

            return View(viewModel);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> StreamAnnouncerSettings(StreamAnnouncerSettingsViewModel viewModel)
        {
            // Save the settings to the database
            await _streamAnnouncerSettingsService.SaveSettings(new StreamAnnouncerSettings
            {
                GuildId = viewModel.GuildId,
                AnnouncementChannelId = viewModel.ChannelId
            });

            return RedirectToAction("Index");
        }

        #endregion
    }
}