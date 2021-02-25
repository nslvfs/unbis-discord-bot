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
using System.Text.RegularExpressions;
using System.Threading;
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
                    DiscordIntents.Guilds |
                    DiscordIntents.GuildMessageReactions,
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
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
            // Commands.SetHelpFormatter<Logic.HelpFormatter>();
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<Flaschendrehen>();
            Commands.RegisterCommands<JaNein>();
            Commands.RegisterCommands<Quotes>();
            Commands.RegisterCommands<RussischRoulette>();
            Commands.RegisterCommands<SimpleCmds>();
            Commands.RegisterCommands<Urls>();
            Commands.RegisterCommands<Wegbuxen>();
            Commands.RegisterCommands<RssFeeds>();
            Commands.RegisterCommands<NslBlog>();

            Client.ClientErrored += Client_ClientError;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.MessageCreated += Client_MessageCreated;
            Client.MessageUpdated += Client_MessageUpdated;

            var _messageCache = Task.Factory.StartNew(() => ClearMessageCache());

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

        private async Task Client_MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            await Client_MessageHandling(sender, e.Message, e.Guild);
        }
        private async Task Client_MessageUpdated(DiscordClient sender, MessageUpdateEventArgs e)
        {
            await Client_MessageHandling(sender, e.Message, e.Guild);
        }

        private async Task Client_MessageHandling(DiscordClient sender, DiscordMessage e, DiscordGuild g)
        {
            var Message = new Model.Message();
            if (!e.Author.IsBot)
            {
                Message.Author = e.Author;
                Message.AuthorId = e.Author.Id;
                Message.ChannelId = e.Channel.Id;
                Message.Timestamp = e.Timestamp;
                ArchivMessages.Add(Message);
            }
            if (g.Id == 791393115097137171)
            {
                if (checkBadWords(e.Content))
                {
                    await e.DeleteAsync();
                    await e.Channel.SendMessageAsync("Ah ah aaaah das sagen wir hier nicht! " + e.Author.Mention).ConfigureAwait(false);
                }
                /*
                if(e.Author.Id != 807641560006000670 && e.Author.Id != 134719067016658945)
                    await e.Message.DeleteAsync();
                */
            }

            var msgArr = e.Content.Split();
            if (msgArr.Length >= 3)
            {
                if (msgArr[0] == "was" && msgArr[msgArr.Length - 1] == "sagt")
                {
                    string target = string.Empty;
                    for (int i = 1; i <= msgArr.Length - 2; i++)
                    {
                        target = target + " " + msgArr[i];
                    }

                    target = target.Trim();
                    await e.Channel.SendMessageAsync("das was " + target + " sagt").ConfigureAwait(false);
                }
            }

        }

        private static void ClearMessageCache()
        {
            var sender = Client;
            while (true)
            {
                ArchivMessages.RemoveAll(item => item.Timestamp.AddMinutes(10) < DateTimeOffset.Now);
                Thread.Sleep(1000 * 60 * 5);
            }

        }

        public static void RemoveUserfromMessageArchiv(ulong userId)
        {
            ArchivMessages.RemoveAll(item => item.AuthorId == userId);
        }

        private Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(e.Exception, "Exception aufgetreten");
            return Task.CompletedTask;
        }

        private static bool checkBadWords(string Message)
        {

            string[] badWords = new string[] { "cat", "katz", "k a t z", "kater", "karzer", "kätze", "cutze", "kâtze",
                "kátze", "kàtze" , "kardse", "curtze", "quadsen", "𝕶𝖆𝖙", "𝔎𝔞𝔱", "k atze", "k_a_tze", "k4tz3", "kads",
                "k4t", "k\na\nt\nz", "miau", "mauz", "miez", "gatze", ":cat2:", ":cat:", ":black_cat:", ":heart_eyes_cat:",
                "🇰🇦", "🇿🇪", "𝖟𝖊", "|<atze", "|</-\\tze", "qadse", "quadse", "koschka", "kxaxtxzxe", "|<atze", "k4tze",
                "katže", "kazze", "kätz", "mieds", "kattze", "mîez", "mîezekåtze", "kitten", "kity", "kitties",
                ":regional_indicator_k: :regional_indicator_a: :regional_indicator_t: :regional_indicator_z: :regional_indicator_e:",
                "kudze", "kaatzee", "kazeh"};
            foreach (var item in badWords)
            {
                var msg = Regex.Replace(Message.ToLower(), @"([^\w]|_)", "");
                if (msg.Contains(item))
                    return true;
            }
            return false;
        }
    }
}
