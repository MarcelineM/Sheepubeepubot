using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SheepuBot
{
    class Program
    {
        static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();




        string token = "MzI2Mjc4MDkyNTc2MzkxMTY5.DK4wzg.xmUhSNLxG4cXY-kHKgR0vVhCqXI";
        private AudioService _service;
        private CommandService commands;
        private DiscordSocketClient client;
        private IServiceProvider services;

        public async Task Start()
        {
            commands = new CommandService();
            _service = new AudioService();

            

            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            services = new ServiceCollection().BuildServiceProvider();


            await InstallCommands();

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            client.Log += Log;
            client.UserJoined += UserJoined;

            await Task.Delay(-1);
        }

        public async Task UserJoined(SocketGuildUser user)
        {
            var caca = client.GetGuild(346056010521378818);
            var channel = client.GetChannel(348816949867839494) as SocketTextChannel;
            var rules = client.GetChannel(365146388973223946) as SocketTextChannel;
            var guest = user.Guild.Roles.Where(input => input.Name.ToUpper() == "SPOOPY BOOB").FirstOrDefault() as SocketRole;

            await user.AddRoleAsync(guest);
            await channel.SendMessageAsync("Welcome " + user.Mention + "! :sheep:");
        }

        public async Task InstallCommands()
        {
            // Hook the MessageReceived Event into our Command Handler
            client.MessageReceived += HandleCommand;
            // Discover all of the commands in this assembly and load them.
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        public async Task HandleCommand(SocketMessage msgParam)
        {
            var msg = msgParam as SocketUserMessage;
            char prefix = '!';
            if (msg == null) return;

            int argPos = 0;

            if (!(msg.HasCharPrefix(prefix, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos))) return;
            // Create a command context apparently
            var context = new CommandContext(client, msg);
            //execute the command (result does not indicate return value,
            //rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);

        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }

            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message}");
            Console.ForegroundColor = ConsoleColor.White;

            return Task.CompletedTask;
        }

    }
}
