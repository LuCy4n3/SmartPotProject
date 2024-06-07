namespace ITDaysDemo
{
    partial class Demo
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblVoltage = new System.Windows.Forms.Label();
            this.lblCurrent = new System.Windows.Forms.Label();
            this.lblVoltageAndCurrentReceivedTime = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOnOff = new System.Windows.Forms.Button();
            this.btnDimming = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLightStatus = new System.Windows.Forms.Label();
            this.lblDimmingLevel = new System.Windows.Forms.Label();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.lblHumidity = new System.Windows.Forms.Label();
            this.lblBatteryVoltage = new System.Windows.Forms.Label();
            this.lblReceivedTime = new System.Windows.Forms.Label();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblCOValue = new System.Windows.Forms.Label();
            this.lblCOReceivedTime = new System.Windows.Forms.Label();
            this.richTextBoxDebug = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel4, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.71472F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 68.28528F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1915, 1526);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.lblVoltage);
            this.flowLayoutPanel2.Controls.Add(this.lblCurrent);
            this.flowLayoutPanel2.Controls.Add(this.lblVoltageAndCurrentReceivedTime);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(962, 5);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(948, 473);
            this.flowLayoutPanel2.TabIndex = 5;
            // 
            // lblVoltage
            // 
            this.lblVoltage.AutoSize = true;
            this.lblVoltage.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVoltage.ForeColor = System.Drawing.Color.Red;
            this.lblVoltage.Location = new System.Drawing.Point(5, 0);
            this.lblVoltage.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblVoltage.Name = "lblVoltage";
            this.lblVoltage.Padding = new System.Windows.Forms.Padding(16);
            this.lblVoltage.Size = new System.Drawing.Size(201, 83);
            this.lblVoltage.TabIndex = 0;
            this.lblVoltage.Text = "Voltage:";
            // 
            // lblCurrent
            // 
            this.lblCurrent.AutoSize = true;
            this.lblCurrent.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCurrent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblCurrent.Location = new System.Drawing.Point(5, 83);
            this.lblCurrent.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblCurrent.Name = "lblCurrent";
            this.lblCurrent.Padding = new System.Windows.Forms.Padding(16);
            this.lblCurrent.Size = new System.Drawing.Size(200, 83);
            this.lblCurrent.TabIndex = 1;
            this.lblCurrent.Text = "Current:";
            // 
            // lblVoltageAndCurrentReceivedTime
            // 
            this.lblVoltageAndCurrentReceivedTime.AutoSize = true;
            this.lblVoltageAndCurrentReceivedTime.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblVoltageAndCurrentReceivedTime.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblVoltageAndCurrentReceivedTime.Location = new System.Drawing.Point(5, 166);
            this.lblVoltageAndCurrentReceivedTime.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblVoltageAndCurrentReceivedTime.Name = "lblVoltageAndCurrentReceivedTime";
            this.lblVoltageAndCurrentReceivedTime.Padding = new System.Windows.Forms.Padding(16);
            this.lblVoltageAndCurrentReceivedTime.Size = new System.Drawing.Size(324, 83);
            this.lblVoltageAndCurrentReceivedTime.TabIndex = 2;
            this.lblVoltageAndCurrentReceivedTime.Text = "Received Time:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnOnOff);
            this.flowLayoutPanel1.Controls.Add(this.btnDimming);
            this.flowLayoutPanel1.Controls.Add(this.tableLayoutPanel2);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(5, 5);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(947, 473);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // btnOnOff
            // 
            this.btnOnOff.BackgroundImage = global::ITDaysDemo.Properties.Resources.Bottom___lumina_normala1;
            this.btnOnOff.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOnOff.Location = new System.Drawing.Point(3, 3);
            this.btnOnOff.Name = "btnOnOff";
            this.btnOnOff.Size = new System.Drawing.Size(440, 264);
            this.btnOnOff.TabIndex = 0;
            this.btnOnOff.Text = "ON/OFF";
            this.btnOnOff.UseVisualStyleBackColor = true;
            this.btnOnOff.Click += new System.EventHandler(this.btnOnOff_Click);
            // 
            // btnDimming
            // 
            this.btnDimming.BackgroundImage = global::ITDaysDemo.Properties.Resources.Centered___dimming;
            this.btnDimming.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnDimming.Image = global::ITDaysDemo.Properties.Resources.Centered___dimming;
            this.btnDimming.Location = new System.Drawing.Point(451, 5);
            this.btnDimming.Margin = new System.Windows.Forms.Padding(5);
            this.btnDimming.Name = "btnDimming";
            this.btnDimming.Size = new System.Drawing.Size(403, 264);
            this.btnDimming.TabIndex = 1;
            this.btnDimming.Text = "Dimming";
            this.btnDimming.UseVisualStyleBackColor = true;
            this.btnDimming.Click += new System.EventHandler(this.btnDimming_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.lblLightStatus, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblDimmingLevel, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 277);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(900, 179);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // lblLightStatus
            // 
            this.lblLightStatus.AutoSize = true;
            this.lblLightStatus.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblLightStatus.ForeColor = System.Drawing.Color.Blue;
            this.lblLightStatus.Location = new System.Drawing.Point(3, 0);
            this.lblLightStatus.Name = "lblLightStatus";
            this.lblLightStatus.Size = new System.Drawing.Size(147, 59);
            this.lblLightStatus.TabIndex = 1;
            this.lblLightStatus.Text = "label2";
            // 
            // lblDimmingLevel
            // 
            this.lblDimmingLevel.AutoSize = true;
            this.lblDimmingLevel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDimmingLevel.ForeColor = System.Drawing.Color.Blue;
            this.lblDimmingLevel.Location = new System.Drawing.Point(453, 0);
            this.lblDimmingLevel.Name = "lblDimmingLevel";
            this.lblDimmingLevel.Size = new System.Drawing.Size(48, 59);
            this.lblDimmingLevel.TabIndex = 0;
            this.lblDimmingLevel.Text = "a";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.lblTemperature);
            this.flowLayoutPanel3.Controls.Add(this.lblHumidity);
            this.flowLayoutPanel3.Controls.Add(this.lblBatteryVoltage);
            this.flowLayoutPanel3.Controls.Add(this.lblReceivedTime);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(5, 488);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(947, 1033);
            this.flowLayoutPanel3.TabIndex = 4;
            // 
            // lblTemperature
            // 
            this.lblTemperature.AutoSize = true;
            this.lblTemperature.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTemperature.ForeColor = System.Drawing.Color.Red;
            this.lblTemperature.Location = new System.Drawing.Point(5, 32);
            this.lblTemperature.Margin = new System.Windows.Forms.Padding(5, 32, 5, 32);
            this.lblTemperature.Name = "lblTemperature";
            this.lblTemperature.Size = new System.Drawing.Size(545, 106);
            this.lblTemperature.TabIndex = 0;
            this.lblTemperature.Text = "Temperature:";
            // 
            // lblHumidity
            // 
            this.lblHumidity.AutoSize = true;
            this.lblHumidity.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblHumidity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblHumidity.Location = new System.Drawing.Point(5, 186);
            this.lblHumidity.Margin = new System.Windows.Forms.Padding(5, 16, 5, 32);
            this.lblHumidity.Name = "lblHumidity";
            this.lblHumidity.Size = new System.Drawing.Size(419, 106);
            this.lblHumidity.TabIndex = 1;
            this.lblHumidity.Text = "Humidity:";
            // 
            // lblBatteryVoltage
            // 
            this.lblBatteryVoltage.AutoSize = true;
            this.lblBatteryVoltage.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblBatteryVoltage.ForeColor = System.Drawing.Color.Green;
            this.lblBatteryVoltage.Location = new System.Drawing.Point(5, 340);
            this.lblBatteryVoltage.Margin = new System.Windows.Forms.Padding(5, 16, 5, 32);
            this.lblBatteryVoltage.Name = "lblBatteryVoltage";
            this.lblBatteryVoltage.Size = new System.Drawing.Size(653, 106);
            this.lblBatteryVoltage.TabIndex = 2;
            this.lblBatteryVoltage.Text = "Battery Voltage:";
            // 
            // lblReceivedTime
            // 
            this.lblReceivedTime.AutoSize = true;
            this.lblReceivedTime.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblReceivedTime.Location = new System.Drawing.Point(3, 478);
            this.lblReceivedTime.Name = "lblReceivedTime";
            this.lblReceivedTime.Size = new System.Drawing.Size(490, 86);
            this.lblReceivedTime.TabIndex = 3;
            this.lblReceivedTime.Text = "Received Time:";
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.lblCOValue);
            this.flowLayoutPanel4.Controls.Add(this.lblCOReceivedTime);
            this.flowLayoutPanel4.Controls.Add(this.richTextBoxDebug);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(960, 486);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(952, 1037);
            this.flowLayoutPanel4.TabIndex = 6;
            // 
            // lblCOValue
            // 
            this.lblCOValue.AutoSize = true;
            this.lblCOValue.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCOValue.ForeColor = System.Drawing.Color.Red;
            this.lblCOValue.Location = new System.Drawing.Point(5, 32);
            this.lblCOValue.Margin = new System.Windows.Forms.Padding(5, 32, 5, 32);
            this.lblCOValue.Name = "lblCOValue";
            this.lblCOValue.Size = new System.Drawing.Size(594, 86);
            this.lblCOValue.TabIndex = 1;
            this.lblCOValue.Text = "Carbon Monoxide:";
            // 
            // lblCOReceivedTime
            // 
            this.lblCOReceivedTime.AutoSize = true;
            this.lblCOReceivedTime.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblCOReceivedTime.Location = new System.Drawing.Point(3, 150);
            this.lblCOReceivedTime.Name = "lblCOReceivedTime";
            this.lblCOReceivedTime.Size = new System.Drawing.Size(490, 86);
            this.lblCOReceivedTime.TabIndex = 4;
            this.lblCOReceivedTime.Text = "Received Time:";
            // 
            // richTextBoxDebug
            // 
            this.richTextBoxDebug.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxDebug.Location = new System.Drawing.Point(3, 239);
            this.richTextBoxDebug.Name = "richTextBoxDebug";
            this.richTextBoxDebug.Size = new System.Drawing.Size(598, 386);
            this.richTextBoxDebug.TabIndex = 5;
            this.richTextBoxDebug.Text = "";
            // 
            // Demo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1915, 1526);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Demo";
            this.Text = "Demo";
            this.Load += new System.EventHandler(this.Demo_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Button btnOnOff;
        private Button btnDimming;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel3;
        private FlowLayoutPanel flowLayoutPanel2;
        public Label lblDimmingLevel;
        public Label lblLightStatus;
        public Label lblTemperature;
        public Label lblHumidity;
        public Label lblBatteryVoltage;
        public Label lblReceivedTime;
        private TableLayoutPanel tableLayoutPanel2;
        public Label lblVoltage;
        public Label lblCurrent;
        public Label lblVoltageAndCurrentReceivedTime;
        private FlowLayoutPanel flowLayoutPanel4;
        public Label lblCOValue;
        public Label lblCOReceivedTime;
        public RichTextBox richTextBoxDebug;
    }
}