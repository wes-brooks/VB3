
namespace VBLocation
{
   using System.Windows.Forms;
   using GMap.NET.WindowsForms;

   /// <summary>
   /// custom map of GMapControl
   /// </summary>
   public class Map : GMapControl
   {
       private System.ComponentModel.IContainer components;
      /// <summary>
      /// any custom drawing here
      /// </summary>
      /// <param name="drawingContext"></param>
      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         //using(System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Color.Blue, 2))
         //{
         //   e.Graphics.DrawLine(p, 0, 0, 200, 200);
         //}
      }

      private void InitializeComponent()
      {
          this.SuspendLayout();
          // 
          // Map
          // 
          this.Name = "Map";
          this.ResumeLayout(false);

      }
   }
}
