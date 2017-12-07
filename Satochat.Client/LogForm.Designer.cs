namespace Satochat.Client {
    partial class LogForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.logLines = new System.Windows.Forms.ListBox();
            this.logContextMenu = new System.Windows.Forms.ContextMenu();
            this.copyAllMenuItem = new System.Windows.Forms.MenuItem();
            this.copySelectionMenuItem = new System.Windows.Forms.MenuItem();
            this.scrollAutomaticallyMenuItem = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // logLines
            // 
            this.logLines.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logLines.FormattingEnabled = true;
            this.logLines.IntegralHeight = false;
            this.logLines.Location = new System.Drawing.Point(0, 0);
            this.logLines.Name = "logLines";
            this.logLines.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.logLines.Size = new System.Drawing.Size(508, 261);
            this.logLines.TabIndex = 0;
            this.logLines.MouseUp += new System.Windows.Forms.MouseEventHandler(this.logLines_MouseUp);
            // 
            // logContextMenu
            // 
            this.logContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.copyAllMenuItem,
            this.copySelectionMenuItem,
            this.scrollAutomaticallyMenuItem});
            // 
            // copyAllMenuItem
            // 
            this.copyAllMenuItem.Index = 0;
            this.copyAllMenuItem.Text = "Copy all";
            this.copyAllMenuItem.Click += new System.EventHandler(this.copyAllMenuItem_Click);
            // 
            // copySelectionMenuItem
            // 
            this.copySelectionMenuItem.Index = 1;
            this.copySelectionMenuItem.Text = "Copy selection";
            this.copySelectionMenuItem.Click += new System.EventHandler(this.copySelectionMenuItem_Click);
            // 
            // scrollAutomaticallyMenuItem
            // 
            this.scrollAutomaticallyMenuItem.Checked = true;
            this.scrollAutomaticallyMenuItem.Index = 2;
            this.scrollAutomaticallyMenuItem.Text = "Scroll automatically";
            this.scrollAutomaticallyMenuItem.Click += new System.EventHandler(this.scrollAutomaticallyMenuItem_Click);
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 261);
            this.Controls.Add(this.logLines);
            this.Name = "LogForm";
            this.Text = "LogForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox logLines;
        private System.Windows.Forms.ContextMenu logContextMenu;
        private System.Windows.Forms.MenuItem copyAllMenuItem;
        private System.Windows.Forms.MenuItem copySelectionMenuItem;
        private System.Windows.Forms.MenuItem scrollAutomaticallyMenuItem;
    }
}