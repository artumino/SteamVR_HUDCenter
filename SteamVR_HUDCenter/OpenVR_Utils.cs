using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SteamVR_HUDCenter.Elements;

namespace SteamVR_HUDCenter
{
    //TODO: Move everything to an Active class that also manages OpenVR Initialization
    public class OpenVR_Utils
    {
        public static List<Handlable> RegisteredItems = new List<Handlable>(1);

        //Register new item to our list, this method is purely used to avoid duplicates in our list
        public static void RegisterNewItem(Handlable item)
        {
            if (RegisteredItems.Contains<Handlable>(item))
                throw new Exception("Item with the same name and type already registered.");
            AssingHandle(item);
            RegisteredItems.Add(item);
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
    }
}
