using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SteamVR_HUDCenter;
using SteamVR_HUDCenter.Elements.Forms;

namespace VRTestApplication
{
    public partial class TestForm : VRForm
    {
        private Valve.VR.NotificationBitmap_t notification_icon;
        private Bitmap notification_bitmap; 

        public TestForm()
        {
            InitializeComponent();

            notification_bitmap = new Bitmap(@"Resources\white-lambda.png");
            notification_icon = new Valve.VR.NotificationBitmap_t();
            MouseDown += TestForm_MouseDown;
            MouseUp += TestForm_MouseUp;
        }

        private void TestForm_MouseUp(object sender, MouseEventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("MouseUp " + e.X + "," + e.Y);
        }

        private void TestForm_MouseDown(object sender, MouseEventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("MouseDown " + e.X + "," + e.Y);
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("Button_MouseClick " + e.X + "," + e.Y);
            ShowNotification();
        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("Button_MouseEnter");
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            SteamVR_HUDCenter.UDebug.Log("Button_MouseDown " + e.X + "," + e.Y);
        }

        private void ShowNotification()
        {
            //Locks Image Data & Pass it to OpenVR as notification Icon
            System.Drawing.Imaging.BitmapData TextureData =
            notification_bitmap.LockBits(
                    new Rectangle(0, 0, notification_bitmap.Width, notification_bitmap.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            notification_icon.m_pImageData = TextureData.Scan0;
            notification_icon.m_nWidth = TextureData.Width;
            notification_icon.m_nHeight = TextureData.Height;
            notification_icon.m_nBytesPerPixel = 4;

            //Displays Notification
            Overlay.Controller.DisplayNotification("Hello World!", Overlay, Valve.VR.EVRNotificationType.Transient, Valve.VR.EVRNotificationStyle.Application , notification_icon);

            //Unlocks Image Data
            notification_bitmap.UnlockBits(TextureData);
        }
    }
}
