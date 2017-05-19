using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
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
                cmds.Add(mesdat[i]);
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
                "`help`, `denizen`, `getstarted` "
                + "...";

        public void CMD_Help(string[] cmds, SocketMessage message)
        {
            message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Available Commands:\n" + CmdsHelp).Wait();
        }

        public void CMD_GetStarted(string[] cmds, SocketMessage message)
        {
            message.Channel.SendMessageAsync(POSITIVE_PREFIX + "Hey! Currently your best hope at getting started with Denizen2 "
                    + "is just talking to us here on Discord! We're working on tutorial videos, which will be available at "
                    + "https://forum.denizenscript.com/viewtopic.php?f=12&t=57").Wait();
        }

        public void CMD_Denizen(string[] cmds, SocketMessage message)
        {
            EmbedBuilder bed = new EmbedBuilder();
            EmbedAuthorBuilder auth = new EmbedAuthorBuilder();
            auth.Name = "Denizen Script";
            auth.IconUrl = Client.CurrentUser.GetAvatarUrl();
            auth.Url = "https://denizenscript.com";
            bed.Author = auth;
            bed.Color = new Color(0x00, 0xAA, 0xFF);
            bed.Title = "Denizen Scripting Engine";
            bed.Description = "The Denizen Scripting Engine is a powerful script system primarily used for writing complex Minecraft mods.";
            bed.AddField((efb) => efb.WithName("How do I get started using this system?")
                    .WithValue("@ me (" + Client.CurrentUser.Username + ") with the message: getstarted"));
            bed.Footer = new EmbedFooterBuilder().WithIconUrl(auth.IconUrl).WithText("Copyright (C) Denizen Script Team");
            message.Channel.SendMessageAsync(POSITIVE_PREFIX, embed: bed.Build()).Wait();
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
        }

        public DiscordBot(string code, string[] args)
        {
            Console.WriteLine("Discord bot setting up...");
            DefaultCommands();
            Client = new DiscordSocketClient();
            Client.Ready += () =>
            {
                Console.WriteLine("Args: " + args.Length);
                if (args.Length > 0 && ulong.TryParse(args[0], out ulong a1))
                {
                    ISocketMessageChannel chan = (Client.GetChannel(a1) as ISocketMessageChannel);
                    Console.WriteLine("Restarted as per request in channel: " + chan.Name);
                    chan.SendMessageAsync(POSITIVE_PREFIX + "Connected and ready!").Wait();
                }
                return Task.CompletedTask;
            };
            Client.MessageReceived += (message) =>
            {
                if (message.Author.Id == Client.CurrentUser.Id)
                {
                    return Task.CompletedTask;
                }
                if (message.Channel.Name.StartsWith("@") || !(message.Channel is SocketGuildChannel))
                {
                    Console.WriteLine("Refused message from (" + message.Author.Username + "): (Invalid Channel: " + message.Channel.Name + "): " + message.Content);
                    return Task.CompletedTask;
                }
                bool mentionedMe = false;
                foreach (SocketUser user in message.MentionedUsers)
                {
                    if (user.Id == Client.CurrentUser.Id)
                    {
                        mentionedMe = true;
                        break;
                    }
                }
                Console.WriteLine("Parsing message from (" + message.Author.Username + "), in channel: " + message.Channel.Name + ": " + message.Content);
                if (mentionedMe)
                {
                    Respond(message);
                }
                return Task.CompletedTask;
            };
            Console.WriteLine("Discord bot loading...");
            Client.StartAsync().Wait();
        }
    }
}
