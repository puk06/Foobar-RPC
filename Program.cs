using System.Diagnostics;
using DiscordRPC;

namespace Foobar_RPC
{
    internal class Program
    {
        private static readonly DiscordRpcClient Client = new("1277600550233309204");

        private static void Main()
        {
            const string software = "foobar2000.exe";
            string previousWindowTitle = string.Empty;

            bool isConnectedToDiscord = false;

            while (!isConnectedToDiscord)
            {
                try
                {
                    Client.Initialize();
                    isConnectedToDiscord = true;
                    Console.WriteLine("Connected to Discord!");
                }
                catch
                {
                    Console.WriteLine("Failed to connect to Discord. Retrying in 5 seconds...");
                    Thread.Sleep(5000);
                }
            }

            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    string windowTitle = GetWindowTitleByProcessName(software);
                    if (!string.IsNullOrEmpty(windowTitle))
                    {
                        if (windowTitle == "foobar2000")
                        {
                            if (previousWindowTitle == "foobar2000") continue;
                            previousWindowTitle = "foobar2000";

                            Console.WriteLine("Foobar2000 is open but not playing anything.");
                            Client.SetPresence(new RichPresence()
                            {
                                Details = "Idle",
                                State = "Not listening to music",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "foobar_idle",
                                    LargeImageText = "Foobar2000 by Peter Pawlowski"
                                }
                            });
                        }
                        else
                        {
                            string songTitle = GetSongTitle(windowTitle);
                            if (songTitle == previousWindowTitle) continue;
                            previousWindowTitle = songTitle;

                            Console.WriteLine("Song Change Detected! Now Playing: " + songTitle);
                            Client.SetPresence(new RichPresence()
                            {
                                Details = songTitle,
                                State = "Listening to music",
                                Assets = new Assets()
                                {
                                    LargeImageKey = "foobar_playing",
                                    LargeImageText = "Foobar2000 by Peter Pawlowski"
                                }
                            });
                        }
                    }
                    else
                    {
                        Client.ClearPresence();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static string GetWindowTitleByProcessName(string executableName)
        {
            string processName = Path.GetFileNameWithoutExtension(executableName);
            var processes = Process.GetProcessesByName(processName);

            foreach (Process process in processes)
            {
                if (!string.IsNullOrEmpty(process.MainWindowTitle))
                {
                    return process.MainWindowTitle;
                }
            }

            return null;
        }

        private static string GetSongTitle(string windowTitle) => windowTitle[..^12];
    }
}
