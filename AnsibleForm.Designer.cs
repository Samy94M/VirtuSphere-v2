namespace VirtuSphere
{
    partial class AnsibleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnsibleForm));
            this.listFiles = new System.Windows.Forms.ListView();
            this.txtAnsible = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.labelMissionName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.comboPlaybooks = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listFiles
            // 
            this.listFiles.Location = new System.Drawing.Point(689, 34);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(176, 123);
            this.listFiles.TabIndex = 0;
            this.listFiles.UseCompatibleStateImageBehavior = false;
            this.listFiles.View = System.Windows.Forms.View.SmallIcon;
            this.listFiles.SelectedIndexChanged += new System.EventHandler(this.loadConfig);
            // 
            // txtAnsible
            // 
            this.txtAnsible.Enabled = false;
            this.txtAnsible.Location = new System.Drawing.Point(15, 34);
            this.txtAnsible.Multiline = true;
            this.txtAnsible.Name = "txtAnsible";
            this.txtAnsible.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAnsible.Size = new System.Drawing.Size(668, 500);
            this.txtAnsible.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboPlaybooks);
            this.groupBox1.Controls.Add(this.btnSave);
            this.groupBox1.Controls.Add(this.labelMissionName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.listFiles);
            this.groupBox1.Controls.Add(this.txtAnsible);
            this.groupBox1.Location = new System.Drawing.Point(25, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(876, 555);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Generierten Ansible Playbooks";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(737, 410);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(84, 32);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Speichern";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // labelMissionName
            // 
            this.labelMissionName.AutoSize = true;
            this.labelMissionName.Location = new System.Drawing.Point(774, 207);
            this.labelMissionName.Name = "labelMissionName";
            this.labelMissionName.Size = new System.Drawing.Size(24, 13);
            this.labelMissionName.TabIndex = 15;
            this.labelMissionName.Text = "leer";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(689, 207);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Missionsname: ";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(737, 297);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(84, 23);
            this.button4.TabIndex = 13;
            this.button4.Text = "Reload";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(737, 326);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(84, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "Explorer";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(726, 163);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(119, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Bearbeitungsmodus";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(737, 502);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 32);
            this.button2.TabIndex = 4;
            this.button2.Text = "Ausführen";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboPlaybooks
            // 
            this.comboPlaybooks.FormattingEnabled = true;
            this.comboPlaybooks.Location = new System.Drawing.Point(724, 475);
            this.comboPlaybooks.Name = "comboPlaybooks";
            this.comboPlaybooks.Size = new System.Drawing.Size(121, 21);
            this.comboPlaybooks.TabIndex = 17;
            // 
            // AnsibleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 585);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnsibleForm";
            this.Text = "AnsibleForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        internal System.Windows.Forms.ListView listFiles;
        internal System.Windows.Forms.TextBox txtAnsible;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label labelMissionName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        internal System.Windows.Forms.ComboBox comboPlaybooks;
    }
}