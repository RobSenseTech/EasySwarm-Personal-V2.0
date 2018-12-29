using System;
using System.Drawing;
using System.Windows.Forms;
using CCWin;
using GMap.NET;
using GMap.NET.MapProviders;
using CCWin.SkinControl;
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace EasySwarm2._0
{
    public partial class MainFram : CCSkinMain
    {
        #region 
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
        string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
        string def, StringBuilder retVal, int size, string filePath);
        #endregion

        private ExcelReader excelReader = new ExcelReader();
        private Manager manager = null;
        private delegate void InvokeGetPortNames(string name);
        private int skinComboBox_ComNum_Index = -1;
        private bool bIsOpen = false;
        private int onlineCount = 0;

        private delegate void InvokeGetWhiteLists(string node);
        private delegate void InvokeSetOnlineList(string node);

        private string languageStr;

        private float baseLng = 0;
        private float baseLat = 0;
        private float baseAlt = 0;

        private Dictionary<string, string> dictionaryLanguage = new Dictionary<string, string>();

        private bool bIsExecute = false;

        private List<string> importMacList = new List<string>();

        private bool bIsGetRssi = false;
        private string strSelectNode;
        private UInt32 timerCount = 0;
        private byte getRssiCount = 0x00;
        private UInt32 sendCount = 0;
        private UInt32 receiveCount = 0;
        private float perNodedBm = 0;
        private float perGatewaydBm = 0;
        int count = 0;
        int x;//= (20 - width % 20) % 5;
        int y;// = (20 - width % 20) / 5;
        int test = 0;
        int width;// = skinPanel4.Size.Width;
        int height;// = skinPanel4.Size.Height;
        long timeOut = DateTime.Now.Ticks;

        public MainFram()
        {
            InitializeComponent();

            string path = Application.StartupPath;
            path += "\\config.ini";
            StringBuilder str = new StringBuilder();
            GetPrivateProfileString("MAIN", "language", "", str, 500, path);
            languageStr = str.ToString();

            if (languageStr == "")
                languageStr = "en-US";

            switch (languageStr)
            {
                case "zh-CN":
                    CHSToolStripMenuItem.Checked = true;
                    EnToolStripMenuItem.Checked = false;
                    break;
                case "en-US":
                    CHSToolStripMenuItem.Checked = false;
                    EnToolStripMenuItem.Checked = true;
                    break;
                default:
                    break;
            }

            if (!LoadLanguage(languageStr))
                MessageBox.Show(dictionaryLanguage["TEXT_BROKEN"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Error);

            userMapControl_Map.MapProvider = GMapProviders.BingHybridMap;
            GMap.NET.GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            userMapControl_Map.MinZoom = 2;
            userMapControl_Map.MaxZoom = 18;
            userMapControl_Map.Zoom = 18;
            userMapControl_Map.ShowCenter = false;
            userMapControl_Map.Position = new PointLatLng(30.18875, 120.134343);
            userMapControl_Map.DragButton = MouseButtons.Left;

            FormClosing += new FormClosingEventHandler(FormCloseing_do);
            InitRssi();
            str = new StringBuilder();
            GetPrivateProfileString("MAIN", "bIsShowFO", "1", str, 500, path);

            if (str.ToString() == "1")
            {
                FirmwareOption aa = new FirmwareOption(languageStr);
                aa.ShowDialog();
            }

            GetPrivateProfileString("MAIN", "APMorPX4", "APM", str, 500, path);

            if (str.ToString() == "APM")
            {
                skinRadioButton_APM.Checked = true;
                manager = new Manager("APM");
                skinComboBox_px4.Items.Clear();
                skinComboBox_px4.Items.Add("ArduCopter V3.3.3 Quad");
                skinComboBox_px4.Items.Add("ArduCopter V3.5.7 Quad");
                GetPrivateProfileString("MAIN", "firmwareIndex", "0", str, 500, path);
                skinComboBox_px4.SelectedIndex = int.Parse(str.ToString());
                skinLabel_currentFirme.Text = dictionaryLanguage["TEXT_CURFIRMWARM"] + skinComboBox_px4.Text;
            }
            else
            {
                skinRadioButton_PX4.Checked = true;
                manager = new Manager("PX4");
                skinComboBox_px4.Items.Clear();
                skinComboBox_px4.Items.Add("PX4 V1.6.5");
                skinComboBox_px4.Items.Add("PX4 V1.7.2");
                GetPrivateProfileString("MAIN", "firmwareIndex", "0", str, 500, path);
                GetPrivateProfileString("MAIN", "firmwareIndex", "0", str, 500, path);
                skinComboBox_px4.SelectedIndex = int.Parse(str.ToString());
                skinLabel_currentFirme.Text = dictionaryLanguage["TEXT_CURFIRMWARM"] + skinComboBox_px4.Text;
            }
            manager.delecheckNodeEvent += new Manager.delegateCheckNodeEvent(InvokesetListBox_OnlineNode);
            manager.deleGetRssiEvent += new Manager.delegateGetRssiEvent(drawRssi);
            userMapControl_Map.changeMakeEvent += new UserMapControl.delegateChangeMake(ChangeGotoAlt);
        }

        public void InitRssi()
        {
            int width = skinPanel4.Size.Width;
            int height = skinPanel4.Size.Height;
            float ir = (float)height / 148;
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, width, height));
            int i;
            for (i = 0; i < width; i += 20)
            {
                g.DrawLine(new Pen(Color.FromArgb(50, Color.Gray), 1), new Point(i, 0), new Point(i, height));
            }
            for (i = 0; i < height; i += 20)
            {
                g.DrawLine(new Pen(Color.FromArgb(50, Color.Gray), 1), new Point(0, i), new Point(width, i));
            }

            skinPanel4.BackgroundImage = b;
            skinPanel4.Invalidate();
            g.Dispose();
        }

        private void FormCloseing_do(Object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private bool LoadLanguage(string file)
        {
            dictionaryLanguage.Clear();

            string path = @"language//" + file + @"//mainFram.json";

            if (!File.Exists(path))
                return false;

            var content = File.ReadAllText(path, Encoding.UTF8);
            if (!string.IsNullOrEmpty(content))
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                foreach (string key in dict.Keys)
                {
                    if (!dictionaryLanguage.ContainsKey(key))
                    {
                        dictionaryLanguage.Add(key, dict[key]);
                    }
                    else
                    {
                        dictionaryLanguage[key] = dict[key];
                    }
                }
            }

            skinLabel_title.Text = dictionaryLanguage["TEXT_TITLE"];
            skinTabPage_main1.Text = dictionaryLanguage["TEXT_NET"];
            skinGroupBox_OnlineNode.Text = dictionaryLanguage["TEXT_OLNODE"];
            if (!bIsOpen)
                skinButton_OpenCom.Text = dictionaryLanguage["TEXT_OPEN"];
            else
                skinButton_OpenCom.Text = dictionaryLanguage["TEXT_CLOSE"];

            skinLabel_ComNum.Text = dictionaryLanguage["TEXT_COMNUM"];
            skinLabel_OpenComTip.Text = dictionaryLanguage["TEXT_CONNECTTIP"];
            lab_curTestNodeTip.Text = dictionaryLanguage["TEXT_CURTESTNODETIP"];

            if (bIsGetRssi)
                lab_testTip.Text = dictionaryLanguage["TEXT_TESTTIP_1"];
            else
                lab_testTip.Text = dictionaryLanguage["TEXT_TESTTIP_2"];

            skinLabel_gatewayQTitle.Text = dictionaryLanguage["TEXT_GATEWAYRQ"];
            skinLabel_gwQueriesTitle.Text = dictionaryLanguage["TEXT_INQUIRE"];
            skinLabel_timerTitle.Text = dictionaryLanguage["TEXT_TIME"];
            skinLabel_lossRateTip.Text = dictionaryLanguage["TEXT_LOSSRATE"];
            skinLabel_nodeQueriesTitle.Text = dictionaryLanguage["TEXT_INQUIRE"];
            skinLabel_nodeQTitle.Text = dictionaryLanguage["TEXT_NODEQ"];
            skinLabel_CurrentCom.Text = dictionaryLanguage["TEXT_CURCOM"];
            skinLabel_OnlineNodes.Text = dictionaryLanguage["TEXT_OLCOUNT"];
            skinLabel_ProgressTip.Text = "";
            if (!bIsGetRssi)
                skinButton_Measure.Text = dictionaryLanguage["TEXT_STARTTEST"];
            else
                skinButton_Measure.Text = dictionaryLanguage["TEXT_STOPTEST"];

            skinTabPage_main2.Text = dictionaryLanguage["TEXT_SWARM"];
            skinGroupBox_OnlineDrones.Text = dictionaryLanguage["TEXT_SINGLEDRONE"];
            skinButton_Arm.Text = dictionaryLanguage["TEXT_ARM"];
            skinButton_Disarm.Text = dictionaryLanguage["TEXT_DISARM"];
            skinButton_Takeoff.Text = dictionaryLanguage["TEXT_TAKEOFF"];
            skinButton_Pause.Text = dictionaryLanguage["TEXT_PAUSE"];
            skinButton_Return.Text = dictionaryLanguage["TEXT_RTL"];
            skinButton_Land.Text = dictionaryLanguage["TEXT_LAND"];
            skinLabel5.Text = dictionaryLanguage["TEXT_ALT"];
            skinTextBox_takeoffAlt.WaterText = dictionaryLanguage["TEXT_TAKEOFFALT"];
            skinGroupBox_AutoFormation.Text = dictionaryLanguage["TEXT_SWARMPLAN"];
            skinLabel_lngFlag.Text = dictionaryLanguage["TEXT_BASELNG"];
            skinLabel_latFlag.Text = dictionaryLanguage["TEXT_BASELAT"];
            skinLabel_altFlag.Text = dictionaryLanguage["TEXT_BASEALT"];
            skinLabel1.Text = dictionaryLanguage["TEXT_DEGREE"];
            skinLabel3.Text = dictionaryLanguage["TEXT_DEGREE"];
            skinLabel4.Text = dictionaryLanguage["TEXT_METER"];
            lab_ExecutePlan.Text = dictionaryLanguage["TEXT_PLANPATH"];
            skinButton_ImportAuto.Text = dictionaryLanguage["TEXT_IMPORTPLAN"];
            skinButton_Execute.Text = dictionaryLanguage["TEXT_STARTPLAN"];
            skinButton_ALLPAUSE.Text = dictionaryLanguage["TEXT_PAUSE"];
            skinButton_goto.Text = dictionaryLanguage["TEXT_GOTO"];
            btn_DeleteMaker.Text = dictionaryLanguage["TEXT_DEL"];
            btn_ClearMaker.Text = dictionaryLanguage["TEXT_CLR"];
            btn_setGotoAlt.Text = dictionaryLanguage["TEXT_SET"];
            tb_gotoAlt.WaterText = dictionaryLanguage["TEXT_GOTOALTWATER"];
           
            skinTabPage_main3.Text = dictionaryLanguage["TEXT_SYSSET"];
            skinTabPage_sys1.Text = dictionaryLanguage["TEXT_FIRMWARM"];
            //skinLabel_currentFirme.Text = "";
            skinLabel_currentFirme.Text = dictionaryLanguage["TEXT_CURFIRMWARM"] + skinComboBox_px4.Text;
            skinButton_setPX4.Text = dictionaryLanguage["TEXT_SET"];
            skinGroupBox_px4.Text = dictionaryLanguage["TEXT_FIRMWARM"];
            skinTabPage_sys2.Text = dictionaryLanguage["TEXT_NODESET"];
            skinGroupBox_modifyMac.Text = dictionaryLanguage["TEXT_NODEMIDIFY"];
            skinLabel_selectNode.Text = dictionaryLanguage["TEXT_SELECTNODE"];
            skinLabel2.Text = dictionaryLanguage["TEXT_NEWNODEMAC"];
            skinButton_modifyMAC.Text = dictionaryLanguage["TEXT_SET"];

            skinTabPage1.Text = dictionaryLanguage["TEXT_GATEWAYSET"];
            skinGroupBox_Gateway.Text = dictionaryLanguage["TEXT_WHITELIST"];
            skinLabel_MAC.Text = dictionaryLanguage["TEXT_NODEMAC"];
            skinButton_AddMAC.Text = dictionaryLanguage["TEXT_ADD"];
            skinButton_DeleteMAC.Text = dictionaryLanguage["TEXT_DELETE"];
            skinButton_ClearMAC.Text = dictionaryLanguage["TEXT_CLEAR"];
            skinButton_importMAC.Text = dictionaryLanguage["TEXT_IMPORT"];

            LanguageToolStripMenuItem.Text = dictionaryLanguage["TEXT_LANGUAGE"];
            EnToolStripMenuItem.Text = dictionaryLanguage["TEXT_ENGLISH"];
            CHSToolStripMenuItem.Text = dictionaryLanguage["TEXT_CHINESE"];
            OfficialToolStripMenuItem.Text = dictionaryLanguage["TEXT_WWW"];
            AboutToolStripMenuItem.Text = dictionaryLanguage["TEXT_ABOUT"];

            return true;
        }

        private void skinButton_ImportAuto_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = dictionaryLanguage["TEXT_PLANFILE"];
            fdlg.Filter = "Excel（*.csv）|*.csv";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;

            if (fdlg.ShowDialog() != DialogResult.OK)
                return;

            excelReader.Open(fdlg.FileName);
            lab_PlanPath.Text = fdlg.SafeFileName;
            //this.toolTip_plan.SetToolTip(this.lab_PlanPath, fdlg.SafeFileName);

            skinButton_Execute.Enabled = true;
            bIsExecute = false;
        }

        private void Arm(int rowNum)
        {
            int columCount = 5;
            while (true)
            {
                if (!bIsExecute)
                    break;
                string str = excelReader.GetCellString(rowNum, columCount).PadLeft(4, '0');

                if (str == "ENDL")
                    break;

                UInt32 Get_Mac;
                byte[] mac = new byte[2];
                Get_Mac = Convert.ToUInt32(str, 16);
                mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);
                mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                manager.arm(mac);

                columCount += 4;
                Thread.Sleep(50);
            }
        }

        private void Takeoff(int rowNum)
        {
            int columCount = 5;
            while (true)
            {
                if (!bIsExecute)
                    break;
                string str = excelReader.GetCellString(rowNum, columCount).PadLeft(4, '0');
                if (str == "ENDL")
                    break;

                UInt32 Get_Mac;
                byte[] mac = new byte[2];
                Get_Mac = Convert.ToUInt32(str, 16);
                mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);
                mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                manager.takeoff(mac, excelReader.GetCellInt(rowNum, columCount + 3));

                columCount += 4;
                Thread.Sleep(50);
            }
        }

        private void Disarm(int rowNum)
        {
            int columCount = 5;
            while (true)
            {
                if (!bIsExecute)
                    break;
                string str = excelReader.GetCellString(rowNum, columCount).PadLeft(4, '0');
                if (str == "ENDL")
                    break;

                UInt32 Get_Mac;
                byte[] mac = new byte[2];
                Get_Mac = Convert.ToUInt32(str, 16);
                mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);
                mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                manager.disarm(mac);

                columCount += 4;
                Thread.Sleep(50);
            }
        }

        private void Gotoposition(int rowNum)
        {
            int columCount = 5;
            while (true)
            {
                if (!bIsExecute)
                    break;
                string str = excelReader.GetCellString(rowNum, columCount).PadLeft(4, '0');
                if (str == "ENDL")
                    break;

                UInt32 Get_Mac;
                byte[] mac = new byte[2];
                Get_Mac = Convert.ToUInt32(str, 16);
                mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);
                mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                float ba = 360 / (float)(40030173 * Math.Cos(baseLat * Math.PI / 180));
                manager.gotolocation(mac, baseLng + (float)excelReader.GetCellDouble(rowNum, columCount + 1) * ba, baseLat + (float)excelReader.GetCellDouble(rowNum, columCount + 2) * 0.000009f, (float)excelReader.GetCellDouble(rowNum, columCount + 3) + baseAlt);

                columCount += 4;
                Thread.Sleep(50);
            }
        }

        private void Return(int rowNum)
        {
            int columCount = 5;
            while (true)
            {
                if (!bIsExecute)
                    break;
                string str = excelReader.GetCellString(rowNum, columCount).PadLeft(4, '0');
                if (str == "ENDL")
                    break;

                UInt32 Get_Mac;
                byte[] mac = new byte[2];
                Get_Mac = Convert.ToUInt32(str, 16);
                mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);
                mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                manager.return2launch(mac);

                columCount += 4;
                Thread.Sleep(50);
            }
        }

        private void ExcuteTask()
        {
            bIsExecute = true;
            for (int row = 1; row <= excelReader.GetRowCount(); row++)
            {

                if (!bIsExecute)
                    break;
                string cmdStr = excelReader.GetCellString(row, 3);
                int sleepTimer = (int)(float.Parse(excelReader.GetCellString(row, 4)) * 1000);
                switch (cmdStr)
                {
                    case "ARM":
                        Arm(row);
                        Thread.Sleep(sleepTimer);
                        break;
                    case "TAKEOFF":
                        Takeoff(row);
                        Thread.Sleep(sleepTimer);
                        break;
                    case "DISARM":
                        Disarm(row);
                        Thread.Sleep(sleepTimer);
                        break;
                    case "GOTO":
                        Gotoposition(row);
                        Thread.Sleep(sleepTimer);
                        break;     
                    case "RTL":
                        Return(row);
                        Thread.Sleep(sleepTimer);
                        break;
                    default:
                        break;
                }
            }

            Invoke((EventHandler)(delegate
            {
                bIsExecute = false;
            }));
        }

        private void skinButton_Execute_Click(object sender, EventArgs e)
        {
            Regex reg = new Regex(@"^(\-|\+)?\d+(\.\d{6})$");
            if (!reg.IsMatch(skinTextBox_lngFlag.Text))
            {
                MessageBox.Show(dictionaryLanguage["TEXT_LNGERROR"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!reg.IsMatch(skinTextBox_latFlag.Text))
            {
                MessageBox.Show(dictionaryLanguage["TEXT_LATERROR"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Regex reg1 = new Regex(@"^(\-|\+)?\d+(\.\d+)?$");
            if (!reg1.IsMatch(skinTextBox_altFlag.Text))
            {
                MessageBox.Show(dictionaryLanguage["TEXT_ALTERROR"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            baseLng = float.Parse(skinTextBox_lngFlag.Text);
            baseLat = float.Parse(skinTextBox_latFlag.Text);
            baseAlt = float.Parse(skinTextBox_altFlag.Text);

            bIsExecute = true;
            skinButton_Execute.Enabled = false;

            Thread executeTask = new Thread(new ThreadStart(ExcuteTask));
            executeTask.Start();
        }

        private void skinButton_ALLPAUSE_Click(object sender, EventArgs e)
        {
            bIsExecute = false;
            for (int i = 1; i < listBox_Ready.Items.Count; i++)
            {
                UInt32 Get_Mac;
                byte[] mac = new byte[2];
                Get_Mac = Convert.ToUInt32(listBox_Ready.Items[i].ToString(), 16);
                mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);
                mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                manager.pause(mac);
            }
        }

        private void FrmMain_SysBottomClick(object sender, SysButtonEventArgs e)
        {
            if (e.SysButton.Name == "SysTool")
            {
                Point l = PointToScreen(e.SysButton.Location);
                l.Y += e.SysButton.Size.Height + 1;
                FramMenu.Show(l);
            }
        }

        private void OfficialToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.robsense.com/");
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutFram about = new AboutFram(languageStr);
            about.ShowDialog();
        }

        private void CHSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CHSToolStripMenuItem.Checked = true;
            EnToolStripMenuItem.Checked = false;
            languageStr = "zh-CN";
            LoadLanguage(languageStr);
            string path = Application.StartupPath;
            path += "\\config.ini";

            WritePrivateProfileString("MAIN", "language", languageStr, path);
        }

        private void EnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CHSToolStripMenuItem.Checked = false;
            EnToolStripMenuItem.Checked = true;
            languageStr = "en-US";
            LoadLanguage(languageStr);

            string path = Application.StartupPath;
            path += "\\config.ini";
        
            WritePrivateProfileString("MAIN", "language", languageStr, path);
        }
        
        private void clear_OnlineNode()
        {
            listBox_Ready.Items.Clear();
            listBox_Ready.Items.Add("ALL");
            listBox_OnlineNode.Items.Clear();
            skinComboBox_nodesMAC.Items.Clear();
            onlineCount = 0;
            skinLabel_OnlineNodesCount.Text = "";
        } 
    
        private void skinButton_OpenCom_Click(object sender, EventArgs e)
        {
            if (!bIsOpen)
            {
                skinProgressBar1.Value = 0;
                skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_OPENSERIAL"];
                if (!manager.connect(skinComboBox_ComNum.Text))
                {
                    skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_OPENFAILED"];
                    MessageBox.Show(dictionaryLanguage["TEXT_OPENFAILED"]);
                    return;
                }
                skinButton_OpenCom.Text = dictionaryLanguage["TEXT_CLOSE"];
                bIsOpen = true;

                skinButton_AddMAC.Enabled = true;
                skinButton_DeleteMAC.Enabled = true;
                skinButton_importMAC.Enabled = true;
                skinButton_ClearMAC.Enabled = true;

                listBox_WhiteList.Items.Clear();
                clear_OnlineNode();
                skinLabel_CurrentComNum.Text = skinComboBox_ComNum.Text;
                manager.refresh_ap();
                manager.refresh_ap();
                skinProgressBar1.Maximum = 100;
                skinProgressBar1.Value = 100;
                skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_OPENTIP"];
                Thread heatThread = new Thread(new ThreadStart(tool_getWhiteList));
                heatThread.Start();
            }
            else
            {
                if (!manager.disconnect())
                {
                    MessageBox.Show(dictionaryLanguage["TEXT_CLOSEFAILED"]);
                    return;
                }
                
                skinButton_Measure.Text = dictionaryLanguage["TEXT_STARTTEST"];
                bIsGetRssi = false;
                skinButton_OpenCom.Text = dictionaryLanguage["TEXT_OPEN"];
                listBox_WhiteList.Items.Clear();
                clear_OnlineNode();
                skinLabel_CurrentComNum.Text = "";
                bIsOpen = false;     
            }
        }

        private void skinComboBox_ComNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (skinComboBox_ComNum_Index != skinComboBox_ComNum.SelectedIndex && bIsOpen)
            {
                manager.disconnect();
                skinButton_OpenCom.Text = dictionaryLanguage["TEXT_OPEN"];
                listBox_WhiteList.Items.Clear();
                clear_OnlineNode();
                skinLabel_CurrentComNum.Text = "";
                bIsOpen = false;
            }
        }

        private void SetPortNames(string name)
        {
            skinComboBox_ComNum.Items.Add(name);
        }
        
        private void GetPortNames()
        {
            InvokeGetPortNames mi = new InvokeGetPortNames(SetPortNames);
            
            string[] serialport_name = SerialPort.GetPortNames();
           
            foreach (var item in serialport_name)
            {
                this.BeginInvoke(mi, item);
            }
        }

        private void skinComboBox_ComNum_MouseClick(object sender, MouseEventArgs e)
        {
            this.skinComboBox_ComNum.Items.Clear();
            Thread heatThread = new Thread(new ThreadStart(GetPortNames));
            heatThread.Start();           
        }

        private void setWhiteList_ComboBox(string name)
        {
            listBox_WhiteList.Items.Add(name);
        }

        private void tool_getWhiteList()
        {
            InvokeGetWhiteLists mi = new InvokeGetWhiteLists(setWhiteList_ComboBox);

            while (true)
            {
                string settings = manager.get_settings();
               
                if (settings != null)
                {
                    string[] arr = settings.Split(' ');

                    for (int i = 0; i < arr.Length - 1; i++)
                    {
                        for (int j = 0; j < arr.Length - 1 - i; j++)
                        {
                            if (arr[j].CompareTo(arr[j + 1]) > 0)
                            {
                                string tmp = arr[j];
                                arr[j] = arr[j + 1];
                                arr[j + 1] = tmp;
                            }
                        }
                    }

                    for (int i = 0; i < arr.Length; i++)
                    {
                        if (arr[i] == "")
                            continue;
                        this.BeginInvoke(mi, arr[i]);
                    }

                    return;
                }
                Thread.Sleep(50);
            }
        }

        private void skinButton_AddMAC_Click(object sender, EventArgs e)
        {
            try
            {
                Regex reg = new Regex(@"^\d{4}$");
                if (!reg.IsMatch(skinTextBox_MAC.Text))
                {
                    MessageBox.Show(dictionaryLanguage["TEXT_MACERROR"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!listBox_WhiteList.Items.Contains(skinTextBox_MAC.Text) && bIsOpen)  //If repeat not add.
                {          
                    uint mac = uint.Parse(skinTextBox_MAC.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
                    byte[] node_mac = new byte[2];
                    node_mac[0] = (byte)((mac >> 8) & 0x000000ff);
                    node_mac[1] = (byte)(mac & 0x00000000ff);
                    manager.add_node(node_mac);

                    manager.clear_settings();
                    listBox_WhiteList.Items.Clear();
                    manager.refresh_ap();
                    manager.refresh_ap();
                    Thread heatThread = new Thread(new ThreadStart(tool_getWhiteList));
                    heatThread.Start();
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void skinButton_DeleteMAC_Click(object sender, EventArgs e)
        {
            if (bIsOpen)
            {
                if (listBox_WhiteList.SelectedIndex == -1)
                {
                    MessageBox.Show(dictionaryLanguage["TEXT_SELECTNODEERROR"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                uint mac = uint.Parse(listBox_WhiteList.SelectedItem.ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                byte[] node_mac = new byte[2];
                node_mac[0] = (byte)((mac >> 8) & 0x000000ff);
                node_mac[1] = (byte)(mac & 0x00000000ff);
                manager.delete_node(node_mac);
                listBox_WhiteList.Items.Remove(listBox_WhiteList.SelectedItem);

                manager.clear_settings();
                listBox_WhiteList.Items.Clear();
                manager.refresh_ap();
                manager.refresh_ap();
                Thread heatThread = new Thread(new ThreadStart(tool_getWhiteList));
                heatThread.Start();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinButton_RefreshWhiteList_Click(object sender, EventArgs e)
        {
            listBox_WhiteList.Items.Clear();
            listBox_OnlineNode.Items.Clear();
            listBox_Ready.Items.Clear();
            clear_OnlineNode();
            manager.refresh_ap();
            manager.refresh_ap();
            Thread heatThread = new Thread(new ThreadStart(tool_getWhiteList));
            heatThread.Start();
        }

        private void skinButton_RefreshOnline_Click(object sender, EventArgs e)
        {
            if (listBox_WhiteList.Items.Count == 0)
                return;
            skinButton_RefreshOnline.Enabled = false;
            btn_RefreshOLDrone.Enabled = false;
            skinButton_AddMAC.Enabled = false;
            skinButton_DeleteMAC.Enabled = false;
            skinButton_importMAC.Enabled = false;
            skinButton_ClearMAC.Enabled = false;

            listBox_OnlineNode.Items.Clear();
            listBox_Ready.Items.Clear();
            listBox_Ready.Items.Add("ALL");
            skinComboBox_nodesMAC.Items.Clear();

            onlineCount = 0;
            skinProgressBar1.Maximum = listBox_WhiteList.Items.Count;
            skinProgressBar1.Value = 0;
            skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_REFRESHOLNODE"];
            Thread heatThread = new Thread(new ThreadStart(CheckNode));
            heatThread.Start();
        }

        private delegate void InvokeProgressBarChange(int count, bool bIsEnd);
        private void CheckNode()
        {
            InvokeProgressBarChange mi = new InvokeProgressBarChange(progressBarChange_checkNode);
            for (int i = 0; i < listBox_WhiteList.Items.Count; i++)
            {
                if (i + 1 == listBox_WhiteList.Items.Count)
                    this.BeginInvoke(mi, i + 1, true);
                else
                    this.BeginInvoke(mi, i + 1, false);
                manager.CheckNode(listBox_WhiteList.Items[i].ToString());
                Thread.Sleep(200);
            }

            this.Invoke((EventHandler)(delegate
            {
                skinButton_RefreshOnline.Enabled = true;
                btn_RefreshOLDrone.Enabled = true;
                skinButton_AddMAC.Enabled = true;
                skinButton_DeleteMAC.Enabled = true;
                skinButton_importMAC.Enabled = true;
                skinButton_ClearMAC.Enabled = true;
            }));
        }

        private void progressBarChange_checkNode(int count, bool bIsEnd)
        {
            if (bIsEnd)
            {
                skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_GETSUCCESS"];
                skinButton_AddMAC.Enabled = true;
                skinButton_DeleteMAC.Enabled = true;
                skinButton_importMAC.Enabled = true;
                skinButton_ClearMAC.Enabled = true;
            }
            skinProgressBar1.Value = count;
        }

        private void InvokesetListBox_OnlineNode(object sender, Lora.StringEvent e)
        {
            InvokeSetOnlineList mi = new InvokeSetOnlineList(setListBox_OnlineNode);
            this.BeginInvoke(mi, e.node);
        }

        private void setListBox_OnlineNode(string node)
        {
            onlineCount++;
            listBox_OnlineNode.Items.Add(node);
            listBox_Ready.Items.Add(node);
            skinComboBox_nodesMAC.Items.Add(node);
            skinLabel_OnlineNodesCount.Text = onlineCount.ToString();
        }

        private void skinButton_Arm_Click(object sender, EventArgs e)
        {
            if (listBox_Ready.SelectedIndex < 0)
            {
                MessageBox.Show(dictionaryLanguage["TEXT_SELECTARM"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (listBox_Ready.Text == "ALL")
                {
                    manager.arm();
                }
                else
                {
                    UInt32 Get_Mac;
                    byte[] mac = new byte[2];
                    Get_Mac = Convert.ToUInt32(listBox_Ready.SelectedItem.ToString(), 16);// String convert to uin16
                    mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);                      //uint16 convert to byte
                    mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                    manager.arm(mac);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        private void skinButton_Takeoff_Click(object sender, EventArgs e)
        {
            if (listBox_Ready.SelectedIndex < 0)
            {
                MessageBox.Show(dictionaryLanguage["TEXT_SELECTTAKEOFF"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (listBox_Ready.Text == "ALL")
                {
                    string altStr = skinTextBox_takeoffAlt.Text;
                    Regex reg = new Regex(@"^(\-|\+)?\d+(\.\d+)?$");
                    if (!reg.IsMatch(altStr))
                    {
                        MessageBox.Show(dictionaryLanguage["TEXT_ALTERROR"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    float alt = float.Parse(altStr);
                    manager.takeoff(alt);
                }
                else
                {
                    UInt32 Get_Mac;
                    byte[] mac = new byte[2];
                    Get_Mac = Convert.ToUInt32(listBox_Ready.SelectedItem.ToString(), 16);// String convert to uin16
                    mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);                      //uint16 convert to byte
                    mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                    string alt = skinTextBox_takeoffAlt.Text;
                    Regex reg = new Regex(@"^(\-|\+)?\d+(\.\d+)?$");
                    if (!reg.IsMatch(alt))
                    {
                        MessageBox.Show(dictionaryLanguage["TEXT_ALTERROR"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    manager.takeoff(mac, float.Parse(alt));
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        private void skinButton_Land_Click(object sender, EventArgs e)
        {
            if (listBox_Ready.SelectedIndex < 0)
            {
                MessageBox.Show(dictionaryLanguage["TEXT_SELECTLAND"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (listBox_Ready.Text == "ALL")
                {
                    manager.land();
                }
                else
                {
                    UInt32 Get_Mac;
                    byte[] mac = new byte[2];
                    Get_Mac = Convert.ToUInt32(listBox_Ready.SelectedItem.ToString(), 16);// String convert to uin16
                    mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);                      //uint16 convert to byte
                    mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                    manager.land(mac);
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        private void skinButton_Disarm_Click(object sender, EventArgs e)
        {
            if (listBox_Ready.SelectedIndex < 0)
            {
                MessageBox.Show(dictionaryLanguage["TEXT_SELECTDISARM"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (listBox_Ready.Text == "ALL")
                {
                    manager.disarm();
                }
                else
                {
                    UInt32 Get_Mac;
                    byte[] mac = new byte[2];
                    Get_Mac = Convert.ToUInt32(listBox_Ready.SelectedItem.ToString(), 16);// String convert to uin16
                    mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);                      //uint16 convert to byte
                    mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                    manager.disarm(mac);
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        private void skinButton_Pause_Click(object sender, EventArgs e)
        {
            if (listBox_Ready.SelectedIndex < 0)
            {
                MessageBox.Show(dictionaryLanguage["TEXT_SELECTPAUSE"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (listBox_Ready.Text == "ALL")
                {
                    manager.pause();
                }
                else
                {
                    UInt32 Get_Mac;
                    byte[] mac = new byte[2];
                    Get_Mac = Convert.ToUInt32(listBox_Ready.SelectedItem.ToString(), 16);// String convert to uin16
                    mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);                      //uint16 convert to byte
                    mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                    manager.pause(mac);
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        private void skinButton_Return_Click(object sender, EventArgs e)
        {
            if (listBox_Ready.SelectedIndex < 0)
            {
                MessageBox.Show(dictionaryLanguage["TEXT_SELECTRTL"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (listBox_Ready.Text == "ALL")
                {
                    manager.return2launch();
                }
                else
                {
                    UInt32 Get_Mac;
                    byte[] mac = new byte[2];
                    Get_Mac = Convert.ToUInt32(listBox_Ready.SelectedItem.ToString(), 16);// String convert to uin16
                    mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);                      //uint16 convert to byte
                    mac[1] = Convert.ToByte(Get_Mac & 0x00ff);
                    manager.return2launch(mac);
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        private void progressBarChange_clearMac(int count, bool bIsEnd)
        {
            if (bIsEnd)
            {
                skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_CLEARSUCCESS"];
                skinButton_AddMAC.Enabled = true;
                skinButton_DeleteMAC.Enabled = true;
                skinButton_importMAC.Enabled = true;
                skinButton_ClearMAC.Enabled = true;

                listBox_WhiteList.Items.Clear();
                clear_OnlineNode();
                manager.clear_settings();
                manager.refresh_ap();
                manager.refresh_ap();
                Thread heatThread = new Thread(new ThreadStart(tool_getWhiteList));
                heatThread.Start();
            }
            skinProgressBar1.Value = count;
        }

        private void deleteMac()
        {
            InvokeProgressBarChange mi = new InvokeProgressBarChange(progressBarChange_clearMac);
            for (int i = 0; i < listBox_WhiteList.Items.Count; i++)
            {
                uint mac = uint.Parse(listBox_WhiteList.Items[i].ToString(), System.Globalization.NumberStyles.AllowHexSpecifier);
                byte[] node_mac = new byte[2];
                node_mac[0] = (byte)((mac >> 8) & 0x000000ff);
                node_mac[1] = (byte)(mac & 0x00000000ff);
                manager.delete_node(node_mac);
                if (i + 1 == listBox_WhiteList.Items.Count)
                    this.BeginInvoke(mi, i + 1, true);
                else
                    this.BeginInvoke(mi, i + 1, false);
                Thread.Sleep(200);
            }
        }

        private void skinButton_ClearMAC_Click(object sender, EventArgs e)
        {
            if (listBox_WhiteList.Items.Count == 0)
                return;

            skinButton_AddMAC.Enabled = false;
            skinButton_DeleteMAC.Enabled = false;
            skinButton_importMAC.Enabled = false;
            skinButton_ClearMAC.Enabled = false;

            skinProgressBar1.Maximum = listBox_WhiteList.Items.Count;
            skinProgressBar1.Value = 0;
            skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_CLEARWL"];
            Thread heatThread = new Thread(new ThreadStart(deleteMac));
            heatThread.Start();
        }

        private void progressBarChange_addMac(int count, bool bIsEnd)
        {
            if (bIsEnd)
            {
                skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_IMPORTSUCCESS"];
                skinButton_AddMAC.Enabled = true;
                skinButton_DeleteMAC.Enabled = true;
                skinButton_importMAC.Enabled = true;
                skinButton_ClearMAC.Enabled = true;

                listBox_WhiteList.Items.Clear();
                clear_OnlineNode();
                manager.clear_settings();
                manager.refresh_ap();
                manager.refresh_ap();
                Thread heatThread = new Thread(new ThreadStart(tool_getWhiteList));
                heatThread.Start();
            }
            skinProgressBar1.Value = count;
        }

        private void addMac()
        {
            InvokeProgressBarChange mi = new InvokeProgressBarChange(progressBarChange_addMac);
            for (int i = 0; i < importMacList.Count; i++)
            {
                uint mac = uint.Parse(importMacList[i], System.Globalization.NumberStyles.AllowHexSpecifier);
                byte[] node_mac = new byte[2];
                node_mac[0] = (byte)((mac >> 8) & 0x000000ff);
                node_mac[1] = (byte)(mac & 0x00000000ff);
                manager.add_node(node_mac);
                if (i + 1 == importMacList.Count)
                    this.BeginInvoke(mi, i + 1, true);
                else
                    this.BeginInvoke(mi, i + 1, false);
                Thread.Sleep(200);
            }
        }

        private void skinButton_importMAC_Click(object sender, EventArgs e)
        {
            if (!bIsOpen)
                return;

            importMacList.Clear();

            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = dictionaryLanguage["TEXT_MACLIST"];
            fdlg.Filter = "txt（*.txt）|*.txt";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;

            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                string str = fdlg.FileName;

                FileStream fs = new FileStream(str, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader mysr = new StreamReader(fs, Encoding.Default);

                string strline = null;
                while ((strline = mysr.ReadLine()) != null)
                {
                    Regex reg = new Regex(@"^\d{4}$");
                    if (reg.IsMatch(strline))
                        importMacList.Add(strline);
                }  
            }

            if (importMacList.Count == 0)
                return;

            skinButton_AddMAC.Enabled = false;
            skinButton_DeleteMAC.Enabled = false;
            skinButton_importMAC.Enabled = false;
            skinButton_ClearMAC.Enabled = false;

            skinProgressBar1.Maximum = importMacList.Count;
            skinProgressBar1.Value = 0;
            skinLabel_ProgressTip.Text = dictionaryLanguage["TEXT_IMPORTMAC"];
            Thread heatThread = new Thread(new ThreadStart(addMac));
            heatThread.Start();
        }

        private void skinButton_setPX4_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath;
            path += "\\config.ini";
            if (skinRadioButton_PX4.Checked)
            {
                manager = new Manager("PX4");
                manager.delecheckNodeEvent += new Manager.delegateCheckNodeEvent(InvokesetListBox_OnlineNode);
                manager.deleGetRssiEvent += new Manager.delegateGetRssiEvent(drawRssi);
                WritePrivateProfileString("MAIN", "APMorPX4", "PX4", path);
                WritePrivateProfileString("MAIN", "firmwareIndex", skinComboBox_px4.SelectedIndex.ToString(), path);
                //skinButton_OpenCom_Click(this, new EventArgs());
            }
            else
            {
                //skinButton_OpenCom_Click(this, new EventArgs());
                manager = new Manager("APM");
                manager.delecheckNodeEvent += new Manager.delegateCheckNodeEvent(InvokesetListBox_OnlineNode);
                manager.deleGetRssiEvent += new Manager.delegateGetRssiEvent(drawRssi);
                WritePrivateProfileString("MAIN", "APMorPX4", "APM", path);
                WritePrivateProfileString("MAIN", "firmwareIndex", skinComboBox_px4.SelectedIndex.ToString(), path);
            }
        }

        private void skinButton_modifyMAC_Click(object sender, EventArgs e)
        {
            if (skinComboBox_nodesMAC.SelectedIndex < 0)
            {
                MessageBox.Show(dictionaryLanguage["TEXT_SELECTMODIFY"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            Regex reg = new Regex(@"^\d{4}$");
            if (!reg.IsMatch(skinTextBox_newNodeMAC.Text))
            {
                MessageBox.Show(dictionaryLanguage["TEXT_NEWMACERROR"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            uint mac = uint.Parse(skinComboBox_nodesMAC.Text, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] oldMac = new byte[2];
            oldMac[0] = (byte)((mac >> 8) & 0x000000ff);
            oldMac[1] = (byte)(mac & 0x00000000ff);

            string newNode = skinTextBox_newNodeMAC.Text;
            mac = uint.Parse(newNode, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] newMac = new byte[2];
            newMac[0] = (byte)((mac >> 8) & 0x000000ff);
            newMac[1] = (byte)(mac & 0x00000000ff);

            manager.set_node_mac(oldMac, newMac);

            MessageBox.Show(dictionaryLanguage["TEXT_SETSUCCESS"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void skinButton_goto_Click(object sender, EventArgs e)
        {
            if (listBox_Ready.SelectedIndex < 0)
            {
                MessageBox.Show(dictionaryLanguage["TEXT_SELECTGOTO"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (listBox_Ready.Text == "ALL")
                {
                    MessageBox.Show(dictionaryLanguage["TEXT_NOALL"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    if (userMapControl_Map.makerOverlay.Markers.Count <= 0)
                    {
                        MessageBox.Show(dictionaryLanguage["TEXT_NOMAKER"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (userMapControl_Map.selectedId < 0 || userMapControl_Map.selectedId > userMapControl_Map.makerOverlay.Markers.Count)
                    {
                        MessageBox.Show(dictionaryLanguage["TEXT_NOSELECTMAKER"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                   
                    UInt32 Get_Mac;
                    byte[] mac = new byte[2];
                    Get_Mac = Convert.ToUInt32(listBox_Ready.SelectedItem.ToString(), 16);// String convert to uin16
                    mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);                      //uint16 convert to byte
                    mac[1] = Convert.ToByte(Get_Mac & 0x00ff);

                    float lng = (float)userMapControl_Map.makerOverlay.Markers[userMapControl_Map.selectedId - 1].Position.Lng;
                    float lat = (float)userMapControl_Map.makerOverlay.Markers[userMapControl_Map.selectedId - 1].Position.Lat;
                    float alt = ((CustomMarker)userMapControl_Map.makerOverlay.Markers[userMapControl_Map.selectedId - 1]).alt;
                    manager.gotolocation(mac, lng, lat, alt);

                    userMapControl_Map.NextPostion();
                }

            }
            catch (Exception exp)
            {
                Console.WriteLine(exp);
            }
        }

        private void getRssi()
        {
            UInt32 Get_Mac;
            byte[] mac = new byte[2];
            Get_Mac = Convert.ToUInt32(strSelectNode, 16);// String convert to uin16
            mac[0] = Convert.ToByte((Get_Mac >> 8) & 0x00ff);                      //uint16 convert to byte
            mac[1] = Convert.ToByte(Get_Mac & 0x00ff);

            while (bIsGetRssi)
            {
                this.Invoke((EventHandler)(delegate
                {
                    if (sendCount != 0)
                        skinLabel_lossRate.Text = ((float)((float)(sendCount - receiveCount) / (float)sendCount * 100)).ToString() + "%";
                }));
                manager.get_rssi(mac, getRssiCount);
                if (getRssiCount == 0xFF)
                    getRssiCount = 0;
                else
                    ++getRssiCount;
                this.Invoke((EventHandler)(delegate
                {
                    skinLabel_gwQueries.Text = sendCount.ToString() + "/" + receiveCount.ToString();
                    skinLabel_NodeQueries.Text = sendCount.ToString() + "/" + receiveCount.ToString();
                    ++sendCount;
                }));
                Thread.Sleep(1000);
            }
        }

        private void Timer()
        {
            while (bIsGetRssi)
            {
                this.Invoke((EventHandler)(delegate
                {
                    skinLabel_timer.Text = timerCount.ToString() + " S";
                    timerCount++;
                    if ((DateTime.Now.Ticks - timeOut) / 10000000 >= 5)
                    {
                        lab_testTip.Text = dictionaryLanguage["TEXT_TESTTIP_2"];
                        skinButton_Measure.Text = dictionaryLanguage["TEXT_STARTTEST"];
                        skinLabel1_gatewayQ.Text = " ";
                        skinLabel_nodeQ.Text = " ";
                        bIsGetRssi = false;
                        Thread heatThread = new Thread(new ThreadStart(disconnect));
                        heatThread.Start();
                    }
                }));
                Thread.Sleep(1000);
            }
        }

        private void disconnect()
        {
            MessageBox.Show(strSelectNode + dictionaryLanguage["TEXT_NODEDISCONNECT"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        //private int perReceiveNum = -2;
        //private int lossCount = 0;

        //private void drawRssi()
        //{
        //    InitRssi();

        //    int width = skinPanel4.Size.Width;
        //    int height = skinPanel4.Size.Height;

        //    float ir = (float)height / 148;
        //    Bitmap b = new Bitmap(width, height);
        //    Graphics g = Graphics.FromImage(b);
        //    int count = 0;
        //    int x = (20 - width % 20) % 5;
        //    int y = (20 - width % 20) / 5;
        //    int test = 0;
        //    uint allCount = 0;

        //    while (bIsGetRssi)
        //    {
        //        Bitmap tmp = new Bitmap(width, height);
        //        Bitmap b1 = new Bitmap(5, height - 1);
        //        Graphics g1 = Graphics.FromImage(b1);
        //        // g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        //        this.Invoke((EventHandler)(delegate
        //        {
        //            skinPanel4.DrawToBitmap(tmp, new Rectangle(0, 0, width, height));
        //        }));

        //        float tmpdBm = manager.node_dBm;
        //        float tmp_gwdBm = manager.gw_dBm;
        //        this.Invoke((EventHandler)(delegate
        //        {
        //            skinLabel1_gatewayQ.Text = tmp_gwdBm.ToString() + " dBm";
        //            skinLabel_nodeQ.Text = tmpdBm.ToString() + " dBm";
        //        }));

        //        if (tmpdBm == 0)
        //            tmpdBm = perNodedBm;
        //        if (tmp_gwdBm == 0)
        //            tmp_gwdBm = perGatewaydBm;

        //        Invoke((EventHandler)(delegate
        //        {
        //            int tmpRe = manager.receiveNum;
        //            if (tmpRe != perReceiveNum)
        //            {
        //                receiveCount = (uint)tmpRe + 255 * allCount;
        //                if (tmpRe == 255)
        //                    ++allCount;

        //                perReceiveNum = tmpRe;
        //            }
        //            else
        //            {
        //                ++lossCount;
        //                if (lossCount == 6)
        //                {
        //                    this.Invoke((EventHandler)(delegate
        //                    {
        //                        lab_testTip.Text = "节点已断开";
        //                        skinButton_Measure.Text = "开始测量";
        //                        skinLabel1_gatewayQ.Text = " ";
        //                        skinLabel_nodeQ.Text = " ";
        //                        bIsGetRssi = false;
        //                        Thread heatThread = new Thread(new ThreadStart(disconnect));
        //                        heatThread.Start();
        //                    }));
        //                    return;
        //                }
        //            }
        //            skinLabel_gwQueries.Text = (sendCount - 1).ToString() + "/" + receiveCount.ToString() ;
        //            skinLabel_NodeQueries.Text = (sendCount - 1).ToString() + "/" + receiveCount.ToString();
        //            skinLabel_lossRate.Text = ((double)lossCount * 100 / (double)sendCount).ToString() + " %";
        //        }));

        //        for (int i = 0; i < height; i += 20)
        //        {
        //            g1.DrawLine(new Pen(Color.FromArgb(50, Color.Gray), 1), new Point(0, i), new Point(width, i));
        //        }
        //        if (test == 0 && (width % 20 == 1))
        //        {
        //            g1.DrawLine(new Pen(Color.FromArgb(50, Color.Gray), 1), new Point(0, 0), new Point(0, height));
        //            test++;
        //        }

        //        if (count == y)
        //            g1.DrawLine(new Pen(Color.FromArgb(50, Color.Gray), 1), new Point(x, 0), new Point(x, height));

        //        if (++count == 4)
        //            count = 0;
        //        g1.DrawLine(new Pen(Color.DarkMagenta, 2), new PointF(0, Math.Abs(perNodedBm) * ir), new PointF(5, Math.Abs(tmpdBm) * ir));
        //        g1.DrawLine(new Pen(Color.Red, 2), new PointF(0, Math.Abs(perGatewaydBm) * ir), new PointF(5, Math.Abs(tmp_gwdBm) * ir));

        //        g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, width, height));
        //        g.DrawImage(tmp, new Rectangle(0, 0, width - 6, height), new Rectangle(5, 0, width - 6, height), GraphicsUnit.Pixel);
        //        g.DrawImage(b1, new Rectangle(width - 6, 0, 5, height), new Rectangle(0, 0, 5, height), GraphicsUnit.Pixel);
        //        perNodedBm = tmpdBm;
        //        perGatewaydBm = tmp_gwdBm;

        //        skinPanel4.BackgroundImage = b;
        //        Invoke((EventHandler)(delegate
        //        {
        //            skinPanel4.Invalidate();
        //        }));
        //        Thread.Sleep(1000);
        //    }

        //    g.Dispose();
        //}


        private void drawRssi(Object sendr, Lora.IntEvent e)
        {
            if (!bIsGetRssi)
                return;

            timeOut = DateTime.Now.Ticks;
        
            float ir = (float)height / 148;
            Bitmap b = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(b);
          
            Bitmap tmp = new Bitmap(width, height);
            Bitmap b1 = new Bitmap(5, height - 1);
            Graphics g1 = Graphics.FromImage(b1);
            // g1.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            this.Invoke((EventHandler)(delegate
            {
                skinPanel4.DrawToBitmap(tmp, new Rectangle(0, 0, width, height));
            }));

            float tmpdBm = e.node_value;//manager.node_dBm;
            float tmp_gwdBm = e.gw_value;// manager.gw_dBm;
            this.Invoke((EventHandler)(delegate
            {
                skinLabel1_gatewayQ.Text = tmp_gwdBm.ToString() + " dBm";
                skinLabel_nodeQ.Text = tmpdBm.ToString() + " dBm";
            }));

            if (tmpdBm == 0)
                tmpdBm = perNodedBm;
            if (tmp_gwdBm == 0)
                tmp_gwdBm = perGatewaydBm;

            Invoke((EventHandler)(delegate
            {
                receiveCount++; 
                skinLabel_gwQueries.Text = sendCount.ToString() + "/" + receiveCount.ToString();
                skinLabel_NodeQueries.Text = sendCount.ToString() + "/" + receiveCount.ToString();
            }));

            for (int i = 0; i < height; i += 20)
            {
                g1.DrawLine(new Pen(Color.FromArgb(50, Color.Gray), 1), new Point(0, i), new Point(width, i));
            }
            if (test == 0 && (width % 20 == 1))
            {
                g1.DrawLine(new Pen(Color.FromArgb(50, Color.Gray), 1), new Point(0, 0), new Point(0, height));
                test++;
            }

            if (count == y)
                g1.DrawLine(new Pen(Color.FromArgb(50, Color.Gray), 1), new Point(x, 0), new Point(x, height));

            if (++count == 4)
                count = 0;
            g1.DrawLine(new Pen(Color.DarkMagenta, 2), new PointF(0, Math.Abs(perNodedBm) * ir), new PointF(5, Math.Abs(tmpdBm) * ir));
            g1.DrawLine(new Pen(Color.Red, 2), new PointF(0, Math.Abs(perGatewaydBm) * ir), new PointF(5, Math.Abs(tmp_gwdBm) * ir));

            g.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, width, height));
            g.DrawImage(tmp, new Rectangle(0, 0, width - 6, height), new Rectangle(5, 0, width - 6, height), GraphicsUnit.Pixel);
            g.DrawImage(b1, new Rectangle(width - 6, 0, 5, height), new Rectangle(0, 0, 5, height), GraphicsUnit.Pixel);
            perNodedBm = tmpdBm;
            perGatewaydBm = tmp_gwdBm;

            skinPanel4.BackgroundImage = b;
            Invoke((EventHandler)(delegate
            {
                skinPanel4.Invalidate();
            }));
            g.Dispose();
        }

        private void skinButton_Measure_Click(object sender, EventArgs e)
        {
            if (bIsGetRssi ^= true)
            {
                if (listBox_OnlineNode.SelectedIndex < 0)
                {
                    MessageBox.Show(dictionaryLanguage["TEXT_SELECTTEST"], dictionaryLanguage["TEXT_WARMING"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                InitRssi();

                timerCount = 0;
                sendCount = 0;
                receiveCount = 0;
                perNodedBm = 0;
                perGatewaydBm = 0;
                getRssiCount = 0;
                //perReceiveNum = -2;
                //lossCount = 0;

                width = skinPanel4.Size.Width;
                height = skinPanel4.Size.Height;
                count = 0;
                x = (20 - width % 20) % 5;
                y = (20 - width % 20) / 5;
                test = 0;

                timeOut = DateTime.Now.Ticks;

                skinButton_Measure.Text = dictionaryLanguage["TEXT_STOPTEST"];
                lab_testTip.Text = dictionaryLanguage["TEXT_TESTTIP_1"];
                strSelectNode = listBox_OnlineNode.Text;
                lab_curTestNode.Text = strSelectNode;
                Thread heatThread = new Thread(new ThreadStart(getRssi));
                heatThread.Start();
                Thread timerThread = new Thread(new ThreadStart(Timer));
                timerThread.Start();
                //Thread drRssi = new Thread(new ThreadStart(drawRssi));
                //drRssi.Start();
            }
            else
            {
                skinButton_Measure.Text = dictionaryLanguage["TEXT_STARTTEST"];
                lab_curTestNode.Text = "";
            }  
        }

        private void skinRadioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (skinRadioButton_APM.Checked)
            {
                skinComboBox_px4.Items.Clear();
                skinComboBox_px4.Items.Add("ArduCopter V3.5.7 Quad");
                skinComboBox_px4.Items.Add("ArduCopter V3.3.3 Quad");
                skinComboBox_px4.SelectedIndex = 0;
                skinLabel_currentFirme.Text = dictionaryLanguage["TEXT_CURFIRMWARM"] + skinComboBox_px4.Text;
            }
        }

        private void skinRadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (skinRadioButton_PX4.Checked)
            {
                skinComboBox_px4.Items.Clear();
                skinComboBox_px4.Items.Add("PX4 V1.6.5");
                skinComboBox_px4.Items.Add("PX4 V1.7.2");
                skinComboBox_px4.SelectedIndex = 0;
                skinLabel_currentFirme.Text = dictionaryLanguage["TEXT_CURFIRMWARM"] + skinComboBox_px4.Text;
            }
        }

        private void btn_DeleteMaker_Click(object sender, EventArgs e)
        {
            userMapControl_Map.DeleteMarker();
        }

        private void btn_ClearMaker_Click(object sender, EventArgs e)
        {
            userMapControl_Map.ClearMarker();
        }

        private void btn_setGotoAlt_Click(object sender, EventArgs e)
        {
            userMapControl_Map.SetGotoAlt(float.Parse(tb_gotoAlt.Text));
        }
        
        private void ChangeGotoAlt(Object sender, floatEvent e)
        {
            tb_gotoAlt.Text = e.value.ToString();
        }
    }
}
