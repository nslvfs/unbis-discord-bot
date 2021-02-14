using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public static List<Model.Message> ArchivMessages { get; set; }
        public readonly EventId BotEventId = new EventId(42, "exekutivfs");
        public async Task TaskAsync()
        {
            var json = string.Empty;
            ArchivMessages = new List<Model.Message>();
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
                MinimumLogLevel = LogLevel.Information,
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
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            Client.ClientErrored += Client_ClientError;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.MessageCreated += Client_MessageCreated;

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            sender.Logger.LogInformation(BotEventId, "Client läuft");
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
        {
            sender.Logger.LogInformation(BotEventId, $"Guild verbunden: {e.Guild.Name}");
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            e.Context.Client.Logger.LogInformation(BotEventId, $"{e.Context.User.Username} erfolgreich ausgeführt '{e.Command.QualifiedName}'");
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            e.Context.Client.Logger.LogError(BotEventId, $"{e.Context.User.Username} versuchte '{e.Command?.QualifiedName ?? "<unbekannter befehl>"}' Fehler: {e.Exception.GetType()}: {e.Exception.Message ?? "<keine nachricht>"}", DateTime.Now);
            if (e.Exception is ChecksFailedException ex)
            {
                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");
                var embed = new DiscordEmbedBuilder
                {
                    Title = "Access denied",
                    Description = $"{emoji} Keine ausreichenden Rechte",
                    Color = new DiscordColor(0xFF0000)
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }

        private Task Client_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            var Message = new Model.Message();
            if (!e.Author.IsBot) { 
                Message.Author = e.Message.Author;
                Message.ChannelId = e.Message.Channel.Id;
                Message.Timestamp = e.Message.Timestamp;
                ArchivMessages.Add(Message);
            }

            // ArchivMessages.RemoveAll(item => item.Timestamp.AddMinutes(10) < DateTimeOffset.Now);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(e.Exception, "Exception aufgetreten");
            return Task.CompletedTask;
        }
    }
}
