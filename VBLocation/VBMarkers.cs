using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GMap.NET;
using GMap.NET.WindowsForms;

namespace VBLocation
{
    public class VBMarker : GMapMarker
    {
        private Image img;

        public Image MarkerImage
        {
            get { return img; }
            set { img = value; }
        }

        public VBMarker(PointLatLng p) : base(p)
        {

        }

        public override void OnRender(Graphics g)
        {
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(base.LocalPosition.X, base.LocalPosition.Y, img.Width, img.Height);
            g.DrawImageUnscaledAndClipped(img, rect);
        }

    }
}
