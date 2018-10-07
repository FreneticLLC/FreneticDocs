using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Discord;
using Discord.Net;
using Discord.WebSocket;

namespace FreneticDocs.Models
{
    public class DiscordBot
    {
        public const string POSITIVE_PREFIX = "+++ ";
        
        public const string NEGATIVE_PREFIX = "--- ";

        public const string INFO_PREFIX = "=== ";

        public DiscordSocketClient Client;

        public static Dictionary<string, Action<string[], SocketMessage>> CommonCmds = new Dictionary<string, Action<string[], SocketMessage>>(1024);

        public void Respond(SocketMessage message)
        {
            string[] mesdat = message.Content.Split(' ');
            StringBuilder resBuild = new StringBuilder(message.Content.Length);
            List<string> cmds = new List<string>();
            for (int i = 0; i < mesdat.Length; i++)
            {
                if (mesdat[i].Contains("<") && mesdat[i].Contains(">"))
                {
                    continue;
                }
                resBuild.Append(mesdat[i]).Append(" ");
                if (mesdat[i].Length > 0)
                {
                    cmds.Add(mesdat[i]);
                }
            }
            if (cmds.Count == 0)
            {
                Console.WriteLine("Empty input, ignoring: " + message.Author.Username);
                return;
            }
            string fullMsg = resBuild.ToString();
            Console.WriteLine("Found input from: (" + message.Author.Username + "), in channel: " + message.Channel.Name + ": " + fullMsg);
            string lowCmd = cmds[0].ToLowerInvariant();
            cmds.RemoveAt(0);
            if (CommonCmds.TryGetValue(lowCmd, out Action<string[], SocketMessage> acto))
            {
                acto.Invoke(cmds.ToArray(), message);
            }
            else
            {
                message.Channel.SendMessageAsync(NEGATIVE_PREFIX + "Unknown command. Consider the __**help**__ command?").Wait();
            }
        }

        public string CmdsHelp = 
                "`help`, `denizen`, `getstarted`, `restart`, `reload`, `update`, "
                + "`command`, "
                + "...";

        public void CMD_Help(string[] cmds, SocketMessage message)
        {
            message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Available Commands:\n" + CmdsHelp).Wait();
        }

        public void CMD_GetStarted(string[] cmds, SocketMessage message)
        {
            // TODO: Generic this via config.
            message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Hey! Currently your best hope at getting started with Denizen2 "
                    + "is just talking to us here on Discord! We're working on tutorial videos, which will be available at "
                    + "https://forum.denizenscript.com/viewtopic.php?f=12&t=57").Wait();
        }

        public void CMD_Denizen(string[] cmds, SocketMessage message)
        {
            // TODO: Generic this via config.
            EmbedBuilder bed = new EmbedBuilder();
            EmbedAuthorBuilder auth = new EmbedAuthorBuilder();
            auth.Name = "Denizen Script Info";
            auth.IconUrl = Client.CurrentUser.GetAvatarUrl();
            auth.Url = "https://denizenscript.com";
            bed.Author = auth;
            bed.Color = new Color(0x00, 0xAA, 0xFF);
            bed.Title = "Denizen Scripting Engine";
            bed.Description = "The Denizen Scripting Engine is a powerful script system primarily used for writing complex Minecraft mods.";
            bed.AddField((efb) => efb.WithName("How do I get started using this system?")
                    .WithValue("To get started, please @ me (" + Client.CurrentUser.Username + ") with the message: getstarted"));
            bed.Footer = new EmbedFooterBuilder().WithIconUrl(auth.IconUrl).WithText("Copyright (C) Denizen Script Team");
            message.Channel.SendMessageAsync(POSITIVE_PREFIX, embed: bed.Build()).Wait();
        }

