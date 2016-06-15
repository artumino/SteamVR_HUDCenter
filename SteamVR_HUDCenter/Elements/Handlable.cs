using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamVR_HUDCenter.Elements
{
    public class Handlable
    {
        public string Name { get; private set; }
        public string Key { get { return String.Format("{0}_{1}", this.GetType().Name, this.Name); } }
        public ulong Handle;

        public Handlable(string FriendlyName)
        {
            this.Name = FriendlyName;
            OpenVR_Utils.RegisterNewItem(this);
        }

        public bool Equals(Handlable item)
        {
            return this.Key.Equals(item.Key);
        }
    }
}
