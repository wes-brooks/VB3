using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing.Drawing2D;
using System.Text;
using System.IO;
using System.Reflection;
using GMap.NET;
using GMap.NET.CacheProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using VBCommon.Spatial;


namespace VBLocation
{
    public partial class frmLocation : UserControl
    {

        Site _site;

        PointLatLng start;
        PointLatLng end;

        // marker
        GMapMarker currentMarker;
        GMapMarker center;

        // layers
        GMapOverlay top;
        GMapOverlay objects;
        GMapOverlay routes;
        VBGMapOverlay stations;

        //custom stuff
        //VBProjectManager _projMgr = null;

        GMapMarker siteMarker;
        GMapMarker firstBeachMarker;
        GMapMarker secondBeachMarker;
        GMapMarker waterMarker;
        public PointLatLng sitelocation = new PointLatLng();
        private PointLatLng marker1 = new PointLatLng();
        private PointLatLng marker2 = new PointLatLng();
        private PointLatLng watermark = new PointLatLng();

        string add1stMarker = "Add 1st Beach Marker";
        string remove1stMarker = "Remove 1st Beach Marker";
        string add2ndMarker = "Add 2nd Beach Marker";
        string remove2ndMarker = "Remove 2nd Beach Marker";
        string addMarker = "Add Water Marker";
        string removeMarker = "Remove Water Marker";
        private int zoomlevel = 12;
        bool isMouseDown = false;

        public event EventHandler LocationFormEvent;
        public bool boolComplete = false;

        ToolTip _tooltipTrackBar = new ToolTip();
        //private LocationPlugin _plugin = null;

        
        public frmLocation()//LocationPlugin plugin)
        {
            InitializeComponent();
            String strPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            MainMap.CacheLocation = strPath + "\\cache";
            //_plugin = plugin;
            InitMap();
            showDefaultMap();
            _tooltipTrackBar.SetToolTip(trackBar1, trackBar1.Value.ToString());
        }


        public IDictionary<string, object> PackState()
        {
            //to hold packed state
            IDictionary<string, object> dictPluginState = new Dictionary<string, object>();

            if (_site != null)
            {
                if (_site.Orientation != null)
                {
                    dictPluginState.Add("Orientation", _site.Orientation);
                }

                if (_site.Location != null)
                {
                    Dictionary<string, double> dictLocation = new Dictionary<string, double>();
                    dictLocation.Add("Latitude", _site.Location.Latitude);
                    dictLocation.Add("Longitude", _site.Location.Longitude);
                    dictPluginState.Add("Location", dictLocation);
                }

                if (_site.LeftMarker != null)
                {
                    Dictionary<string, double> dictLeftMarker = new Dictionary<string, double>();
                    dictLeftMarker.Add("Latitude", _site.LeftMarker.Latitude);
                    dictLeftMarker.Add("Longitude", _site.LeftMarker.Longitude);
                    dictPluginState.Add("LeftMarker", dictLeftMarker);
                }

                if (_site.RightMarker != null)
                {
                    Dictionary<string, double> dictRightMarker = new Dictionary<string, double>();
                    dictRightMarker.Add("Latitude", _site.RightMarker.Latitude);
                    dictRightMarker.Add("Longitude", _site.RightMarker.Longitude);
                    dictPluginState.Add("RightMarker", dictRightMarker);
                }

                if (_site.WaterMarker != null)
                {
                    Dictionary<string, double> dictWaterMarker = new Dictionary<string, double>();
                    dictWaterMarker.Add("Latitude", _site.WaterMarker.Latitude);
                    dictWaterMarker.Add("Longitude", _site.WaterMarker.Longitude);
                    dictPluginState.Add("WaterMarker", dictWaterMarker);
                }
            }

            if (dictPluginState.ContainsKey("Location") && dictPluginState.ContainsKey("Orientation") && dictPluginState.ContainsKey("LeftMarker") && dictPluginState.ContainsKey("RightMarker") && dictPluginState.ContainsKey("WaterMarker"))
            {
                dictPluginState.Add("Complete", true);
            }
            else
            {
                dictPluginState.Add("Complete", false);
            }

            return (dictPluginState);
        }


        public void UnpackState(IDictionary<string, object> dictPackedState)
        {
            if (dictPackedState.Count == 0) return;
        }


        /// <summary>
        /// Fires when the app opens a project file
        /// </summary>
        /// <param name="projMgr"></param>
        private void ProjectOpenedListener()
        {
            //Something must be seriously messed up if this happens.
            //if (_projMgr == null)
            //    return;

            //if (_projMgr._projectType == VBTools.Globals.ProjectType.MODEL) return;

            ////Console.WriteLine("\n*** Location: project opened.***\n");

            //if (_site == null)
            //    showDefaultMap();
            //else
            //    showMap();

            //this.Show();
        }


        /// <summary>
        /// Fires when the app opens a project file
        /// </summary>
        /// <param name="projMgr"></param>
        private void ProjectSavedListener()
        {
            //Something must be seriously messed up if this happens.
            //if (_projMgr == null)
            //    return;

            //_projMgr.SiteInfo = saveMapProjectInfo();
        }


