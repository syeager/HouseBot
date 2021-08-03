using System.Threading.Tasks;
using HouseBot.Client.Events;
using HouseBot.Client.Services;
using HouseBot.Data.Events;

namespace HouseBot.Client.Consumers
{
    internal sealed class WindowsNotificationConsumer : EventConsumer<WindowsNotificationData>
    {
        private readonly PostWindowsNotification postWindowsNotification;
        private readonly GetBotIcon getBotIcon;

        public WindowsNotificationConsumer()
        {
            postWindowsNotification = new PostWindowsNotification();
            getBotIcon = new GetBotIcon();
        }

        protected override Task ConsumeAsync(WindowsNotificationData eventData)
        {
            string iconPath = getBotIcon.ForEmotion(eventData.BotIcon);
            postWindowsNotification.Notify("HouseBot", eventData.Message, iconPath);
            return Task.CompletedTask;
        }
    }
}