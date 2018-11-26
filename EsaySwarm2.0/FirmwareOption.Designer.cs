namespace EasySwarm2._0
{
    partial class FirmwareOption
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FirmwareOption));
            this.rb_ArduPoilt = new CCWin.SkinControl.SkinRadioButton();
            this.rb_PX4 = new CCWin.SkinControl.SkinRadioButton();
            this.combox_list = new CCWin.SkinControl.SkinComboBox();
            this.btn_set = new CCWin.SkinControl.SkinButton();
            this.cb_tip = new CCWin.SkinControl.SkinCheckBox();
            this.skinPanel1 = new CCWin.SkinControl.SkinPanel();
            this.lab_ignore = new CCWin.SkinControl.SkinLabel();
            this.skinPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rb_ArduPoilt
            // 
            this.rb_ArduPoilt.AutoSize = true;
            this.rb_ArduPoilt.BackColor = System.Drawing.Color.Transparent;
            this.rb_ArduPoilt.Checked = true;
            this.rb_ArduPoilt.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.rb_ArduPoilt.DownBack = null;
            this.rb_ArduPoilt.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rb_ArduPoilt.Location = new System.Drawing.Point(17, 14);
            this.rb_ArduPoilt.MouseBack = null;
            this.rb_ArduPoilt.Name = "rb_ArduPoilt";
            this.rb_ArduPoilt.NormlBack = null;
            this.rb_ArduPoilt.SelectedDownBack = null;
            this.rb_ArduPoilt.SelectedMouseBack = null;
            this.rb_ArduPoilt.SelectedNormlBack = null;
            this.rb_ArduPoilt.Size = new System.Drawing.Size(79, 21);
            this.rb_ArduPoilt.TabIndex = 0;
            this.rb_ArduPoilt.TabStop = true;
            this.rb_ArduPoilt.Text = "ArduPilot";
            this.rb_ArduPoilt.UseVisualStyleBackColor = false;
            this.rb_ArduPoilt.CheckedChanged += new System.EventHandler(this.rb_ArduPoilt_CheckedChanged);
            // 
            // rb_PX4
            // 
            this.rb_PX4.AutoSize = true;
            this.rb_PX4.BackColor = System.Drawing.Color.Transparent;
            this.rb_PX4.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.rb_PX4.DownBack = null;
            this.rb_PX4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rb_PX4.Location = new System.Drawing.Point(122, 14);
            this.rb_PX4.MouseBack = null;
            this.rb_PX4.Name = "rb_PX4";
            this.rb_PX4.NormlBack = null;
            this.rb_PX4.SelectedDownBack = null;
            this.rb_PX4.SelectedMouseBack = null;
            this.rb_PX4.SelectedNormlBack = null;
            this.rb_PX4.Size = new System.Drawing.Size(48, 21);
            this.rb_PX4.TabIndex = 1;
            this.rb_PX4.Text = "PX4";
            this.rb_PX4.UseVisualStyleBackColor = false;
            this.rb_PX4.CheckedChanged += new System.EventHandler(this.rb_PX4_CheckedChanged);
            // 
            // combox_list
            // 
            this.combox_list.DisplayMember = "0";
            this.combox_list.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.combox_list.FormattingEnabled = true;
            this.combox_list.Items.AddRange(new object[] {
            "ArduoCpter V3.5.7 Quad",
            "ArduCopter V3.3.3 Quad"});
            this.combox_list.Location = new System.Drawing.Point(17, 47);
            this.combox_list.Name = "combox_list";
            this.combox_list.Size = new System.Drawing.Size(166, 22);
            this.combox_list.TabIndex = 3;
            this.combox_list.WaterText = "";
            // 
            // btn_set
            // 
            this.btn_set.BackColor = System.Drawing.Color.Transparent;
            this.btn_set.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btn_set.DownBack = null;
            this.btn_set.Location = new System.Drawing.Point(221, 47);
            this.btn_set.MouseBack = null;
            this.btn_set.Name = "btn_set";
            this.btn_set.NormlBack = null;
            this.btn_set.Size = new System.Drawing.Size(50, 23);
            this.btn_set.TabIndex = 4;
            this.btn_set.Text = "设置";
            this.btn_set.UseVisualStyleBackColor = false;
            this.btn_set.Click += new System.EventHandler(this.btn_set_Click);
            // 
            // cb_tip
            // 
            this.cb_tip.AutoSize = true;
            this.cb_tip.BackColor = System.Drawing.Color.Transparent;
            this.cb_tip.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.cb_tip.DownBack = null;
            this.cb_tip.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_tip.Location = new System.Drawing.Point(17, 89);
            this.cb_tip.MouseBack = null;
            this.cb_tip.Name = "cb_tip";
            this.cb_tip.NormlBack = null;
            this.cb_tip.SelectedDownBack = null;
            this.cb_tip.SelectedMouseBack = null;
            this.cb_tip.SelectedNormlBack = null;
            this.cb_tip.Size = new System.Drawing.Size(183, 21);
            this.cb_tip.TabIndex = 5;
            this.cb_tip.Text = "下次启动不在显示此提示信息";
            this.cb_tip.UseVisualStyleBackColor = false;
            this.cb_tip.CheckedChanged += new System.EventHandler(this.cb_tip_CheckedChanged);
            // 
            // skinPanel1
            // 
            this.skinPanel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.skinPanel1.Controls.Add(this.lab_ignore);
            this.skinPanel1.Controls.Add(this.cb_tip);
            this.skinPanel1.Controls.Add(this.rb_ArduPoilt);
            this.skinPanel1.Controls.Add(this.btn_set);
            this.skinPanel1.Controls.Add(this.rb_PX4);
            this.skinPanel1.Controls.Add(this.combox_list);
            this.skinPanel1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skinPanel1.DownBack = null;
            this.skinPanel1.Location = new System.Drawing.Point(4, 28);
            this.skinPanel1.MouseBack = null;
            this.skinPanel1.Name = "skinPanel1";
            this.skinPanel1.NormlBack = null;
            this.skinPanel1.Size = new System.Drawing.Size(287, 117);
            this.skinPanel1.TabIndex = 6;
            // 
            // lab_ignore
            // 
            this.lab_ignore.AutoSize = true;
            this.lab_ignore.BackColor = System.Drawing.Color.Transparent;
            this.lab_ignore.BorderColor = System.Drawing.Color.White;
            this.lab_ignore.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_ignore.ForeColor = System.Drawing.Color.Gray;
            this.lab_ignore.Location = new System.Drawing.Point(239, 90);
            this.lab_ignore.Name = "lab_ignore";
            this.lab_ignore.Size = new System.Drawing.Size(32, 17);
            this.lab_ignore.TabIndex = 7;
            this.lab_ignore.Text = "忽略";
            this.lab_ignore.Click += new System.EventHandler(this.skinLabel1_Click);
            // 
            // FirmwareOption
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(295, 149);
            this.CloseBoxSize = new System.Drawing.Size(29, 24);
            this.CloseDownBack = ((System.Drawing.Image)(resources.GetObject("$this.CloseDownBack")));
            this.CloseMouseBack = ((System.Drawing.Image)(resources.GetObject("$this.CloseMouseBack")));
            this.CloseNormlBack = ((System.Drawing.Image)(resources.GetObject("$this.CloseNormlBack")));
            this.Controls.Add(this.skinPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirmwareOption";
            this.RoundStyle = CCWin.SkinClass.RoundStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请选择固件版本";
            this.skinPanel1.ResumeLayout(false);
            this.skinPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private CCWin.SkinControl.SkinRadioButton rb_ArduPoilt;
        private CCWin.SkinControl.SkinRadioButton rb_PX4;
        private CCWin.SkinControl.SkinComboBox combox_list;
        private CCWin.SkinControl.SkinButton btn_set;
        private CCWin.SkinControl.SkinCheckBox cb_tip;
        private CCWin.SkinControl.SkinPanel skinPanel1;
        private CCWin.SkinControl.SkinLabel lab_ignore;
    }
}