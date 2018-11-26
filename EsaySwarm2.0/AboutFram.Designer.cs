namespace EasySwarm2._0
{
    partial class AboutFram
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutFram));
            this.skinPanel1 = new CCWin.SkinControl.SkinPanel();
            this.btn_ok = new CCWin.SkinControl.SkinButton();
            this.linkLab_www = new System.Windows.Forms.LinkLabel();
            this.lab_copyright = new CCWin.SkinControl.SkinLabel();
            this.lab_version = new CCWin.SkinControl.SkinLabel();
            this.skinPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // skinPanel1
            // 
            this.skinPanel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.skinPanel1.Controls.Add(this.btn_ok);
            this.skinPanel1.Controls.Add(this.linkLab_www);
            this.skinPanel1.Controls.Add(this.lab_copyright);
            this.skinPanel1.Controls.Add(this.lab_version);
            this.skinPanel1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skinPanel1.DownBack = null;
            this.skinPanel1.Location = new System.Drawing.Point(4, 28);
            this.skinPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.skinPanel1.MouseBack = null;
            this.skinPanel1.Name = "skinPanel1";
            this.skinPanel1.NormlBack = null;
            this.skinPanel1.Radius = 4;
            this.skinPanel1.Size = new System.Drawing.Size(276, 129);
            this.skinPanel1.TabIndex = 0;
            // 
            // btn_ok
            // 
            this.btn_ok.BackColor = System.Drawing.Color.Transparent;
            this.btn_ok.BaseColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btn_ok.BorderColor = System.Drawing.Color.Gray;
            this.btn_ok.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_ok.DownBack = null;
            this.btn_ok.DownBaseColor = System.Drawing.Color.Gray;
            this.btn_ok.Location = new System.Drawing.Point(189, 103);
            this.btn_ok.MouseBack = null;
            this.btn_ok.MouseBaseColor = System.Drawing.Color.White;
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.NormlBack = null;
            this.btn_ok.Size = new System.Drawing.Size(75, 23);
            this.btn_ok.TabIndex = 3;
            this.btn_ok.Text = "确定";
            this.btn_ok.UseVisualStyleBackColor = false;
            this.btn_ok.Click += new System.EventHandler(this.skinButton1_Click);
            // 
            // linkLab_www
            // 
            this.linkLab_www.AutoSize = true;
            this.linkLab_www.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLab_www.Location = new System.Drawing.Point(66, 74);
            this.linkLab_www.Name = "linkLab_www";
            this.linkLab_www.Size = new System.Drawing.Size(119, 14);
            this.linkLab_www.TabIndex = 2;
            this.linkLab_www.TabStop = true;
            this.linkLab_www.Text = "www.robsense.com";
            this.linkLab_www.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // lab_copyright
            // 
            this.lab_copyright.ArtTextStyle = CCWin.SkinControl.ArtTextStyle.None;
            this.lab_copyright.AutoSize = true;
            this.lab_copyright.BackColor = System.Drawing.Color.Transparent;
            this.lab_copyright.BorderColor = System.Drawing.Color.White;
            this.lab_copyright.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_copyright.Location = new System.Drawing.Point(66, 45);
            this.lab_copyright.Name = "lab_copyright";
            this.lab_copyright.Size = new System.Drawing.Size(125, 17);
            this.lab_copyright.TabIndex = 1;
            this.lab_copyright.Text = "Copyright（C）2018";
            // 
            // lab_version
            // 
            this.lab_version.ArtTextStyle = CCWin.SkinControl.ArtTextStyle.None;
            this.lab_version.AutoSize = true;
            this.lab_version.BackColor = System.Drawing.Color.Transparent;
            this.lab_version.BorderColor = System.Drawing.Color.White;
            this.lab_version.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_version.Location = new System.Drawing.Point(66, 16);
            this.lab_version.Name = "lab_version";
            this.lab_version.Size = new System.Drawing.Size(198, 17);
            this.lab_version.TabIndex = 0;
            this.lab_version.Text = "EasySwarm Personal Edition v2.0";
            // 
            // AboutFram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 161);
            this.CloseBoxSize = new System.Drawing.Size(29, 24);
            this.CloseDownBack = ((System.Drawing.Image)(resources.GetObject("$this.CloseDownBack")));
            this.CloseMouseBack = ((System.Drawing.Image)(resources.GetObject("$this.CloseMouseBack")));
            this.CloseNormlBack = ((System.Drawing.Image)(resources.GetObject("$this.CloseNormlBack")));
            this.Controls.Add(this.skinPanel1);
            this.EffectCaption = CCWin.TitleType.Title;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutFram";
            this.RoundStyle = CCWin.SkinClass.RoundStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "关于";
            this.skinPanel1.ResumeLayout(false);
            this.skinPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CCWin.SkinControl.SkinPanel skinPanel1;
        private CCWin.SkinControl.SkinButton btn_ok;
        private System.Windows.Forms.LinkLabel linkLab_www;
        private CCWin.SkinControl.SkinLabel lab_copyright;
        private CCWin.SkinControl.SkinLabel lab_version;
    }
}