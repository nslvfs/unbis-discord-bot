using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using unbis_discord_bot.Commands;

namespace unbis_discord_bot
{
    public class Bot
    {
        public static bool DoCheckBadWords { get; set; }

        public static DateTime BotStart { get; set; }
        public static DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public static ConfigJson ConfigJson { get; set; }
        public static List<Model.Message> ArchivMessages { get; set; } = [];
        public readonly EventId BotEventId = new(42, "exekutivfs");
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            ConfigJson = JsonSerializer.Deserialize<ConfigJson>(json);
            BotStart = DateTime.Now;

            //assigns year, month, day, hour, min, seconds

            DoCheckBadWords = true;

            var config = new DiscordConfiguration
            {
                Token = ConfigJson.Token,

                TokenType = TokenType.Bot,
                Intents = DiscordIntents.GuildMessages |
                    DiscordIntents.GuildBans |
                    DiscordIntents.GuildMembers |
                    DiscordIntents.GuildWebhooks |
                    DiscordIntents.GuildPresences |
                    DiscordIntents.Guilds |
                    DiscordIntents.GuildMessageReactions |
                    DiscordIntents.MessageContents,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Information,
                LogTimestampFormat = "dd MMM yyyy - HH:mm:ss tt",
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = [ConfigJson.Prefix],
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

            Client.ClientErrored += Client_ClientError;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.MessageCreated += Client_MessageCreated;
            Client.MessageUpdated += Client_MessageUpdated;
            Client.MessageReactionAdded += Client_ReactionAdded;
            Client.GuildMemberUpdated += Client_GuildMemberUpdated;

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<BadWords>();
            Commands.RegisterCommands<Flaschendrehen>();
            Commands.RegisterCommands<JaNein>();
            Commands.RegisterCommands<Quotes>();
            Commands.RegisterCommands<RssFeeds>();
            Commands.RegisterCommands<RussischRoulette>();
            Commands.RegisterCommands<SimpleCmds>();
            Commands.RegisterCommands<Urls>();
            Commands.RegisterCommands<Wegbuxen>();
            Commands.RegisterCommands<XSichter>();
            Commands.RegisterCommands<WettCommands>();

            await Client.ConnectAsync();

            try
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                await Client.DisconnectAsync();
            }
        }

        public static Task RemoveUserfromMessageArchiv(ulong userId)
        {
            ArchivMessages.RemoveAll(item => item.AuthorId == userId);
            return Task.CompletedTask;
        }

