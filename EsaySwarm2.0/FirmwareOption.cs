using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CCWin;
using System.Runtime.InteropServices;
using System.IO;
using Newtonsoft.Json;

namespace EasySwarm2._0
{
    public partial class FirmwareOption : CCSkinMain
    {
        #region 
        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
        string val, string filePath);

        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
        string def, StringBuilder retVal, int size, string filePath);
        #endregion
        private Dictionary<string, string> dictionaryLanguage = new Dictionary<string, string>();

        public void LoadLanguage(string file)
        {
            string path = @"language//" + file + @"//firmwareOption.json";
            var content = File.ReadAllText(path, Encoding.UTF8);
            if (!string.IsNullOrEmpty(content))
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                foreach (string key in dict.Keys)
                {
                    //遍历集合如果语言资源键值不存在，则创建，否则更新
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

            this.Text = dictionaryLanguage["TEXT_FIRMWARM"];
            btn_set.Text = dictionaryLanguage["TEXT_SET"];
            lab_ignore.Text = dictionaryLanguage["TEXT_IGNORE"];
            cb_tip.Text = dictionaryLanguage["TEXT_STARTUPTIP"];
        }

        public FirmwareOption(string language)
        {
            InitializeComponent();

            LoadLanguage(language);

            string path = Application.StartupPath;
            path += "\\config.ini";

            StringBuilder str = new StringBuilder();
            GetPrivateProfileString("MAIN", "APMorPX4", "APM", str, 500, path);

            if (str.ToString() == "APM")
            {
                GetPrivateProfileString("MAIN", "firmwareIndex", "0", str, 500, path);
                rb_ArduPoilt.Checked = true;
                combox_list.Items.Clear();
                combox_list.Items.Add("ArduCopter V3.3.3 Quad");
                combox_list.Items.Add("ArduCopter V3.5.7 Quad");
                combox_list.SelectedIndex = int.Parse(str.ToString());
            }
            else
            {
                GetPrivateProfileString("MAIN", "firmwareIndex", "0", str, 500, path);
                rb_PX4.Checked = true;
                combox_list.Items.Clear();
                combox_list.Items.Add("PX4 V1.6.5");
                combox_list.Items.Add("PX4 V1.7.2");
                combox_list.SelectedIndex = int.Parse(str.ToString());
            }
        }

        private void rb_ArduPoilt_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ArduPoilt.Checked)
            {
                combox_list.Items.Clear();
                combox_list.Items.Add("ArduCopter V3.3.3 Quad");
                combox_list.Items.Add("ArduCopter V3.5.7 Quad");
                combox_list.SelectedIndex = 0;
            }
        }

        private void rb_PX4_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_PX4.Checked)
            {
                combox_list.Items.Clear();
                combox_list.Items.Add("PX4 V1.6.5");
                combox_list.Items.Add("PX4 V1.7.2");
                combox_list.SelectedIndex = 0;
            }
        }

        private void btn_set_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath;
            path += "\\config.ini";

            if (rb_ArduPoilt.Checked)
            {
                WritePrivateProfileString("MAIN", "APMorPX4", "APM", path);
                WritePrivateProfileString("MAIN", "firmwareIndex", combox_list.SelectedIndex.ToString(), path);
            }
            else if (rb_PX4.Checked)
            {
                WritePrivateProfileString("MAIN", "APMorPX4", "PX4", path);
                WritePrivateProfileString("MAIN", "firmwareIndex", combox_list.SelectedIndex.ToString(), path);
            }
            else
            {
                WritePrivateProfileString("MAIN", "APMorPX4", "APM", path);
                WritePrivateProfileString("MAIN", "firmwareIndex", combox_list.SelectedIndex.ToString(), path);
            }

            Close();
        }

        private void cb_tip_CheckedChanged(object sender, EventArgs e)
        {
            string path = Application.StartupPath;
            path += "\\config.ini";
            if (cb_tip.Checked)
                WritePrivateProfileString("MAIN", "bIsShowFO", "0", path);
            else
                WritePrivateProfileString("MAIN", "bIsShowFO", "1", path);
        }

        private void btn_skip_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void skinLabel1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
