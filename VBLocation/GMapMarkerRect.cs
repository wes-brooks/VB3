using System.Drawing;
using GMap.NET.WindowsForms;
using GMap.NET;

namespace GMap.NET.WindowsForms.Markers
{
    public class GMapMarkerRect : GMapMarker
    {
        public Pen Pen;
        

        public GMapMarkerRect(PointLatLng p) : base(p)
        {
            Pen = new Pen(Brushes.Red, 5);
        }


        public override void OnRender(Graphics g)
        {
            g.DrawRectangle(Pen, new System.Drawing.Rectangle(LocalPosition.X-Size.Width/2, LocalPosition.Y-Size.Height/2, Size.Width, Size.Height));
        }
    }
}