        public void InitMap()
        {
            //google key passed to map control for geocoding functionality.  See the document
            // "To use an API key add the following line to GMaps.doc" in the VBLocation project
            //directory for further information.
            GMaps.Instance.SetApiKey = "ABQIAAAA30Mmhg0NkZeUVdyOISPPexT2yXp_ZAY8_ufC3CFXhHIE1NvwkxRA65eRe7uIAzbaF4VXn8vlh6w--A";

            if (!DesignMode)
            {
                // config gmaps
                GMaps.Instance.UseRouteCache = true;
                GMaps.Instance.UseGeocoderCache = true;
                GMaps.Instance.UsePlacemarkCache = true;
                GMaps.Instance.Mode = AccessMode.ServerOnly;

                // config map 
                MainMap.MapType = MapType.YahooMap;

                // map events
                MainMap.OnCurrentPositionChanged += new CurrentPositionChanged(MainMap_OnCurrentPositionChanged);
                MainMap.OnTileLoadStart += new TileLoadStart(MainMap_OnTileLoadStart);
                MainMap.OnTileLoadComplete += new TileLoadComplete(MainMap_OnTileLoadComplete);
                MainMap.OnEmptyTileError += new EmptyTileError(MainMap_OnEmptyTileError);
                MainMap.OnMapZoomChanged += new MapZoomChanged(MainMap_OnMapZoomChanged);
                MainMap.OnMapTypeChanged += new MapTypeChanged(MainMap_OnMapTypeChanged);
                MainMap.MouseMove += new MouseEventHandler(MainMap_MouseMove);
                MainMap.MouseDown += new MouseEventHandler(MainMap_MouseDown);
                MainMap.MouseUp += new MouseEventHandler(MainMap_MouseUp);
                // custom: to paint the water
                MainMap.Paint += new PaintEventHandler(MainMap_Paint);

                // get map type
                comboBoxMapType.DataSource = Enum.GetValues(typeof(MapType));
                comboBoxMapType.SelectedItem = MainMap.MapType;

                // add custom layers  
                routes = new GMapOverlay(MainMap, "routes");
                MainMap.Overlays.Add(routes);

                objects = new GMapOverlay(MainMap, "objects");
                MainMap.Overlays.Add(objects);

                top = new GMapOverlay(MainMap, "top");
                MainMap.Overlays.Add(top);

                stations = new VBGMapOverlay(MainMap, "stations");
                
                MainMap.Overlays.Add(stations);
            }
        }


        private void showDefaultMap()
        {
            if (!DesignMode)
            {
                initMarkers();
                //MainMap.MapType = MapType.GoogleMap;
                MainMap.MaxZoom = trackBar1.Maximum;
                MainMap.MinZoom = trackBar1.Minimum;
                MainMap.Zoom = MainMap.MinZoom + 3;
                trackBar1.Value = MainMap.Zoom;

                MainMap.MapType = MapType.YahooMap;
                // get map type
                //comboBoxMapType.DataSource = Enum.GetValues(typeof(MapType));
                comboBoxMapType.DataSource = GetMapTypes();
                comboBoxMapType.SelectedItem = MainMap.MapType;

                // epa athens site
                MainMap.CurrentPosition = new PointLatLng(39.5718, -98.9648);
                //GMapMarker siteMarker = new GMapMarkerGoogleRed(MainMap.CurrentPosition);
                //top.Markers.Add(siteMarker);

                // show some site info
                //siteMarker.TooltipMode = MarkerTooltipMode.Always;
                //siteMarker.ToolTipText = "Welcome to EPA Athens Georgia";
                //objects.Markers.Add(siteMarker);
                //GMapMarkerRect mBorders = new GMapMarkerRect(MainMap.CurrentPosition);
                //mBorders.Size = new System.Drawing.Size(100, 100);
                //mBorders.TooltipMode = MarkerTooltipMode.Always;
                //objects.Markers.Add(mBorders);

                // get map type
                //comboBoxMapType.DataSource = Enum.GetValues(typeof(MapType));
                //comboBoxMapType.SelectedItem = MainMap.MapType;

                // acccess mode
                //comboBoxMode.DataSource = Enum.GetValues(typeof(AccessMode));
                //comboBoxMode.Items.Add(AccessMode.ServerOnly);
                //comboBoxMode.SelectedItem = GMaps.Instance.Mode;

                // get position
                textBoxLat.Text = MainMap.CurrentPosition.Lat.ToString(CultureInfo.InvariantCulture);
                textBoxLng.Text = MainMap.CurrentPosition.Lng.ToString(CultureInfo.InvariantCulture);

                // set current marker
                currentMarker = new GMapMarkerGoogleRed(MainMap.CurrentPosition);
                top.Markers.Add(currentMarker);

                // map center
                center = new GMapMarkerCross(MainMap.CurrentPosition);
            }
        }


        public void showMap()
        {
            if (!DesignMode)
            {
                if (_site != null)
                {
                    initMarkers();
                    zoomlevel = 16;
                    PointLatLng point = new PointLatLng();
                    if (_site.Location != null)
                    {
                        point.Lat = _site.Location.Latitude;
                        point.Lng = _site.Location.Longitude;
                        MainMap.CurrentPosition = point;                        
                        top.Markers.Remove(currentMarker);
                        currentMarker = new GMapMarkerGoogleRed(point);
                        top.Markers.Add(currentMarker);
                        center = new GMapMarkerCross(point);
                        top.Markers.Add(center);

                        siteMarker = new GMapMarkerGoogleGreen(point);
                        top.Markers.Add(siteMarker);
                        siteMarker.TooltipMode = MarkerTooltipMode.Always;
                        siteMarker.ToolTipText = _site.BeachName.ToString();
                        objects.Markers.Add(siteMarker);
                        sitelocation = point;
                    }

                    if (_site.LeftMarker != null)
                    {
                        btnBeachMarker.Text = remove1stMarker;
                        point.Lat = _site.LeftMarker.Latitude;
                        point.Lng = _site.LeftMarker.Longitude;
                        firstBeachMarker = new GMapMarkerGoogleGreen(point);                        
                        top.Markers.Add(firstBeachMarker);
                    }

                    if (_site.RightMarker != null)
                    {
                        btnBeachMarker2.Text = remove2ndMarker;
                        point.Lat = _site.RightMarker.Latitude;
                        point.Lng = _site.RightMarker.Longitude;
                        secondBeachMarker = new GMapMarkerGoogleGreen(point);                        
                        top.Markers.Add(secondBeachMarker);
                    }

                    if (_site.WaterMarker != null)
                    {
                        point.Lat = _site.WaterMarker.Latitude;
                        point.Lng = _site.WaterMarker.Longitude;
                        MainMap.CurrentPosition = new PointLatLng(point.Lat, point.Lng);
                        btnSelectWater.Text = addMarker;
                        EventArgs e = new EventArgs();
                        btnSelectWater_Click(this, e);
                    }
                    // display project info

                    if (_site.Orientation == double.NaN)               
                        txtBeachAngle.Text = string.Empty;
                    else 
                        txtBeachAngle.Text = _site.Orientation.ToString("###0.##");


                    if (_site.Location != null)
                    {                        
                        textBoxCurrLat.Text = _site.Location.Latitude.ToString();                        
                        textBoxCurrLng.Text = _site.Location.Longitude.ToString();
                    }
                    else
                    {                       
                        textBoxCurrLat.Text = string.Empty;                        
                        textBoxCurrLng.Text = string.Empty;
                    }
                }

                MainMap.ZoomAndCenterMarkers(null);
            }
        }


