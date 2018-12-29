using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;

namespace EasySwarm2._0
{
    public partial class UserMapControl : GMapControl
    {
        public GMapOverlay makerOverlay { get; }

        private int makerNum = 0;
        public int selectedId { get; set; }

        private bool bIsDown = false;

        public UserMapControl()
        {
            InitializeComponent();

            makerOverlay = new GMapOverlay("makerOverlay");
            Overlays.Add(makerOverlay); 

            MouseClick += MapControl_MouseClick;
            MouseMove += MapControl_MouseMove;
            MouseDown += MapControl_MouseDown;
            MouseUp += MapControl_MouseUp;
        }

        public void NextPostion()
        {
            ((CustomMarker)makerOverlay.Markers.ElementAt(selectedId - 1)).brushGround = new SolidBrush(Color.DarkGoldenrod);
            ++selectedId;
            if (selectedId <= makerNum)
                ((CustomMarker)makerOverlay.Markers.ElementAt(selectedId - 1)).brushGround = new SolidBrush(Color.DarkGreen);
            else
                selectedId = -1;

            Refresh();
        }

        public void DeleteMarker()
        {
            if (selectedId > 0)
            {
                makerOverlay.Markers.RemoveAt(selectedId - 1);
                --makerNum;

                makerOverlay.Routes.Clear();

                List<PointLatLng> tmp = new List<PointLatLng>();
                for (int i = 0; i < makerNum; i++)
                {
                    if (i >= selectedId - 1)
                        ((CustomMarker)makerOverlay.Markers.ElementAt(i)).ID = i + 1;

                    tmp.Add(makerOverlay.Markers.ElementAt(i).Position);
                }

                GMapRoute routeTmp = new GMapRoute(tmp, "airLine");
                routeTmp.Stroke = new Pen(Color.DarkGoldenrod, 2);
                makerOverlay.Routes.Add(routeTmp);

                selectedId = selectedId < makerNum ? selectedId : makerNum;
                if (selectedId > 0)
                    ((CustomMarker)makerOverlay.Markers.ElementAt(selectedId - 1)).brushGround = new SolidBrush(Color.DarkGreen);
            }
        }

        public void SetGotoAlt(float alt)
        {
            if (selectedId > 0 && selectedId <= makerOverlay.Markers.Count)
                ((CustomMarker)makerOverlay.Markers.ElementAt(selectedId - 1)).alt = alt;
        }

        public void ClearMarker()
        {
            selectedId = -1;
            makerNum = 0;
            makerOverlay.Routes.Clear();
            makerOverlay.Markers.Clear();
            Refresh();
        }

        private void MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (makerNum > 0)
            {
                int tmpId = -1;
                for (int i = 0; i < makerNum; i++)
                {
                    if (makerOverlay.Markers.ElementAt(i).IsMouseOver)
                    {
                        tmpId = i + 1;
                    }
                }

                if (tmpId == selectedId)
                    bIsDown = true;

            }

        }

        private void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (bIsDown)
            {
                ((CustomMarker)makerOverlay.Markers.ElementAt(selectedId - 1)).Position = FromLocalToLatLng(e.X, e.Y);
                makerOverlay.Routes.Clear();
                List<PointLatLng> temp = new List<PointLatLng>();

                for (int i = 0; i < makerNum; i++)
                {
                    temp.Add(makerOverlay.Markers.ElementAt(i).Position);
                }

                GMapRoute route = new GMapRoute(temp, "airLine");
                route.Stroke = new Pen(Color.Goldenrod, 2);

                makerOverlay.Routes.Add(route);
            }

        }

        private void MapControl_MouseUp(object sender, MouseEventArgs e)
        {
            bIsDown = false;
        }
        
        public delegate void delegateChangeMake(object sender, floatEvent e);
        public event delegateChangeMake changeMakeEvent;

        private void MapControl_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Right:
                    {
                        for (int i = 0; i < makerNum; i++)
                        {
                            if (makerOverlay.Markers.ElementAt(i).IsMouseOver)
                            {
                                return;
                            }
                        }

                        ++makerNum;
                        selectedId = makerNum;
                        PointLatLng latLng = FromLocalToLatLng(e.X, e.Y);

                        if (makerNum > 1)
                        {
                            for (int i = 0; i < makerNum - 1; i++)
                                ((CustomMarker)makerOverlay.Markers.ElementAt(i)).brushGround = new SolidBrush(Color.DarkGoldenrod);
                        }

                        CustomMarker marker = new CustomMarker(latLng, makerNum);
                        makerOverlay.Markers.Add(marker);
                        marker.alt = 20;
                        floatEvent fe = new floatEvent();
                        fe.value = 20;
                        changeMakeEvent(this, fe);

                        if (makerNum > 1)
                        {
                            List<PointLatLng> tmp = new List<PointLatLng>();
                            tmp.Add(makerOverlay.Markers.ElementAt(makerNum - 2).Position);
                            tmp.Add(latLng);

                            GMapRoute route = new GMapRoute(tmp, (makerNum - 1).ToString());
                            route.Stroke = new Pen(Color.Goldenrod, 2);
                            makerOverlay.Routes.Add(route);
                        }

                       
                        break;
                    }
                case MouseButtons.Left:
                    {
                        if (makerNum > 0)
                        {
                            int tmpId = selectedId;
                            for (int i = 0; i < makerNum; i++)
                            {
                                if (makerOverlay.Markers.ElementAt(i).IsMouseOver)
                                {
                                    ((CustomMarker)makerOverlay.Markers.ElementAt(i)).brushGround = new SolidBrush(Color.DarkGreen);
                                    floatEvent fe = new floatEvent();
                                    fe.value = ((CustomMarker)makerOverlay.Markers.ElementAt(i)).alt;
                                    changeMakeEvent(this, fe);
                                    selectedId = i + 1;
                                }
                                else
                                    ((CustomMarker)makerOverlay.Markers.ElementAt(i)).brushGround = new SolidBrush(Color.DarkGoldenrod);
                            }

                            if (tmpId == selectedId)
                                ((CustomMarker)makerOverlay.Markers.ElementAt(selectedId - 1)).brushGround = new SolidBrush(Color.DarkGreen);

                        }
                        break;
                    }
                default:
                    break;
            }
        }

      
    }
}
