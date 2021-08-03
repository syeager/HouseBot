using System.IO;
using System.Linq;

namespace HouseBot.Client.Services
{
    public class GetBotIcon
    {
        private const string NetworkPath = "//BATCAVE/Public/HouseBot/BotIcons";
        private const string FallbackEmotion = "grin.ico";

        public string ForEmotion(string emotion)
        {
            emotion = emotion.Trim().ToLower();
            string iconPath = Directory.EnumerateFiles(NetworkPath, $"{emotion}.ico")
                .FirstOrDefault();

            iconPath ??= Path.Combine(NetworkPath, FallbackEmotion);
            return iconPath;
        }
    }
}