        void MainMap_OnMapTypeChanged(MapType type)
        {
            switch (type)
            {
                case MapType.ArcGIS_Map:
                case MapType.ArcGIS_Satellite:
                case MapType.ArcGIS_ShadedRelief:
                case MapType.ArcGIS_Terrain:
                    {
                        MainMap.MaxZoom = 13;
                    }
                    break;

                case MapType.ArcGIS_MapsLT_Map_Hybrid:
                case MapType.ArcGIS_MapsLT_Map_Labels:
                case MapType.ArcGIS_MapsLT_Map:
                case MapType.ArcGIS_MapsLT_OrtoFoto:
                    {
                        MainMap.MaxZoom = 11;
                    }
                    break;

                default:
                    {
                        MainMap.MaxZoom = 17;
                    }
                    break;
            }

            if (MainMap.Zoom > MainMap.MaxZoom)
            {
                MainMap.Zoom = MainMap.MaxZoom;
            }
            trackBar1.Maximum = MainMap.MaxZoom;
        }


        void MainMap_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }


        void UpdateCurrentMarkerPositionText()
        {
            textBoxCurrLat.Text = currentMarker.Position.Lat.ToString(CultureInfo.InvariantCulture);
            textBoxCurrLng.Text = currentMarker.Position.Lng.ToString(CultureInfo.InvariantCulture);
        }