        public static async Task Mute(DiscordMember target, DiscordGuild g, int durationMin = 10)
        {
            if (target.Id == ConfigJson.BotIdSelf || g.Id != ConfigJson.GuildIdUnbi)
            {
                return;
            }
            try
            {
                var roleMuted = g.GetRole(ConfigJson.RoleIdMuted);
                _ = target.GrantRoleAsync(roleMuted);
                _ = Bot.RemoveUserfromMessageArchiv(target.Id);

                var outChannel = g.GetChannel(ConfigJson.ChannelIdRotz);
                await outChannel.SendMessageAsync(target.Mention + " jetzt für " + durationMin + " Minute(n) still").ConfigureAwait(false);
                await Task.Delay(TimeSpan.FromMinutes(durationMin)).ConfigureAwait(false);
                await target.RevokeRoleAsync(roleMuted);
                await outChannel.SendMessageAsync(target.Mention + " jetzt nicht mehr still").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception :" + ex.Message);
                }
            }
        }

        public static bool CheckBadWords(string Message, bool overrideMode = false)
        {
            if (!DoCheckBadWords && !overrideMode)
            {
                return false;
            }
            var fileName = ConfigJson.badwords;
            var badWords = new List<string>();
            if (File.Exists(fileName))
            {
                foreach (var line in File.ReadLines(fileName))
                {
                    badWords.Add(line);
                }
            }
            else
            {
                File.Create(fileName).Dispose();
            }
            if (Message != null)
            {
                foreach (var item in badWords)
                {
                    if (Message.Contains(item))
                    {
                        Console.WriteLine(item);
                        return true;
                    }
                    if (Message.Contains(item, StringComparison.CurrentCultureIgnoreCase))
                    {
                        Console.WriteLine(item);
                        return true;
                    }
                    var msg = Regex.Replace(Message, @"([^\w]|_)", "");
                    if (msg.Contains(item))
                    {
                        Console.WriteLine(item);
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task Client_ReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
        {
            var Message = new Model.Message();
            if (!e.User.IsBot)
            {
                Message.Author = e.User;
                Message.AuthorId = e.User.Id;
                Message.ChannelId = e.Channel.Id;
                Message.Timestamp = e.Emoji.CreationTimestamp;
                ArchivMessages.Add(Message);
            }
            if (e.User.Id == ConfigJson.BotIdSantos)
            {
                await e.Message.DeleteAllReactionsAsync().ConfigureAwait(false);
            }
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
            if (e.Exception is ChecksFailedException)
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
            await Client_MessageHandling(e.Message, e.Guild).ConfigureAwait(false);
        }
        private async Task Client_MessageUpdated(DiscordClient sender, MessageUpdateEventArgs e)
        {
            await Client_MessageHandling(e.Message, e.Guild);
        }

        private static async Task Client_MessageHandling(DiscordMessage e, DiscordGuild g)
        {
            try
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

                if (g.Id == ConfigJson.GuildIdUnbi)
                {
                    if (CheckBadWords(e.Content))
                    {
                        var newMessage = ReplaceBadwords(e.Content);
                        await e.Channel.SendMessageAsync("Ah ah aaaah das sagen wir hier nicht! " + e.Author.Mention);
                        await e.Channel.SendMessageAsync(e.Author.Mention + " wollte sagen: " + newMessage);
                        _ = Mute((DiscordMember)e.Author, g, 1);
                        await e.DeleteAsync();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception :" + ex.Message);
                }
            }
        }

        private async Task Client_GuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            if (e.Guild.Id == ConfigJson.GuildIdUnbi)
            {
                if (CheckBadWords(e.NicknameAfter))
                {
                    try
                    {
                        await e.Member.ModifyAsync(x =>
                        {
                            x.Nickname = "hurensohn";
                            x.AuditLogReason = "Automatisch + Vorher:" + e.NicknameBefore + "Nachher: " + e.NicknameAfter;
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        private Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(e.Exception, "Exception aufgetreten");
            return Task.CompletedTask;
        }

        private static string ReplaceBadwords(string Message)
        {
            string teststring = Message;
            string output = Message;
            var fileName = ConfigJson.badwords;
            var badWords = new List<string>();
            if (File.Exists(fileName))
            {
                foreach (var line in File.ReadLines(fileName))
                {
                    badWords.Add(line);
                }
            }
            else
            {
                File.Create(fileName).Dispose();
            }
            var found = true;
            var useOriginale = true;
            while (found)
            {
                foreach (var item in badWords)
                {
                    found = false;
                    if (teststring.Contains(item))
                    {
                        teststring = teststring.Replace(item, "ZENSIERT");
                        found = true;
                        useOriginale = false;
                    }

                    if (teststring.Contains(item, StringComparison.CurrentCultureIgnoreCase))
                    {
                        teststring = teststring.ToLower();
                        teststring = teststring.Replace(item.ToLower(), "ZENSIERT");
                        found = true;
                        useOriginale = false;
                    }
                    var msg = Regex.Replace(teststring, @"([^\w]|_)", "");
                    if (msg.Contains(item))
                    {
                        Console.WriteLine(item);
                        teststring = msg.Replace(item, "ZENSIERT");
                        found = true;
                        useOriginale = false;
                    }
                }
            }
            if (!useOriginale)
            {
                output = teststring;
            }
            if (output.Length >= 2000)
            {
                output = output[..^100] + "...";
            }
            return output;
        }
    }
}
