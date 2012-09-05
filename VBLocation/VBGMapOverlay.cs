using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using GMap.NET.WindowsForms;

namespace VBLocation
{
    public class VBGMapOverlay : GMapOverlay
    {
        private System.Windows.Forms.GMapControl Control;

        /// <summary>
        /// font for markers tooltip
        /// </summary>
        public Font TooltipFont = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold, GraphicsUnit.Point);

        public VBGMapOverlay(System.Windows.Forms.GMapControl control, string id) : base(control, id)      
        {
            Control = control;
        }
        override protected void DrawToolTip(Graphics g, GMapMarker m, int x, int y)
        {
            GraphicsState s = g.Save();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            System.Drawing.Size st = g.MeasureString(m.ToolTipText, TooltipFont).ToSize();
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x, y, st.Width + Control.TooltipTextPadding.Width, st.Height + Control.TooltipTextPadding.Height);
            rect.Offset(m.ToolTipOffset.X, m.ToolTipOffset.Y);

            g.DrawLine(TooltipPen, x, y, rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            g.FillRectangle(TooltipBackground, rect);
            g.DrawRectangle(TooltipPen, rect);
            g.DrawString(m.ToolTipText, TooltipFont, Brushes.Navy, rect, TooltipFormat);

            g.Restore(s);
        }
    }
}
