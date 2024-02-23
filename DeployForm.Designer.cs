using System.Collections.Generic;
using System.Windows.Forms;

namespace VirtuSphere
{
    partial class DeployForm
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
            this.DeployListView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // DeployListView
            // 
            this.DeployListView.BackColor = System.Drawing.SystemColors.InfoText;
            this.DeployListView.ForeColor = System.Drawing.Color.Lime;
            this.DeployListView.HideSelection = false;
            this.DeployListView.Location = new System.Drawing.Point(12, 12);
            this.DeployListView.Name = "DeployListView";
            this.DeployListView.Size = new System.Drawing.Size(776, 426);
            this.DeployListView.TabIndex = 0;
            this.DeployListView.UseCompatibleStateImageBehavior = false;
            this.DeployListView.View = System.Windows.Forms.View.List;
            // 
            // Deploy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.DeployListView);
            this.Name = "Deploy";
            this.Text = "Deploy";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView DeployListView;
    }
}