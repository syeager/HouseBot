using System.Drawing;
using System.Windows.Forms;

namespace HouseBot.Client.Services
{
    public class PostWindowsNotification
    {
        public void Notify(string title, string text, string iconPath)
        {
            var icon = new NotifyIcon
            {
                Icon = new Icon(iconPath),
                Visible = true,
                BalloonTipTitle = title,
                BalloonTipText = text,
                BalloonTipIcon = ToolTipIcon.Info,
            };

            icon.ShowBalloonTip(2000);
        }
    }
}