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
            this.listFiles = new System.Windows.Forms.ListView();
            this.txtAnsible = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_loginname = new System.Windows.Forms.TextBox();
            this.txt_datacenter = new System.Windows.Forms.TextBox();
            this.txt_datastore = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.combo_action = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listFiles
            // 
            this.listFiles.HideSelection = false;
            this.listFiles.Location = new System.Drawing.Point(655, 34);
            this.listFiles.Name = "listFiles";
            this.listFiles.Size = new System.Drawing.Size(121, 123);
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
            this.txtAnsible.Size = new System.Drawing.Size(634, 500);
            this.txtAnsible.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txt_loginname);
            this.groupBox1.Controls.Add(this.txt_datacenter);
            this.groupBox1.Controls.Add(this.txt_datastore);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.combo_action);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.listFiles);
            this.groupBox1.Controls.Add(this.txtAnsible);
            this.groupBox1.Location = new System.Drawing.Point(25, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(788, 572);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Generierten Ansible Playbooks";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(655, 291);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Loginname";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(655, 243);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Datacenter";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(655, 194);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Datastore";
            // 
            // txt_loginname
            // 
            this.txt_loginname.Enabled = false;
            this.txt_loginname.Location = new System.Drawing.Point(655, 307);
            this.txt_loginname.Name = "txt_loginname";
            this.txt_loginname.Size = new System.Drawing.Size(121, 20);
            this.txt_loginname.TabIndex = 7;
            // 
            // txt_datacenter
            // 
            this.txt_datacenter.Enabled = false;
            this.txt_datacenter.Location = new System.Drawing.Point(655, 259);
            this.txt_datacenter.Name = "txt_datacenter";
            this.txt_datacenter.Size = new System.Drawing.Size(121, 20);
            this.txt_datacenter.TabIndex = 6;
            // 
            // txt_datastore
            // 
            this.txt_datastore.Enabled = false;
            this.txt_datastore.Location = new System.Drawing.Point(655, 210);
            this.txt_datastore.Name = "txt_datastore";
            this.txt_datastore.Size = new System.Drawing.Size(121, 20);
            this.txt_datastore.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(671, 502);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(84, 32);
            this.button2.TabIndex = 4;
            this.button2.Text = "Ausführen";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // combo_action
            // 
            this.combo_action.FormattingEnabled = true;
            this.combo_action.Location = new System.Drawing.Point(655, 437);
            this.combo_action.Name = "combo_action";
            this.combo_action.Size = new System.Drawing.Size(121, 21);
            this.combo_action.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(671, 464);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(84, 32);
            this.button1.TabIndex = 2;
            this.button1.Text = "Erstellen";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.generateConfigs);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(658, 360);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(119, 17);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "Bearbeitungsmodus";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // AnsibleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 617);
            this.Controls.Add(this.groupBox1);
            this.Name = "AnsibleForm";
            this.Text = "AnsibleForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox combo_action;
        private System.Windows.Forms.TextBox txt_loginname;
        private System.Windows.Forms.TextBox txt_datacenter;
        private System.Windows.Forms.TextBox txt_datastore;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        internal System.Windows.Forms.ListView listFiles;
        internal System.Windows.Forms.TextBox txtAnsible;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}