        void MainMap_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                currentMarker.Position = MainMap.FromLocalToLatLng(e.X, e.Y);
                UpdateCurrentMarkerPositionText();
                //update current position???
            }
        }


        // move current marker with left holding
        void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && isMouseDown)
            {
                currentMarker.Position = MainMap.FromLocalToLatLng(e.X, e.Y);
                UpdateCurrentMarkerPositionText();
            }
        }


        // MapZoomChanged
        void MainMap_OnMapZoomChanged()
        {
            trackBar1.Value = MainMap.Zoom;
        }


        // empty tile displayed
        void MainMap_OnEmptyTileError(int zoom, GMap.NET.Point pos)
        {
            //MessageBox.Show("OnEmptyTileError, Zoom: " + zoom + ", " + pos.ToString(), "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        // click on some marker
        void MainMap_OnMarkerClick(GMapMarker item)
        {
            MainMap.CurrentPosition = item.Position;
            MainMap.Zoom = 5;
        }
        

        //loader start loading tiles
        void MainMap_OnTileLoadStart(int loaderId)
        {
            switch (loaderId)
            {
                case 1:
                    progressBar1.Show();
                    break;

                case 2:
                    progressBar2.Show();
                    break;

                case 3:
                    progressBar3.Show();
                    break;
            }

            groupBoxLoading.Invalidate(true);
        }


        // loader end loading tiles
        void MainMap_OnTileLoadComplete(int loaderId)
        {
            switch (loaderId)
            {
                case 1:
                    progressBar1.Hide();
                    break;

                case 2:
                    progressBar2.Hide();
                    break;

                case 3:
                    progressBar3.Hide();
                    break;
            }
            groupBoxLoading.Invalidate(true);
        }


        // current point changed
        void MainMap_OnCurrentPositionChanged(PointLatLng point)
        {
            center.Position = point;
        }


        // change map type
        private void comboBoxMapType_DropDownClosed(object sender, EventArgs e)
        {
            MainMap.MapType = (MapType)comboBoxMapType.SelectedValue;
        }


        // change mode
        private void comboBoxMode_DropDownClosed(object sender, EventArgs e)
        {
            //GMaps.Instance.Mode = (AccessMode)comboBoxMode.SelectedValue;
            //MainMap.ReloadMap();
        }


        // zoom
        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {            
            _tooltipTrackBar.SetToolTip(trackBar1, trackBar1.Value.ToString());
            MainMap.Zoom = trackBar1.Value;

            //if (trackBar1.Value >= 15)
            //    btnShowDataSources.Enabled = true;
            //else
            //    btnShowDataSources.Enabled = false;
        }


        // go to
        private void button8_Click(object sender, EventArgs e)
        {
            double lat = double.Parse(textBoxLat.Text, CultureInfo.InvariantCulture);
            double lng = double.Parse(textBoxLng.Text, CultureInfo.InvariantCulture);

            MainMap.CurrentPosition = new PointLatLng(lat, lng);
        }


        // goto by geocoder
        private void textBoxGeo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter)
            {
                GeoCoderStatusCode status = MainMap.SetCurrentPositionByKeywords(textBoxGeo.Text);
                if (status != GeoCoderStatusCode.G_GEO_SUCCESS)
                {
                    MessageBox.Show("Google Maps Geocoder can't find: '" + textBoxGeo.Text + "', reason: " + status.ToString(), "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }


        // reload map
        private void button1_Click(object sender, EventArgs e)
        {
            MainMap.ReloadMap();
        }


        // cache config
        private void checkBoxUseCache_CheckedChanged(object sender, EventArgs e)
        {
            //GMaps.Instance.UseRouteCache = checkBoxUseRouteCache.Checked;
            //GMaps.Instance.UseGeocoderCache = checkBoxUseGeoCache.Checked;
            GMaps.Instance.UsePlacemarkCache = GMaps.Instance.UseGeocoderCache;
        }


        // clear cache
        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are You sure?", "Clear GMap.NET cache?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
            {
                try
                {
                    System.IO.Directory.Delete(MainMap.CacheLocation, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        // add test route
        private void button3_Click(object sender, EventArgs e)
        {
            MapRoute route = GMaps.Instance.GetRouteBetweenPoints(start, end, false, MainMap.Zoom);
            if (route != null)
            {
                // add route
                GMapRoute r = new GMapRoute(route.Points, route.Name);
                r.Color = Color.Blue;
                routes.Routes.Add(r);

                // add route start/end marks
                GMapMarker m1 = new GMapMarkerGoogleRed(start);
                m1.ToolTipText = "Start: " + route.Name;
                m1.TooltipMode = MarkerTooltipMode.Always;

                GMapMarker m2 = new GMapMarkerGoogleGreen(end);
                m2.ToolTipText = "End: " + end.ToString();
                m2.TooltipMode = MarkerTooltipMode.Always;

                objects.Markers.Add(m1);
                objects.Markers.Add(m2);

                MainMap.ZoomAndCenterRoute(r);

                // testing kml support
                KmlType info = GMaps.Instance.GetRouteBetweenPointsKml(start, end, false);
                if (info != null)
                {

                }
            }
        }


        // add marker on current position
        private void button4_Click(object sender, EventArgs e)
        {
            GMapMarker m = new GMapMarkerGoogleGreen(currentMarker.Position);
            GMapMarkerRect mBorders = new GMapMarkerRect(currentMarker.Position);
            mBorders.Size = new System.Drawing.Size(100, 100);


            //Placemark p = null;
            //if (checkBoxPlacemarkInfo.Checked)
            //{
            //    p = GMaps.Instance.GetPlacemarkFromGeocoder(currentMarker.Position);
            //}

            //if (p != null)
            //{
            //    mBorders.ToolTipText = p.Address;
            //}
            //else
            //{
            //    mBorders.ToolTipText = currentMarker.Position.ToString();
            //}

            objects.Markers.Add(m);
            objects.Markers.Add(mBorders);
        }


        // clear routes
        private void button6_Click(object sender, EventArgs e)
        {
            routes.Routes.Clear();
        }


        // clear markers
        private void button5_Click(object sender, EventArgs e)
        {
            objects.Markers.Clear();
        }


        // show current marker
        private void checkBoxCurrentMarker_CheckedChanged(object sender, EventArgs e)
        {
            //if (checkBoxCurrentMarker.Checked)
            //{
            //    top.Markers.Add(currentMarker);
            //}
            //else
            //{
            //    top.Markers.Remove(currentMarker);
            //}
        }


        // can drag
        private void checkBoxCanDrag_CheckedChanged(object sender, EventArgs e)
        {
            //MainMap.CanDragMap = checkBoxCanDrag.Checked;
        }

        // set route start
        private void buttonSetStart_Click(object sender, EventArgs e)
        {
            start = currentMarker.Position;
        }


        // set route end
        private void buttonSetEnd_Click(object sender, EventArgs e)
        {
            end = currentMarker.Position;
        }


        // zoom to max for markers
        private void button7_Click(object sender, EventArgs e)
        {
            MainMap.ZoomAndCenterMarkers("objects");
        }


        // export map data
        private void button9_Click(object sender, EventArgs e)
        {
            MainMap.ShowExportDialog();
        }


        // import map data
        private void button10_Click(object sender, EventArgs e)
        {
            MainMap.ShowImportDialog();
        }


        // prefetch
        private void button11_Click(object sender, EventArgs e)
        {
            RectLatLng area = MainMap.SelectedArea;
            if (!area.IsEmpty)
            {
                for (int i = MainMap.Zoom; i <= MainMap.MaxZoom; i++)
                {
                    List<GMap.NET.Point> x = MainMap.Projection.GetAreaTileList(area, i, 0);

                    DialogResult res = MessageBox.Show("Ready ripp at Zoom = " + i + " ? Total => " + x.Count, "GMap.NET", MessageBoxButtons.YesNoCancel);

                    if (res == DialogResult.Yes)
                    {
                        TilePrefetcher obj = new TilePrefetcher();
                        obj.ShowCompleteMessage = true;
                        obj.Start(x, i, MainMap.MapType, 100);
                    }
                    else if (res == DialogResult.No)
                    {
                        continue;
                    }
                    else if (res == DialogResult.Cancel)
                    {
                        break;
                    }

                    x.Clear();
                }
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        // saves current map view 
        private void button12_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PNG (*.png)|*.png";
                    sfd.FileName = "GMap.NET image";

                    Image tmpImage = MainMap.ToImage();
                    if (tmpImage != null)
                    {
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            tmpImage.Save(sfd.FileName);

                            MessageBox.Show("Image saved: " + sfd.FileName, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Image failed to save: " + ex.Message, "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // debug
        private void checkBoxDebug_CheckedChanged(object sender, EventArgs e)
        {
            //MainMap.ShowTileGridLines = checkBoxDebug.Checked;
        }


        private void button13_Click(object sender, EventArgs e)
        {
            RectLatLng area = MainMap.SelectedArea;
            if (!area.IsEmpty)
            {
                StaticImage st = new StaticImage(MainMap);
                //st.Owner = this;
                st.Show();
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


        public Site saveMapProjectInfo()
        {
            Site site = new Site();
            //site.Project = txtboxProjName.Text.ToString();
            //site.Name = txtBoxBeachName.Text.ToString();
            if (txtBeachAngle.Text == string.Empty) 
                site.Orientation = double.NaN;
            else 
                site.Orientation = Convert.ToDouble(txtBeachAngle.Text.ToString());

            if (textBoxCurrLat.Text == string.Empty || textBoxCurrLng.Text == string.Empty)
            {
                site.Location.Latitude = double.NaN;
                site.Location.Longitude = double.NaN;
            }
            else
            {
                //site.Location.Latitude = sitelocation.Lat;
                //site.Location.Longitude = sitelocation.Lng;
                site.Location.Latitude = Convert.ToDouble(textBoxCurrLat.Text);
                site.Location.Longitude = Convert.ToDouble(textBoxCurrLng.Text);
            }

            if (firstBeachMarker == null)
            {
                site.LeftMarker.Latitude = double.NaN;
                site.LeftMarker.Longitude = double.NaN;
            }
            else
            {
                site.LeftMarker.Latitude = firstBeachMarker.Position.Lat;//marker1.Lat;
                site.LeftMarker.Longitude = firstBeachMarker.Position.Lng;//marker1.Lng;
            }

            if (secondBeachMarker == null)
            {
                site.RightMarker.Latitude = double.NaN;
                site.RightMarker.Longitude = double.NaN;
            }
            else
            {
                site.RightMarker.Latitude = secondBeachMarker.Position.Lat;//marker2.Lat;
                site.RightMarker.Longitude = secondBeachMarker.Position.Lng;//marker2.Lng;
            }
            if (waterMarker == null)
            {
                site.WaterMarker.Latitude = double.NaN;
                site.WaterMarker.Longitude = double.NaN;
            }
            else
            {
                site.WaterMarker.Latitude = waterMarker.Position.Lat;//watermark.Lat;
                site.WaterMarker.Longitude = waterMarker.Position.Lng;//watermark.Lng;
            }

            return site;
        }


        public void initMarkers()
        {
            objects.Markers.Clear();
            top.Markers.Clear();
            routes.Markers.Clear();
        }


        private void button3_Click_1(object sender, EventArgs e)
        {
            KeyPressEventArgs kp = new KeyPressEventArgs(Convert.ToChar(Keys.Enter));
            textBoxGeo_KeyPress(sender, kp);
        }


        private void btnSelectWater_Click(object sender, EventArgs e)
        {
            try
            {
                if ((firstBeachMarker == null) || (secondBeachMarker == null))
                    return;

                btnSelectWater.Enabled = true;
                string addMarker = "Add Water Marker";
                string removeMarker = "Remove Water Marker";

                //GMap.NET.Point pointH2O = new GMap.NET.Point();
                GMap.NET.Point pointH20 = new GMap.NET.Point();
                //GMapNET.Point pointPerpendicular = new GMapNET.Point();
                //GMap.NET.Point pointPerpendicular = new GMap.NET.Point();
                if (btnSelectWater.Text == addMarker)
                {
                    waterMarker = new GMapMarkerGoogleGreen(currentMarker.Position);
                    //waterMarker = new GMapMarkerGoogleGreen(MainMap.CurrentPosition);
                    //MainMap.Markers.Add(waterMarker);
                    top.Markers.Add(waterMarker);
                    btnSelectWater.Text = removeMarker;
                    pointH20 = MainMap.FromLatLngToLocal(waterMarker.Position);
                    //pointPerpendicular = CoordinatePerpendicular();
                }
                else
                {
                    btnSelectWater.Text = addMarker;
                    //MainMap.Markers.Remove(waterMarker);
                    top.Markers.Remove(waterMarker);
                    waterMarker = null;
                    txtBeachAngle.Text = string.Empty;
                    return;
                }

                //////////////////////////////////////////////
                GMap.NET.Point pointA = MainMap.FromLatLngToLocal(firstBeachMarker.Position);
                GMap.NET.Point pointB = MainMap.FromLatLngToLocal(secondBeachMarker.Position);
                GMap.NET.Point pointWM = MainMap.FromLatLngToLocal(waterMarker.Position);
                //GMapNET.Point pointA = MainMap.FromLatLngToLocal(firstBeachMarker.Position);
                //GMapNET.Point pointB = MainMap.FromLatLngToLocal(secondBeachMarker.Position);

                int side = CoordinatePerpendicular();

                ////rotate marker points thru -pi/2 since we want due N as zero
                //GMap.NET.Point Aprime = rotatePt(-Math.PI / 2, pointA);
                //GMap.NET.Point Bprime = rotatePt(-Math.PI / 2, pointB);
                //GMap.NET.Point WMprime = rotatePt(-Math.PI / 2, pointWM);

                //returns negative angles ccw from pos x axis thru -pi (quads I and II),
                //returns positive angles cw from pos x axis thru pi (quads IV and III)
                double angle = Math.Atan2(pointB.Y - pointA.Y, pointB.X - pointA.X);
                double deg = RadianToDegree((float)angle);
                //double angle = Math.Atan2(Bprime.Y - Aprime.Y, Bprime.X - Aprime.X);
                //double deg = RadianToDegree((float)angle);
                

                //slope
                //double deltaX = Bprime.X - Aprime.X;
                //double slope = double.NaN;
                //if (deltaX != 0) slope = (Bprime.Y - Aprime.Y) / deltaX;
                double deltaX = pointB.X - pointA.X;
                double slope = double.NaN;
                if (deltaX != 0) slope = (pointB.Y - pointA.Y) / deltaX;

                //negates order of pts for angle calc; computes -90 < deg < 90
                if (slope.Equals(double.NaN))
                {
                    deg = 90.0;
                    if (pointA.Y > pointB.Y) deg = -90;
                }
                else if (slope > 0)
                {
                    if (deg < 0) deg = deg + 180.0;
                }
                else if (slope < 0)
                {
                    if (deg > 0) deg = deg - 180.0;
                }
                else
                {
                    deg = 0.0;
                }

                //get angle relative to N (N==0deg) and relative to side of line user selects 
                if (side > 0)
                {
                    deg = deg + 90.0;
                    //translate quadrant i to quadrant iii in ccw direction (i.e., make 135 == -270)
                    //correct??? makes MC's angles as documented.
                    //if (deg > 90.0 && deg <= 135.0) deg = deg - 360.0;
                }
                else
                {
                    deg = deg - 90.0;                  
                }


                txtBeachAngle.Text = deg.ToString("####0.##");

                _site = new Site();
                _site.Orientation = deg;
                _site.Location.Latitude = Convert.ToDouble(textBoxCurrLat.Text);
                _site.Location.Longitude = Convert.ToDouble(textBoxCurrLng.Text);
                _site.LeftMarker.Latitude = firstBeachMarker.Position.Lat;//marker1.Lat;
                _site.LeftMarker.Longitude = firstBeachMarker.Position.Lng;//marker1.Long;
 
                _site.RightMarker.Latitude = secondBeachMarker.Position.Lat;//marker2.Lat;
                _site.RightMarker.Longitude = secondBeachMarker.Position.Lng;//marker2.Long;
 
                _site.WaterMarker.Latitude = waterMarker.Position.Lat;//watermark.Lat;
                _site.WaterMarker.Longitude = waterMarker.Position.Lng;//watermark.Long;

                /*if (_plugin != null)
                {
                    //VBCommon.Interfaces.IBeachSite site = _plugin as VBCommon.Interfaces.IBeachSite;
                    _plugin.Site = _site.Clone();
                }*/

                if (LocationFormEvent != null)
                {
                    EventArgs args  = new EventArgs();
                    LocationFormEvent(this, args);
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
            }
            return;
        }


        private GMap.NET.Point rotatePt(double rotationAngle, GMap.NET.Point pointU)
        {
            //rotation angle in radians
            GMap.NET.Point point = new GMap.NET.Point();
            point.X = (int)(pointU.X * Math.Cos(rotationAngle) - pointU.Y * Math.Sin(rotationAngle));
            point.Y = (int) (pointU.X * Math.Sin(rotationAngle) + pointU.Y * Math.Cos(rotationAngle));
            return point;
        }


        private float RadianToDegree(float angle)
        {
            float degree = (float)(angle * (180.0 / Math.PI));
            return degree;
        }

        private float DegreeToRadian(float angle)
        {
            float radian = (float)(Math.PI * angle / 180.0);
            return radian;
        }


        private double CalcAngle(GMap.NET.Point point, GMap.NET.Point point2)
        {
            return 0.0;
        }


        private void btnBeachMarker_Click(object sender, EventArgs e)
        {
            if (btnBeachMarker.Text == add1stMarker)
            {
                firstBeachMarker = new GMapMarkerGoogleGreen(currentMarker.Position);
                //firstBeachMarker = new GMapMarkerGoogleGreen(MainMap.CurrentPosition);
                //GMapMarkerRect mBorders = new GMapMarkerRect(MainMap.CurrentPosition);
                //mBorders.Size = new Size(100, 100);                       
                //MainMap.Markers.Add(firstBeachMarker);
                top.Markers.Add(firstBeachMarker);
                btnBeachMarker.Text = remove1stMarker;
            } else {
                btnBeachMarker.Text = add1stMarker;

                if (firstBeachMarker != null)
                    top.Markers.Remove(firstBeachMarker);
                //MainMap.Markers.Remove(firstBeachMarker);

                firstBeachMarker = null;
            }

            if ((firstBeachMarker != null) && (secondBeachMarker != null))
                btnSelectWater.Enabled = true;
            else
                btnSelectWater.Enabled = false;
        }

        private void btnBeachMarker2_Click(object sender, EventArgs e)
        {

            if (btnBeachMarker2.Text == add2ndMarker)
            {
                secondBeachMarker = new GMapMarkerGoogleGreen(currentMarker.Position);
                //secondBeachMarker = new GMapMarkerGoogleGreen(MainMap.CurrentPosition);
                //MainMap.Markers.Add(secondBeachMarker);
                top.Markers.Add(secondBeachMarker);
                btnBeachMarker2.Text = remove2ndMarker;
            } else {
                btnBeachMarker2.Text = add2ndMarker;

                if (secondBeachMarker != null)
                    top.Markers.Remove(secondBeachMarker);
                //MainMap.Markers.Remove(secondBeachMarker);
                txtBeachAngle.Text = "";
                secondBeachMarker = null;
            }

            if ((firstBeachMarker != null) && (secondBeachMarker != null))
                btnSelectWater.Enabled = true;
            else
                btnSelectWater.Enabled = false;
        }


        private void MainMap_Paint(object sender, PaintEventArgs e)
        {
            if ((firstBeachMarker != null) && (secondBeachMarker != null))
            {
                Graphics g = e.Graphics;
                GMap.NET.Point pointTmpA = MainMap.FromLatLngToLocal(firstBeachMarker.Position);
                GMap.NET.Point pointTmpB = MainMap.FromLatLngToLocal(secondBeachMarker.Position);

                GMap.NET.Point pointA = new GMap.NET.Point();
                GMap.NET.Point pointB = new GMap.NET.Point();
                GMap.NET.Point pointMid = new GMap.NET.Point();
                GMap.NET.Point pointNew = new GMap.NET.Point();

                //Order points from left to right
                if (pointTmpA.X <= pointTmpB.X)
                {
                    pointA = pointTmpA;
                    pointB = pointTmpB;
                }
                else
                {
                    pointA = pointTmpB;
                    pointB = pointTmpA;
                }

                Pen pen = new Pen(Color.LimeGreen, 5);
                g.DrawLine(pen, pointA.X, pointA.Y, pointB.X, pointB.Y);

                if (waterMarker != null)
                {
                    double angle = Math.Atan2(pointB.Y - pointA.Y, pointB.X - pointA.X);
                    //Console.Write("**MAngle = " + angle + "\n");
                    Matrix matrix = new Matrix();

                    float deg = RadianToDegree((float)angle);
                    double dist = Math.Sqrt((Math.Pow(pointA.Y - pointB.Y, 2) + Math.Pow(pointA.X - pointB.X, 2)));
                    //Console.Write("**MDist = " + dist + "\n");
                    PointF rotPoint = new PointF(pointA.X, pointA.Y);
                    int side = CoordinatePerpendicular();
                    //Console.Write("**MSide = " + side + "\n");
                    matrix.RotateAt(deg, rotPoint);

                    LinearGradientBrush lgb = null;
                    RectangleF rect = new RectangleF(pointA.X, pointA.Y, Convert.ToInt32(dist), Convert.ToInt32(dist / 2));

                    if (side < 0)
                    {
                        matrix.Translate(0, (side) * (float)dist / 2);
                        if (rect.Width > 0 && rect.Height > 0)
                            lgb = new LinearGradientBrush(rect, Color.DarkBlue, Color.LightBlue, LinearGradientMode.Vertical);
                    }
                    //added else clause to get gradient right (mog 8/8)
                    else { if (rect.Width > 0 && rect.Height > 0) lgb = new LinearGradientBrush(rect, Color.LightBlue, Color.DarkBlue, LinearGradientMode.Vertical); }

                    //else  ... commented out by mog - was only getting the water rect on one side of the line... 
                    // the gradient is still wrong....
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        //lgb = new LinearGradientBrush(rect, Color.LightBlue, Color.DarkBlue, LinearGradientMode.Vertical);
                        g.Transform = matrix;
                        //LinearGradientBrush lgb = new LinearGradientBrush(rect, Color.DarkBlue, Color.LightBlue, LinearGradientMode.Vertical);
                        //pen = new Pen(Color.Blue);
                        g.FillRectangle(lgb, rect);
                        //g.DrawRectangle(pen, pointA.X, pointA.Y, Convert.ToInt32(dist), (Convert.ToInt32(dist / 2)));
                        //pointMid.X = (pointA.X + pointB.X) / 2;
                        //pointMid.Y = (pointA.Y + pointB.Y) / 2;
                        g.ResetTransform();
                    }
                }
            }
        }


        //private GMapNET.Point CoordinatePerpendicular()
        private int CoordinatePerpendicular()
        {
            GMap.NET.Point pointA = new GMap.NET.Point();
            GMap.NET.Point pointB = new GMap.NET.Point();

            if ((firstBeachMarker == null) || (secondBeachMarker == null) || (waterMarker == null))
            {
                pointA.X = -1;
                pointA.Y = -1;
                //return pointA;
                return -999;
            }

            GMap.NET.Point pointTmpA = MainMap.FromLatLngToLocal(firstBeachMarker.Position);
            GMap.NET.Point pointTmpB = MainMap.FromLatLngToLocal(secondBeachMarker.Position);
            GMap.NET.Point pointH20 = MainMap.FromLatLngToLocal(waterMarker.Position);
            
            //Order points from left to right
            if (pointTmpA.X <= pointTmpB.X)
            {
                pointA = pointTmpA;
                pointB = pointTmpB;
            } else {
                pointA = pointTmpB;
                pointB = pointTmpA;
            }
            
            double slope = 0.0;
            double perpSlope = 0.0;

            int side = 1;

            if ((pointB.X - pointA.X) > 0)
            {
                slope = (double)(pointA.Y - pointB.Y) / (double)(pointA.X - pointB.X);
                perpSlope = -1 / slope;

                double b = (double)pointA.Y - (slope * (double)pointA.X);
                double bNew = (double)pointH20.Y - (slope * (double)pointH20.X);


                if (bNew > b)
                    side = 1;
                else
                    side = -1;
            }
            //added clauses for due N side selection (mog 5/10)
            else if (pointH20.X > pointA.X)
            {
                if (pointA.Y < pointB.Y) side = -1;
                else side = 1;
            }
            else if (pointH20.X < pointA.X)
            {
                if (pointA.Y < pointB.Y) side = 1;
                else side = -1;
            }

            return side;
        }


        public void btnShowDataSources_Click(object sender, EventArgs e)
        {
            Cursor current = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            if (trackBar1.Value < 13)
            {
                MessageBox.Show("Please zoom in to level >= 15 on the map in order to limit the amount of data to retrieve.");
                return;
            }


            stations.Markers.Clear();

            //if (_projMgr.SiteInfo.Project != "" || _projMgr.SiteInfo.Project != null)
            //{
                // _projMgr.SiteInfo.Location.Latitude = Convert.ToDouble(textBoxCurrLat.Text.ToString());
                // _projMgr.SiteInfo.Location.Longitude = Convert.ToDouble(textBoxCurrLng.Text.ToString());
            //}

            double minX = MainMap.CurrentViewArea.Left;
            double minY = MainMap.CurrentViewArea.Bottom;
            double maxX = MainMap.CurrentViewArea.Right;
            double maxY = MainMap.CurrentViewArea.Top;

            try
            {
                VBD4EM.D4EMData d4emData = new VBD4EM.D4EMData(minX, minY, maxX, maxY, "gcjjaDgainKJnfefJK");
                //List<VBD4EM.StationInfo> nwisList = null;
                //List<VBD4EM.StationInfo> ncdcList = null;
                //List<VBD4EM.DomainObjects.StationInfo> list = null;
                List<VBD4EM.StationInfo> list = null;

                VBMarker stationMarker = null;
                PointLatLng pt;

                if (cbnwis.Checked)
                {
                    list = d4emData.NWISStations();
                    for (int i = 0; i < list.Count; i++)
                    {
                        pt = new PointLatLng(list[i].Latitude, list[i].Longitude);
                        stationMarker = new VBMarker(pt);
                        stationMarker.MarkerImage = new Bitmap(Properties.Resources.USGS);
                        stationMarker.Size = new System.Drawing.Size(15, 15);
                        System.Drawing.Size size = stationMarker.Size;
                        stationMarker.TooltipMode = MarkerTooltipMode.OnMouseOver;
                        stationMarker.ToolTipText = "Station ID: " + list[i].ID + Environment.NewLine + "Station Name: " + list[i].Name;
                        stations.Markers.Add(stationMarker);
                    }
                }
                if (cbncdc.Checked)
                {
                    list = d4emData.NCDCStations();
                    for (int i = 0; i < list.Count; i++)
                    {
                        pt = new PointLatLng(list[i].Latitude, list[i].Longitude);
                        stationMarker = new VBMarker(pt);
                        stationMarker.MarkerImage = new Bitmap(Properties.Resources.NOAA);
                        stationMarker.Size = new System.Drawing.Size(15, 15);
                        System.Drawing.Size size = stationMarker.Size;
                        stationMarker.TooltipMode = MarkerTooltipMode.OnMouseOver;
                        stationMarker.ToolTipText = "Station ID: " + list[i].ID + Environment.NewLine + "Station Name: " + list[i].Name;
                        stationMarker.ToolTipOffset = new System.Drawing.Point(7, 7);
                        stations.Markers.Add(stationMarker);
                    }
                }

                //if ((nwisList != null) && (nwisList.Count > 0))
                //    list.AddRange(nwisList);

                //if ((ncdcList != null) && (ncdcList.Count > 0))
                //    list.AddRange(ncdcList);

                //GMapMarkerGoogleGreen stationMarker = null;
                


                //for (int i = 0; i < list.Count; i++)
                //{

                //    pt = new PointLatLng(list[i].Latitude, list[i].Longitude);
                //    //stationMarker = new GMapMarkerGoogleGreen(pt);
                //    stationMarker = new VBMarker(pt);
                //    stationMarker.MarkerImage = new Bitmap(Properties.Resources.USGS);
                //    stationMarker.Size = new System.Drawing.Size(15, 15);
                //    System.Drawing.Size size = stationMarker.Size;
                //    stationMarker.TooltipMode = MarkerTooltipMode.OnMouseOver;
                //    stationMarker.ToolTipText = "Station ID: " + list[i].ID + Environment.NewLine + "Station Name: " + list[i].Name;
                //    stations.Markers.Add(stationMarker);
                //}                
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Cursor.Current = current;
            }
        }

        
        public void btnClearStations_Click(object sender, EventArgs e)
        {
            stations.Markers.Clear();
        }


        private void frmLocation_HelpRequested(object sender, HelpEventArgs hlpevent)
        {                
            string apppath = Application.StartupPath.ToString();
            VBCommon.VBCSHelp help = new VBCommon.VBCSHelp(apppath, sender);
            if (!help.Status)
            {
                MessageBox.Show(
                "User documentation is found in the Documentation folder where you installed Virtual Beach"
                + "\nIf your web browser is PDF-enabled, open the User Guide with your browser.",
                "Neither Adobe Acrobat nor Adobe Reader found.",
                MessageBoxButtons.OK);
            }
        }


        private MapType[] GetMapTypes()
        {
            List<MapType> excludedMapTypes = new List<MapType>();

            excludedMapTypes.Add(MapType.ArcGIS_Map);
            excludedMapTypes.Add(MapType.ArcGIS_MapsLT_Map);
            excludedMapTypes.Add(MapType.ArcGIS_MapsLT_Map_Hybrid);
            excludedMapTypes.Add(MapType.ArcGIS_MapsLT_Map_Labels);
            excludedMapTypes.Add(MapType.ArcGIS_MapsLT_OrtoFoto);
            excludedMapTypes.Add(MapType.ArcGIS_Satellite);
            excludedMapTypes.Add(MapType.ArcGIS_ShadedRelief);
            excludedMapTypes.Add(MapType.ArcGIS_Terrain);
            excludedMapTypes.Add(MapType.GoogleHybridChina);
            excludedMapTypes.Add(MapType.GoogleLabelsChina);
            excludedMapTypes.Add(MapType.GoogleMapChina);
            excludedMapTypes.Add(MapType.GoogleSatelliteChina);
            excludedMapTypes.Add(MapType.GoogleTerrainChina);           
            
            Array mapTypes = Enum.GetValues(typeof(MapType));
            List<MapType> includedMapTypes = new List<MapType>();

            for (int i = 0; i < mapTypes.Length; i++)
            {
                if (!excludedMapTypes.Contains((MapType)mapTypes.GetValue(i)))
                {
                    includedMapTypes.Add((MapType)mapTypes.GetValue(i));
                }
            }
            return includedMapTypes.ToArray();
        }


        private void frmLocation_Load(object sender, EventArgs e)
        {
            textBoxCurrLat.Text  = MainMap.CurrentPosition.Lat.ToString();
            textBoxCurrLng.Text  = MainMap.CurrentPosition.Lng.ToString();
        }
    }
}