        void OutputMeta(SocketMessage src, string type, string url, string name, string shortinfo, List<KeyValuePair<string, string>> fields)
        {
            EmbedBuilder bed = new EmbedBuilder();
            EmbedAuthorBuilder auth = new EmbedAuthorBuilder();
            auth.Name = "Meta Docs: " + type;
            auth.IconUrl = Client.CurrentUser.GetAvatarUrl();
            auth.Url = DocsStatic.Config["mainurl"] + url;
            bed.Author = auth;
            bed.Color = new Color(0x00, 0xAA, 0xFF);
            bed.Title = type + ": " + name;
            bed.Description = shortinfo;
            foreach (KeyValuePair<string, string> pair in fields)
            {
                bed.AddField((efb) => efb.WithName(pair.Key).WithValue(pair.Value));
            }
            bed.Footer = new EmbedFooterBuilder().WithIconUrl(auth.IconUrl).WithText("Click the title of this box for more information...");
            src.Channel.SendMessageAsync(POSITIVE_PREFIX, embed: bed.Build()).Wait();
        }

        public void CMD_Command(string[] cmds, SocketMessage message)
        {
            string arg = cmds[0].ToLowerInvariant();
            IEnumerable<ScriptCommand> res = DocsStatic.Meta.Commands.Where((c) => c.Name.Contains(arg));
            ScriptCommand cmdLikely = res.FirstOrDefault();
            if (cmdLikely == null)
            {
                    message.Channel.SendMessageAsync(NEGATIVE_PREFIX + "That's an unknown command!");
            }
            if (res.Count() > 1)
            {
                ScriptCommand exact = DocsStatic.Meta.Commands.Where((c) => c.Name == arg).FirstOrDefault();
                if (exact == null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (ScriptCommand cmd in res)
                    {
                        sb.Append("`").Append(cmd.Name).Append("`, ");
                    }
                    message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Found " + res.Count() + " possible matches: " + sb.ToString().Substring(0, sb.Length - 2));
                }
                cmdLikely = exact;
            }
            List<KeyValuePair<string, string>> fields = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Description", CapOff(cmdLikely.Description, 256)),
                new KeyValuePair<string, string>("Arguments", cmdLikely.Arguments)
            };
            OutputMeta(message, "Command", "/Home/Commands/?search=" + cmdLikely.Name, cmdLikely.Name, cmdLikely.Short, fields);
        }

        public string CapOff(string inp, int len)
        {
            return inp.Length > len ? inp.Substring(0, len) + "... (View Link For More)" : inp;
        }

        bool IsBotCommander(SocketUser usr)
        {
            return (usr as SocketGuildUser).Roles.Where((role) => role.Name.ToLowerInvariant() =="botcommander").FirstOrDefault() != null;
        }

        void CMD_Update(string[] cmds, SocketMessage message)
        {
            // NOTE: This implies a one-guild bot. A multi-guild bot probably shouldn't have this "BotCommander" role-based verification.
            // But under current scale, a true-admin confirmation isn't worth the bother.
            if (!IsBotCommander(message.Author))
            {
                message.Channel.SendMessageAsync(NEGATIVE_PREFIX + "Nope! That's not for you!").Wait();
                return;
            }
            if (!File.Exists("./update.sh"))
            {
                message.Channel.SendMessageAsync(NEGATIVE_PREFIX + "Nope! That's not valid for my current configuration!").Wait();
                return;
            }
            message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Yes, boss. updating now...").Wait();
            Process res = Process.Start("sh", "./update.sh");
            Task.Factory.StartNew(() =>
            {
                bool waitedResult = res.WaitForExit(60000);
                message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Update process returned result: " + waitedResult + " with code " + res.ExitCode).Wait();
            });
        }

        void CMD_Restart(string[] cmds, SocketMessage message)
        {
            // NOTE: This implies a one-guild bot. A multi-guild bot probably shouldn't have this "BotCommander" role-based verification.
            // But under current scale, a true-admin confirmation isn't worth the bother.
            if (!IsBotCommander(message.Author))
            {
                message.Channel.SendMessageAsync(NEGATIVE_PREFIX + "Nope! That's not for you!").Wait();
                return;
            }
            if (!File.Exists("./start.sh"))
            {
                message.Channel.SendMessageAsync(NEGATIVE_PREFIX + "Nope! That's not valid for my current configuration!").Wait();
                return;
            }
            message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Yes, boss. Restarting now...").Wait();
            Process.Start("sh", "./start.sh " + message.Channel.Id);
            Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Shutdown start...");
                for (int i = 0; i < 15; i++)
                {
                    Console.WriteLine("T Minus " + (15 - i));
                    Task.Delay(1000).Wait();
                }
                Console.WriteLine("Shutdown!");
                Environment.Exit(0);
            });
            Client.StopAsync().Wait();
        }

        void CMD_Reload(string[] cmds, SocketMessage message)
        {
            // NOTE: This implies a one-guild bot. A multi-guild bot probably shouldn't have this "BotCommander" role-based verification.
            // But under current scale, a true-admin confirmation isn't worth the bother.
            if (!IsBotCommander(message.Author))
            {
                message.Channel.SendMessageAsync(NEGATIVE_PREFIX + "Nope! That's not for you!").Wait();
                return;
            }
            message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Yes, boss. reloading now...").Wait();
            DocsMeta metaTemp = Startup.LoadMeta();
            lock (DocsStatic.MetaInternal)
            {
                DocsStatic.MetaInternal = metaTemp;
            }
            message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Reloaded!").Wait();
        }

        public void DefaultCommands()
        {
            CommonCmds["help"] = CMD_Help;
            CommonCmds["halp"] = CMD_Help;
            CommonCmds["helps"] = CMD_Help;
            CommonCmds["halps"] = CMD_Help;
            CommonCmds["hel"] = CMD_Help;
            CommonCmds["hal"] = CMD_Help;
            CommonCmds["h"] = CMD_Help;
            CommonCmds["who"] = CMD_Denizen;
            CommonCmds["what"] = CMD_Denizen;
            CommonCmds["where"] = CMD_Denizen;
            CommonCmds["why"] = CMD_Denizen;
            CommonCmds["denizen"] = CMD_Denizen;
            CommonCmds["llc"] = CMD_Denizen;
            CommonCmds["denizenscript"] = CMD_Denizen;
            CommonCmds["website"] = CMD_Denizen;
            CommonCmds["team"] = CMD_Denizen;
            CommonCmds["getstarted"] = CMD_GetStarted;
            CommonCmds["gs"] = CMD_GetStarted;
            CommonCmds["start"] = CMD_GetStarted;
            CommonCmds["getstart"] = CMD_GetStarted;
            CommonCmds["command"] = CMD_Command;
            CommonCmds["cmd"] = CMD_Command;
            CommonCmds["c"] = CMD_Command;
            CommonCmds["restart"] = CMD_Restart;
            CommonCmds["update"] = CMD_Update;
            CommonCmds["reload"] = CMD_Reload;
        }

        public bool ConnectedOnce = false;

        public bool ConnectedCurrently = false;

        public HashSet<ulong> ValidChannels = new HashSet<ulong>(32);

        public void PopulateFromConfig()
        {
            ValidChannels.Clear();
            string[] channelWhitelist = DocsStatic.Config["channel_whitelist"].Split('|');
            foreach (string channel in channelWhitelist)
            {
                ValidChannels.Add(ulong.Parse(channel.Trim()));
            }
        }

        public DiscordBot(string code, string[] args)
        {
            Console.WriteLine("Discord bot setting up...");
            PopulateFromConfig();
            DefaultCommands();
            Client = new DiscordSocketClient();
            Client.Ready += () =>
            {
                if (StopAllLogic)
                {
                    return Task.CompletedTask;
                }
                ConnectedCurrently = true;
                if (ConnectedOnce)
                {
                    return Task.CompletedTask;
                }
                Console.WriteLine("Args: " + args.Length);
                if (args.Length > 0 && ulong.TryParse(args[0], out ulong a1))
                {
                    ISocketMessageChannel chan = (Client.GetChannel(a1) as ISocketMessageChannel);
                    Console.WriteLine("Restarted as per request in channel: " + chan.Name);
                    chan.SendMessageAsync(POSITIVE_PREFIX + "Connected and ready!").Wait();
                }
                ConnectedOnce = true;
                return Task.CompletedTask;
            };
            Client.MessageReceived += (message) =>
            {
                if (StopAllLogic)
                {
                    return Task.CompletedTask;
                }
                if (message.Author.Id == Client.CurrentUser.Id)
                {
                    return Task.CompletedTask;
                }
                LoopsSilent = 0;
                if (message.Author.IsBot || message.Author.IsWebhook)
                {
                    return Task.CompletedTask;
                }
                if (message.Channel.Name.StartsWith("@") || !(message.Channel is SocketGuildChannel))
                {
                    Console.WriteLine("Refused message from (" + message.Author.Username + "): (Invalid Channel: " + message.Channel.Name + "): " + message.Content);
                    return Task.CompletedTask;
                }
                if (ValidChannels.Count != 0 && !ValidChannels.Contains(message.Channel.Id))
                {
                    Console.WriteLine("Refused message from (" + message.Author.Username + "): (Non-whitelisted Channel: " + message.Channel.Name + "): " + message.Content);
                    return Task.CompletedTask;
                }
                bool mentionedMe = message.MentionedUsers.Any((su) => su.Id == Client.CurrentUser.Id);
                Console.WriteLine("Parsing message from (" + message.Author.Username + "), in channel: " + message.Channel.Name + ": " + message.Content);
                if (mentionedMe)
                {
                    Respond(message);
                }
                return Task.CompletedTask;
            };
            Client.Disconnected += (e) =>
            {
                ConnectedCurrently = false;
                return Task.CompletedTask;
            };
            Console.WriteLine("Prepping monitor...");
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Task.Delay(MonitorLoopTime).Wait();
                    if (StopAllLogic)
                    {
                        return;
                    }
                    try
                    {
                        MonitorLoop();
                    }
                    catch (Exception ex)
                    {
                        if (ex is ThreadAbortException)
                        {
                            throw;
                        }
                        Console.WriteLine("Connection monitor loop had exception: " + ex.ToString());
                    }
                }
            });
            Console.WriteLine("Discord bot logging in...");
            Client.LoginAsync(TokenType.Bot, code.Trim()).Wait();
            Console.WriteLine("Discord bot loading...");
            Client.StartAsync().Wait();
        }

        public TimeSpan MonitorLoopTime = new TimeSpan(hours: 0, minutes: 1, seconds: 0);

        public bool MonitorWasFailedAlready = false;

        public bool StopAllLogic = false;

        public void ForceRestartBot()
        {
            lock (MonitorLock)
            {
                StopAllLogic = true;
            }
            Task.Factory.StartNew(() =>
            {
                Client.StopAsync().Wait();
            });
            DocsStatic.DiscBot = new DiscordBot(DocsStatic.Config["discord_bot"], new string[0]);
        }

        public Object MonitorLock = new Object();

        public long LoopsSilent = 0;

        public long LoopsTotal = 0;

        public void MonitorLoop()
        {
            bool isConnected;
            lock (MonitorLock)
            {
                LoopsSilent++;
                LoopsTotal++;
                isConnected = ConnectedCurrently && Client.ConnectionState == ConnectionState.Connected;
            }
            if (!isConnected)
            {
                Console.WriteLine("Monitor detected disconnected state!");
            }
            if (LoopsSilent > 60)
            {
                Console.WriteLine("Monitor detected over an hour of silence, and is assuming a disconnected state!");
                isConnected = false;
            }
            if (LoopsTotal > 60 * 12)
            {
                Console.WriteLine("Monitor detected that the bot has been running for over 12 hours, and will restart soon!");
                isConnected = false;
            }
            if (isConnected)
            {
                MonitorWasFailedAlready = false;
            }
            else
            {
                if (MonitorWasFailedAlready)
                {
                    Console.WriteLine("Monitor is enforcing a restart!");
                    ForceRestartBot();
                }
                MonitorWasFailedAlready = true;
            }
        }
    }
}
