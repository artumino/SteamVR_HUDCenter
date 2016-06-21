using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Valve.VR;
using SteamVR_HUDCenter.Elements;

namespace SteamVR_HUDCenter
{
    //TODO: Move everything to an Active class that also manages OpenVR Initialization
    public class OpenVR_Utils
    {
        private static List<Handlable> RegisteredItems = new List<Handlable>(1);
        private static List<uint> Notifications = new List<uint>(1);

        //Register new item to our list, this method is purely used to avoid duplicates in our list
        public static void RegisterNewItem(Handlable item)
        {
            if (RegisteredItems.Contains<Handlable>(item))
                throw new Exception("Item with the same name and type already registered.");
            AssingHandle(item);
            RegisteredItems.Add(item);
        }

        public static IEnumerable<Overlay> GetRegisteredOverlays()
        {
            return RegisteredItems.Where(hand => hand is Overlay).Select<Handlable, Overlay>(hand => (Overlay)hand);
        }

        public static uint DisplayNotification(string Message, Overlay Overlay, EVRNotificationType Type, EVRNotificationStyle Style, NotificationBitmap_t Bitmap)
        {
            uint ID = GetNewNotificationID();
            OpenVR.Notifications.CreateNotification(Overlay.Handle, 0, Type, Message, Style, ref Bitmap, ref ID);
            Notifications.Add(ID);
            return ID;
        }

        public static void ClearNotifications()
        {
            foreach (uint ID in Notifications)
                OpenVR.Notifications.RemoveNotification(ID);
            Notifications.Clear();
        }

        //Double checks internal handles to reduce Steam work
        private static void AssingHandle(Handlable item)
        {
            Random rnd = new Random();
            do
            {
                item.Handle = (ulong)rnd.Next();
                foreach (Handlable hd in RegisteredItems)
                    if (hd.Handle == item.Handle)
                    {
                        item.Handle = 0;
                        break;
                    }
            } while (item.Handle == 0);
        }
        
        private static uint GetNewNotificationID()
        {
            Random rnd = new Random();
            uint ID = 0;
            do
            {
                ID = (uint)rnd.Next();
                foreach (uint nID in Notifications)
                    if (nID == ID)
                    {
                        ID = 0;
                        break;
                    }
            } while (ID == 0);
            return ID;
        }
    }
}
