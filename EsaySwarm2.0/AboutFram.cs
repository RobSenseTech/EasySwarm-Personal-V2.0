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
using System.IO;
using Newtonsoft.Json;

namespace EasySwarm2._0
{
    public partial class AboutFram : CCSkinMain
    {
        private Dictionary<string, string> dictionaryLanguage = new Dictionary<string, string>();

        public void LoadLanguage(string file)
        {
            string path = @"language//" + file + @"//aboutFram.json";
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

            this.Text = dictionaryLanguage["TEXT_ABOUT"];
            lab_version.Text = dictionaryLanguage["TEXT_version"];
            lab_copyright.Text = dictionaryLanguage["TEXT_COPYRIGHT"];
            linkLab_www.Text = dictionaryLanguage["TEXT_WWW"];
            btn_ok.Text = dictionaryLanguage["TEXT_OK"];
        }

        public AboutFram(string language)
        {
            InitializeComponent();

            LoadLanguage(language);
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
          
            System.Diagnostics.Process.Start("http://" + linkLab_www.Text /*"http://www.robsense.com/"*/);
        }
    }
}
