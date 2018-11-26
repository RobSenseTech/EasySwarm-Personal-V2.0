using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing;

namespace EasySwarm2._0
{
    class CustomMarker : GMapMarker
    {
        public int ID { get; set; }
        public Brush brushGround { get; set; }
        public Brush brushId { get; set; }
        public float alt { set; get; }

        public CustomMarker(GMap.NET.PointLatLng p, int id, Int64 colorGround = 0xFF006400, Int64 colorId = 0xFFFFFFFF) : base(p)
        {
            brushGround = new SolidBrush(Color.FromArgb((int)((colorGround >> 24) & 0xFF), (int)((colorGround >> 16) & 0xFF), (int)((colorGround >> 8) & 0xFF), (int)(colorGround & 0xFF)));
            brushId = new SolidBrush(Color.FromArgb((int)(colorId >> 24), (int)((colorId >> 16) & 0xFF), (int)((colorId >> 8) & 0xFF), (int)(colorId & 0xFF)));
            ID = id;
            Size = new System.Drawing.Size(20, 20); //标记绘制区域的大小
            Offset = new System.Drawing.Point(-10, -10); //标记矩形左上角与指定点的偏移
        }

        //计算匹配矩形大小的字体
        private Font FindFont(Graphics g, string longString, Size Rec, Font PreferedFont)
        {
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Rec.Height / RealSize.Height;
            float WidthScaleRatio = Rec.Width / (RealSize.Width); 
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;
            return new Font(PreferedFont.FontFamily, ScaleFontSize);
        }

        public override void OnRender(Graphics g)
        {
            Rectangle rect = new Rectangle(LocalPosition.X, LocalPosition.Y, Size.Width, Size.Height);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.FillEllipse(brushGround, rect);
  
            Font PreferedFont = new Font("Adobe Gothic Std", 12f, FontStyle.Bold);
            Font IdFont = FindFont(g, ID.ToString(), new Size(rect.Width, rect.Height), PreferedFont);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
                
            g.DrawString(ID.ToString(), IdFont, brushId, rect, sf);
        }
    }
}
