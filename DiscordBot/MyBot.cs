using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.Audio;
using Discord;
using System.Diagnostics;

namespace SheepuBot
{ 

    public class Commands : ModuleBase<ICommandContext>

    {

        private static int[] a = { 51, 0, 51 };
        private static int[] b = { 251, 184, 41 };
        private static int[] c = { 251, 79, 41 };
        private static int[] d = { 253, 202, 190 };
        private static int[] e = { 190, 253, 202 };
        private static int[] f = { 202, 190, 253 };
        private static int[] g = { 253, 234, 190 };

        private static Random rand = new Random();

        private CommandService _service;

        public Commands(CommandService service)
        {
            _service = service;
        }
        /*
        private Process CreateStream(string path)
    {
    var ffmpeg = new ProcessStartInfo
    {
        FileName = "ffmpeg",
        Arguments = $"-i {path} -ac 2 -f s16le -ar 48000 pipe:1",
        UseShellExecute = false,
        CreateNoWindow = false,
        RedirectStandardOutput = true,
        RedirectStandardError = false
    };

    return Process.Start(ffmpeg);
}
        [Command("playah", RunMode = RunMode.Async)]
        public async Task play(string url)
        {
            IVoiceChannel channel = (Context.User as IVoiceState).VoiceChannel;
            IAudioClient client = await channel.ConnectAsync();

            var output = CreateStream(url).StandardOutput.BaseStream;
            var stream = client.CreatePCMStream(AudioApplication.Music);
            await output.CopyToAsync(stream);
            await stream.FlushAsync().ConfigureAwait(false);
        }

        [Command("playme", RunMode = RunMode.Async)]
        public async Task Playme(string song)
        {
            await _aservice.SendAudioAsync(Context.Guild, Context.Channel, song);
            IVoiceChannel channel = (Context.User as IVoiceState).VoiceChannel;
            IAudioClient client = await channel.ConnectAsync();

            var output = CreateStream(song).StandardOutput.BaseStream;
            var stream = client.CreatePCMStream(AudioApplication.Music);
            await output.CopyToAsync(stream);
            await stream.FlushAsync().ConfigureAwait(false);
        }
        
        
        
        [Command("join", RunMode = RunMode.Async)]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            // Get the audio channel
            channel = channel ?? (Context.User as IGuildUser)?.VoiceChannel;
            if (channel == null) { await Context.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
            var audioClient = await channel.ConnectAsync();
        }






            */

        [Command("Tell")]
        [Alias("tell")] 
        [Summary("returns said sentence!")]
        public async Task Say(IUser user)
        {
            var channel = await user.GetOrCreateDMChannelAsync();
            await channel.SendMessageAsync("Lemon for u :lemon:");
        }

        [Command("ayy")]
        public async Task ayy()
        {
            await ReplyAsync("lmao :alien:");
        }

        [Command("Test")]
        public async Task Test()
        {
        }

        [Command("hello")]
        [Summary("say hello!")]
        [Alias("hi")]
        public async Task Hello()
        {
            await ReplyAsync("Hææææ " + Context.Message.Author.Mention + "! :sheep:");
        }

        /*[Command("purge")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Alias("clear", "delete")]
        public async Task Purge([Remainder] int num = 0)
        {
            if (num <= 100)
            {
                var messagesToDelete = await Context.Channel.GetMessagesAsync(num + 1).Flatten();
                await Context.Channel.DeleteMessagesAsync(messagesToDelete);
                if (num == 1)
                {
                    await Context.Channel.SendMessageAsync(Context.User.Username + " deleted 1 message.");
                }
                else
                {
                    await Context.Channel.SendMessageAsync(Context.User.Username + " deleted " + num + " messages.");
                }
            }
            else
            {
                await ReplyAsync("You can't delete more than 100 messages~!");
            }
        }*/



        [Command("help")]
        [Remarks("Shows what a specific command does and what parameters it takes.")]
        public async Task HelpAsync([Remainder] string command = null)
        {
            if (command == null)
            {
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync(); /* A channel is created so that the commands will be privately sent to the user, and not flood the chat. */

                string prefix = "?";  /* put your chosen prefix here */
                var builder = new EmbedBuilder()
                {
                    Color = new Color(rand.Next(255), rand.Next(255), rand.Next(255)),
                    Description = "These are the commands you can use"
                };

                foreach (var module in _service.Modules) /* we are now going to loop through the modules taken from the service we initiated earlier ! */
                {
                    string description = null;
                    foreach (var cmd in module.Commands) /* and now we loop through all the commands per module aswell, oh my! */
                    {
                        var result = await cmd.CheckPreconditionsAsync(Context); /* gotta check if they pass */
                        if (result.IsSuccess)
                            description += $"{prefix}{cmd.Aliases.First()}\n"; /* if they DO pass, we ADD that command's first alias (aka it's actual name) to the description tag of this embed */
                    }

                    if (!string.IsNullOrWhiteSpace(description)) /* if the module wasn't empty, we go and add a field where we drop all the data into! */
                    {
                        builder.AddField(x =>
                        {
                            x.Name = module.Name;
                            x.Value = description;
                            x.IsInline = false;
                        });
                    }
                }
                await dmChannel.SendMessageAsync("", false, builder.Build()); /* then we send it to the user. */

            }
            else
            {
                var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
                var result = _service.Search(Context, command);

                if (!result.IsSuccess)
                {
                    await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                    return;
                }

                var builder = new EmbedBuilder()
                {
                    Color = new Color(rand.Next(255), rand.Next(255), rand.Next(255)),
                    Description = $"Here are some commands like **{command}**"
                };

                foreach (var match in result.Commands)
                {
                    var cmd = match.Command;

                    builder.AddField(x =>
                    {
                        x.Name = string.Join(", ", cmd.Aliases);
                        x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Remarks: {cmd.Remarks}";
                        x.IsInline = false;
                    });
                }
                await dmChannel.SendMessageAsync("", false, builder.Build());
            }
        }

    }
}
