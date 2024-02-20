namespace VirtuSphere
{
    partial class LogForm
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
            this.lsLog = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // lsLog
            // 
            this.lsLog.HideSelection = false;
            this.lsLog.Location = new System.Drawing.Point(13, 13);
            this.lsLog.Name = "lsLog";
            this.lsLog.Size = new System.Drawing.Size(584, 425);
            this.lsLog.TabIndex = 0;
            this.lsLog.UseCompatibleStateImageBehavior = false;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 450);
            this.Controls.Add(this.lsLog);
            this.Name = "LogForm";
            this.Text = "Logs";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lsLog;
    }
}