using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;
using System;

public class AudioService
{
    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

    public async Task JoinAudio(IGuild guild, IVoiceChannel target)
    {
        IAudioClient client;
        if (ConnectedChannels.TryGetValue(guild.Id, out client))
        {
            return;
        }
        if (target.Guild.Id != guild.Id)
        {
            return;
        }

        var audioClient = await target.ConnectAsync();

        if (ConnectedChannels.TryAdd(guild.Id, audioClient))
        {
            // If you add a method to log happenings from this service,
            // you can uncomment these commented lines to make use of that.
            //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
        }
    }

    public async Task LeaveAudio(IGuild guild)
    {
        IAudioClient client;
        if (ConnectedChannels.TryRemove(guild.Id, out client))
        {
            await client.StopAsync();
            //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
        }
    }

    

    public Process CreateStream(string path)
    {
        var ffmpeg = new ProcessStartInfo
        {
           FileName = "ffmpeg",
        Arguments = $"-i {path} -ac 2 -f s16le -ar 48000 pipe:1",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        };
        return Process.Start(ffmpeg);
    }
    private ConcurrentDictionary<ulong, AudioService> voiceSets = new ConcurrentDictionary<ulong, AudioService>();

    IAudioClient audioClient;
    public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
    {
        Console.WriteLine("1");
        // Create FFmpeg using the previous example
        var ffmpeg = CreateStream(path);
        var output = ffmpeg.StandardOutput.BaseStream;
        var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
        await output.CopyToAsync(discord);
        await discord.FlushAsync();
        Console.WriteLine("2");
        Console.WriteLine("2");
        Console.WriteLine("2");
        Console.WriteLine("2");
        Console.WriteLine("2");
    }
}