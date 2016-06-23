using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SteamVR_HUDCenter.Elements.WPF
{
    /// <summary>
    /// Logica di interazione per VRUserControl.xaml
    /// </summary>
    public partial class VRUserControl : UserControl, IVRUserControl
    {
        public delegate void RenderEvent(RenderTargetBitmap bitmap);
        public event RenderEvent OnRendered;
        public VRUserControl()
        {
            InitializeComponent();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)this.Width, (int)this.Height, 96, 96, PixelFormats.Rgb128Float);
            bitmap.Render(this);
            if (OnRendered != null)
                OnRendered(bitmap);
        }
    }
}
