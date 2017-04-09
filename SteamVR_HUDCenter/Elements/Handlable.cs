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
        public string Key { get; private set; }
        public HUDCenterController Controller;
        public ulong Handle;

        public Handlable(string FriendlyName)
        {
            this.Name = FriendlyName;
            this.Key = Guid.NewGuid().ToString();
        }

        public bool Equals(Handlable item)
        {
            return this.Key.Equals(item.Key);
        }

        public virtual void Init(HUDCenterController Controller)
        {
            this.Controller = Controller;
        }
    }
}
