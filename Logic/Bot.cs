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
        public const long guildIdUnbi = 791393115097137171;
        public const long guildIdRatte = 442300530996543489;
        public const long channelIdRotz = 816990123568660510;
        public const long channelIdKiosk = 812403060416446474;
        public const long channelIdMain = 808405679417196546;
        public const long roleIdMuted = 807921762570469386;
        public const long botIdSelf = 807641560006000670;
        public const long botIdSantos = 816383436201656341;
        public const long botIdSpurdo = 812064630364700684;
        public const long userIdEsso = 351514728734130177;
        public const long userIdRattan = 690985661695655966;
        public const long userIdvfs = 134719067016658945;
        public static bool DoCheckBadWords { get; set; }

        public static DiscordMessage LastEssoMessage { get; set; }

        public static DiscordMessage LastRattanMessage { get; set; }

        public static DateTime LastRattanMessageDt { get; set; }

        public static DateTime BotStart { get; set; }
        public static DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public static ConfigJson ConfigJson { get; set; }
        public static List<Model.Message> ArchivMessages { get; set; }
        public readonly EventId BotEventId = new EventId(42, "exekutivfs");
        public async Task TaskAsync()
        {
            var json = string.Empty;
            ArchivMessages = new List<Model.Message>();
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEnd();

            ConfigJson = JsonConvert.DeserializeObject<ConfigJson>(json);
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
                    DiscordIntents.GuildMessageReactions,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Information,
                LogTimestampFormat = "dd MMM yyyy - HH:mm:ss tt",
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { ConfigJson.Prefix },
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

            var _messageCache = Task.Factory.StartNew(() => ClearMessageCache());

            await Client.ConnectAsync();

            await Task.Delay(-1);
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
            if (e.User.Id == Bot.botIdSantos)
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
            await Client_MessageHandling(sender, e.Message, e.Guild).ConfigureAwait(false);
        }
        private async Task Client_MessageUpdated(DiscordClient sender, MessageUpdateEventArgs e)
        {
            await Client_MessageHandling(sender, e.Message, e.Guild);
        }

        private async Task Client_MessageHandling(DiscordClient sender, DiscordMessage e, DiscordGuild g)
        {
            try
            {
                if (e.Author.Id == userIdEsso)
                {
                    LastEssoMessage = e;
                }
                if (e.Author.Id == userIdRattan)
                {
                    LastRattanMessage = e;
                }
                var Message = new Model.Message();
                if (!e.Author.IsBot)
                {
                    Message.Author = e.Author;
                    Message.AuthorId = e.Author.Id;
                    Message.ChannelId = e.Channel.Id;
                    Message.Timestamp = e.Timestamp;
                    ArchivMessages.Add(Message);
                }

                if (g.Id == Bot.guildIdUnbi)
                {
                    if (CheckBadWords(e.Content))
                    {
                        var newMessage = ReplaceBadwords(e.Content);
                        await e.Channel.SendMessageAsync("Ah ah aaaah das sagen wir hier nicht! " + e.Author.Mention);
                        await e.Channel.SendMessageAsync(e.Author.Mention + " wollte sagen: " + newMessage);
                        _ = Mute(e.Channel, (DiscordMember)e.Author, g, 1);
                        await e.DeleteAsync();
                        return;
                    }


                    if (e.Content.StartsWith(".kiss") || e.Content.StartsWith(".kizz"))
                    {
                        var userIds = e.MentionedUsers;
                        foreach (var userID in userIds)
                        {
                            if (userID.Id == Bot.userIdvfs)
                            {
                                if (g.Id == Bot.guildIdUnbi)
                                {
                                    await Mute(e.Channel, (DiscordMember)e.Author, g).ConfigureAwait(false);
                                }
                            }
                        }
                    }

                }

                var msgArr = e.Content.Split();
                if (msgArr.Length >= 3)
                {
                    if (msgArr[0] == "was" && msgArr[^1] == "sagt")
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
            if (e.Guild.Id == Bot.guildIdUnbi)
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

        private static void ClearMessageCache()
        {
            var sender = Client;
            while (true)
            {
                ArchivMessages.RemoveAll(item => item.Timestamp.AddMinutes(10) < DateTimeOffset.Now);
                Thread.Sleep(1000 * 60 * 5);
            }

        }

        public static Task RemoveUserfromMessageArchiv(ulong userId)
        {
            ArchivMessages.RemoveAll(item => item.AuthorId == userId);
            return Task.CompletedTask;
        }

        private Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(e.Exception, "Exception aufgetreten");
            return Task.CompletedTask;
        }

        public static bool CheckBadWords(string Message)
        {
            if (!DoCheckBadWords)
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
                    if (Message.ToLower().Contains(item.ToLower()))
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

        public static string ReplaceBadwords(string Message)
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

                    if (teststring.ToLower().Contains(item.ToLower()))
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

        public static async Task Mute(DiscordChannel channel, DiscordMember target, DiscordGuild g, int durationMin = 10)
        {
            if (target.Id == Bot.botIdSelf || g.Id != Bot.guildIdUnbi)
            {
                return;
            }
            try
            {
                var roleMuted = g.GetRole(Bot.roleIdMuted);
                _ = target.GrantRoleAsync(roleMuted);
                _ = Bot.RemoveUserfromMessageArchiv(target.Id);

                var outChannel = g.GetChannel(Bot.channelIdRotz);
                await outChannel.SendMessageAsync(target.Mention + " jetzt für " + durationMin + " Minute(n) still").ConfigureAwait(false);
                Thread.Sleep(1000 * 60 * durationMin);
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
    }
}
