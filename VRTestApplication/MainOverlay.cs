using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Valve.VR;
using SteamVR_HUDCenter;
using SteamVR_HUDCenter.Elements;
using OpenTK.Graphics.OpenGL;

namespace VRTestApplication
{
    class MainOverlay : Overlay
    {
        private Overlay OverlayToShow;

        public MainOverlay(string FriendlyName,
            string ThumbnailPath,
            float Width,
            Overlay OverlayToShow,
            VROverlayInputMethod InputMethod = VROverlayInputMethod.None)
            : base(FriendlyName, ThumbnailPath, Width, InputMethod)
        {
            this.OverlayToShow = OverlayToShow;
        }

        public MainOverlay(string FriendlyName,
            float Width,
            Overlay OverlayToShow,
            VROverlayInputMethod InputMethod = VROverlayInputMethod.None)
            : base(FriendlyName, Width, InputMethod)
        {
            this.OverlayToShow = OverlayToShow;
        }

        public override void Start()
        {
            Bitmap bmp = new Bitmap(@"Resources/hl3.jpg");
            int textureID = GL.GenTexture();

            System.Drawing.Imaging.BitmapData TextureData =
            bmp.LockBits(
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb
                );

            GL.BindTexture(TextureTarget.Texture2D, textureID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, TextureData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            bmp.UnlockBits(TextureData);

            Texture_t texture = new Texture_t();
            texture.eType = ETextureType.OpenGL;
            texture.eColorSpace = EColorSpace.Auto;
            texture.handle = (IntPtr)textureID;
            OpenVR.Overlay.SetOverlayTexture(this.Handle, ref texture);
        }

        public override void Refresh()
        {

        }

        //Event handlers
        public override void OnVREvent_MouseButtonDown(VREvent_Data_t Data)
        {
            this.OverlayToShow.ToggleVisibility();
        }
    }
}
