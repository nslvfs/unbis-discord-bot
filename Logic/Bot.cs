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
        public static bool googlemode { get; set; }
        public static bool silentMode { get; set; }
        public static bool randomMode { get; set; }
        public static bool cryptoMode { get; set; }
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
            googlemode = true;

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
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

            Client.ClientErrored += Client_ClientError;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.MessageCreated += Client_MessageCreated;
            Client.MessageUpdated += Client_MessageUpdated;
            Client.MessageReactionAdded += Client_ReactionAdded;
            Client.GuildMemberUpdated += Client_GuildMemberUpdated;

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;
            // Commands.SetHelpFormatter<Logic.HelpFormatter>();
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<BadWords>();
            Commands.RegisterCommands<Flaschendrehen>();
            Commands.RegisterCommands<Gurgle>();
            Commands.RegisterCommands<JaNein>();
            Commands.RegisterCommands<NslBlog>();
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

        private Task Client_ReactionAdded(DiscordClient sender, MessageReactionAddEventArgs e)
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
            return Task.CompletedTask;
        }

        private Task OnClientReady(DiscordClient sender, ReadyEventArgs e)
        {
            sender.Logger.LogInformation(BotEventId, "Client läuft");
            silentMode = false;
            randomMode = false;
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
            bool deleted = false;
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
                if (e.Author.Id != 807641560006000670 && e.Author.Id != 134719067016658945 && silentMode && e.Channel.Id != 812403060416446474)
                {
                    _ = Mute(e, g, 3);
                    await e.DeleteAsync();
                }
                if (e.Author.Id != 807641560006000670 && e.Author.Id != 134719067016658945 && randomMode && !e.Author.IsBot)
                {
                    string rnd = Shared.GenerateRandomNumber(1000, 9999).ToString();

                    await ((DiscordMember)e.Author).ModifyAsync(x =>
                    {
                        x.Nickname = rnd;
                        x.AuditLogReason = $"Changed by Random.Mode).";
                    });
                }
                if (checkBadWords(e.Content))
                {
                    await e.DeleteAsync();
                    deleted = true;
                    await e.Channel.SendMessageAsync("Ah ah aaaah das sagen wir hier nicht! " + e.Author.Mention).ConfigureAwait(false);
                    _ = Mute(e, g, 1);
                }
                if(cryptoMode && !deleted && !e.Author.IsBot)
                {
                    try
                    {
                        string message = e.Content;
                        string author = ((DiscordMember)e.Author).DisplayName;
                        string dtNow = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
                        await e.DeleteAsync();
                        Console.WriteLine("Cleartext: " + dtNow + "| " + author + ": " + message);
                        string encryptedstring = Logic.Encryption.Encrypt(message, configJson.cryptoPwd);
                        await e.Channel.SendMessageAsync(author + ": " + encryptedstring).ConfigureAwait(false);
                        
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                }
            }
            if (e.Content.StartsWith(".kiss") || e.Content.StartsWith(".kizz"))
            {
                var userIds = e.MentionedUsers;
                foreach(var userID in userIds)
                {
                    if(userID.Id == 134719067016658945)
                    {
                        if (g.Id == 791393115097137171)
                        {
                            _ = Mute(e, g);
                        }
                    }
                }
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

        private async Task Client_GuildMemberUpdated(DiscordClient sender, GuildMemberUpdateEventArgs e)
        {
            if (e.Guild.Id == 791393115097137171)
            {
                if (checkBadWords(e.NicknameAfter))
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
            var fileName = configJson.badwords;
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
            foreach (var item in badWords)
            {
                if (Message.Contains(item))
                    return true;
                if (Message.ToLower().Contains(item.ToLower()))
                    return true;
                var msg = Regex.Replace(Message, @"([^\w]|_)", "");
                if (msg.Contains(item))
                    return true;
            }
            return false;
        }

        public static async Task Mute(DiscordMessage e, DiscordGuild g, int durationMin = 10)
        {
            var roleMuted = g.GetRole(807921762570469386);
            await ((DiscordMember)e.Author).GrantRoleAsync(roleMuted);
            Bot.RemoveUserfromMessageArchiv(e.Author.Id);
            await e.Channel.SendMessageAsync(e.Author.Mention + " jetzt für " + durationMin + " Minuten gemuted").ConfigureAwait(false);
            Thread.Sleep(1000 * 60 * durationMin);
            await ((DiscordMember)e.Author).RevokeRoleAsync(roleMuted);
            await e.Channel.SendMessageAsync(e.Author.Mention + " jetzt nicht mehr gemuted").ConfigureAwait(false);
        }
    }
}
