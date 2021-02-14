﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using unbis_discord_bot.Commands;

namespace unbis_discord_bot
{
    public class Bot
    {
        public static DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public static ConfigJson configJson { get; set; }
        public async Task TaskAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.GuildMessages | 
                    DiscordIntents.GuildBans |
                    DiscordIntents.GuildMembers |
                    DiscordIntents.GuildWebhooks | 
                    DiscordIntents.Guilds,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                LogTimestampFormat = "dd MMM yyyy - hh:mm:ss tt",
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true,
                DmHelp = false,
                CaseSensitive = false,
                EnableDefaultHelp = true,
                IgnoreExtraArguments = true
            };

            Client.UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromMinutes(2)
            });

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<Flaschendrehen>();
            Commands.RegisterCommands<JaNein>();
            Commands.RegisterCommands<Quotes>();
            Commands.RegisterCommands<RussischRoulette>();
            Commands.RegisterCommands<SimpleCmds>();
            Commands.RegisterCommands<Urls>();
            Commands.RegisterCommands<Wegbuxen>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
