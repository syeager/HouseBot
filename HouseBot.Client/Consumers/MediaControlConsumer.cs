using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using HouseBot.Client.Events;
using HouseBot.Client.Services;
using HouseBot.Data.Events;

namespace HouseBot.Client.Consumers
{
    internal sealed class MediaControlConsumer : EventConsumer<MediaControlData>
    {
        private readonly PostWindowsNotification postWindowsNotification = new();
        private readonly GetBotIcon getBotIcon = new();

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);

        protected override Task ConsumeAsync(MediaControlData eventData)
        {
            switch(eventData.Control)
            {
                case MediaControl.None:
                    throw new NotSupportedException();
                case MediaControl.Play:
                case MediaControl.Pause:
                    Press(0xB3, eventData.Control);
                    break;
                case MediaControl.Stop:
                    Press(0xB2, eventData.Control);
                    break;
                case MediaControl.Next:
                    Press(0xB0, eventData.Control);
                    break;
                case MediaControl.Previous:
                    Press(0xB1, eventData.Control);
                    break;
                case MediaControl.Mute:
                    Press(0xAD, eventData.Control);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return Task.CompletedTask;
        }

        private void Press(byte key, MediaControl control)
        {
            keybd_event(key, 0, 0, IntPtr.Zero);

            var iconPath = getBotIcon.ForEmotion("grin");
            postWindowsNotification.Notify("Media control invoked", control.ToString(), iconPath);
        }
    }
}