namespace Satochat.Client {
    partial class ConversationForm {
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
            this.components = new System.ComponentModel.Container();
            this.history = new System.Windows.Forms.WebBrowser();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.editorTab = new System.Windows.Forms.TabPage();
            this.input = new System.Windows.Forms.TextBox();
            this.previewTab = new System.Windows.Forms.TabPage();
            this.preview = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.changeFontMenuItem = new System.Windows.Forms.MenuItem();
            this.emojiMappingMenuItem = new System.Windows.Forms.MenuItem();
            this.debugMenuItem = new System.Windows.Forms.MenuItem();
            this.addFakeMessagesMenuItem = new System.Windows.Forms.MenuItem();
            this.addTextFormattingDemoMenuItem = new System.Windows.Forms.MenuItem();
            this.embeddedWebBrowserMenuItem = new System.Windows.Forms.MenuItem();
            this.browserInfoMenuItem = new System.Windows.Forms.MenuItem();
            this.copyHistoryHtmlMenuItem = new System.Windows.Forms.MenuItem();
            this.copyPreviewHtmlMenuItem = new System.Windows.Forms.MenuItem();
            this.enableStatsMenuItem = new System.Windows.Forms.MenuItem();
            this.fakeSendingMessageMenuItem = new System.Windows.Forms.MenuItem();
            this.reloadCssMenuItem = new System.Windows.Forms.MenuItem();
            this.spamMessagesMenuItem = new System.Windows.Forms.MenuItem();
            this.statsTimer = new System.Windows.Forms.Timer(this.components);
            this.tabControl1.SuspendLayout();
            this.editorTab.SuspendLayout();
            this.previewTab.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // history
            // 
            this.history.AllowWebBrowserDrop = false;
            this.history.Dock = System.Windows.Forms.DockStyle.Fill;
            this.history.IsWebBrowserContextMenuEnabled = false;
            this.history.Location = new System.Drawing.Point(0, 0);
            this.history.Margin = new System.Windows.Forms.Padding(0);
            this.history.MinimumSize = new System.Drawing.Size(20, 20);
            this.history.Name = "history";
            this.history.ScriptErrorsSuppressed = true;
            this.history.Size = new System.Drawing.Size(501, 494);
            this.history.TabIndex = 0;
            this.history.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.history_Navigating);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Controls.Add(this.editorTab);
            this.tabControl1.Controls.Add(this.previewTab);
            this.tabControl1.Location = new System.Drawing.Point(0, 494);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(496, 51);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // editorTab
            // 
            this.editorTab.Controls.Add(this.input);
            this.editorTab.Location = new System.Drawing.Point(4, 4);
            this.editorTab.Name = "editorTab";
            this.editorTab.Size = new System.Drawing.Size(488, 25);
            this.editorTab.TabIndex = 0;
            this.editorTab.Text = "Editor";
            this.editorTab.UseVisualStyleBackColor = true;
            // 
            // input
            // 
            this.input.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.input.Dock = System.Windows.Forms.DockStyle.Fill;
            this.input.HideSelection = false;
            this.input.Location = new System.Drawing.Point(0, 0);
            this.input.MaxLength = 10000;
            this.input.Multiline = true;
            this.input.Name = "input";
            this.input.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.input.Size = new System.Drawing.Size(488, 25);
            this.input.TabIndex = 0;
            this.input.TextChanged += new System.EventHandler(this.input_TextChanged);
            this.input.KeyDown += new System.Windows.Forms.KeyEventHandler(this.input_KeyDown);
            // 
            // previewTab
            // 
            this.previewTab.Controls.Add(this.preview);
            this.previewTab.Location = new System.Drawing.Point(4, 4);
            this.previewTab.Name = "previewTab";
            this.previewTab.Size = new System.Drawing.Size(488, 25);
            this.previewTab.TabIndex = 1;
            this.previewTab.Text = "Preview";
            this.previewTab.UseVisualStyleBackColor = true;
            // 
            // preview
            // 
            this.preview.AllowWebBrowserDrop = false;
            this.preview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preview.IsWebBrowserContextMenuEnabled = false;
            this.preview.Location = new System.Drawing.Point(0, 0);
            this.preview.MinimumSize = new System.Drawing.Size(20, 20);
            this.preview.Name = "preview";
            this.preview.ScriptErrorsSuppressed = true;
            this.preview.Size = new System.Drawing.Size(488, 25);
            this.preview.TabIndex = 0;
            this.preview.TabStop = false;
            this.preview.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.preview_Navigating);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.history, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(501, 545);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.debugMenuItem});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.changeFontMenuItem,
            this.emojiMappingMenuItem});
            this.menuItem1.Text = "Tools";
            // 
            // changeFontMenuItem
            // 
            this.changeFontMenuItem.Index = 0;
            this.changeFontMenuItem.Text = "Change font";
            this.changeFontMenuItem.Click += new System.EventHandler(this.changeFontMenuItem_Click);
            // 
            // emojiMappingMenuItem
            // 
            this.emojiMappingMenuItem.Index = 1;
            this.emojiMappingMenuItem.Text = "Emoji mapping";
            this.emojiMappingMenuItem.Click += new System.EventHandler(this.emojiMappingMenuItem_Click);
            // 
            // debugMenuItem
            // 
            this.debugMenuItem.Index = 1;
            this.debugMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.addFakeMessagesMenuItem,
            this.addTextFormattingDemoMenuItem,
            this.embeddedWebBrowserMenuItem,
            this.enableStatsMenuItem,
            this.fakeSendingMessageMenuItem,
            this.reloadCssMenuItem,
            this.spamMessagesMenuItem});
            this.debugMenuItem.Text = "Debug";
            // 
            // addFakeMessagesMenuItem
            // 
            this.addFakeMessagesMenuItem.Index = 0;
            this.addFakeMessagesMenuItem.Text = "Add fake messages";
            this.addFakeMessagesMenuItem.Click += new System.EventHandler(this.addFakeMessagesMenuItem_Click);
            // 
            // addTextFormattingDemoMenuItem
            // 
            this.addTextFormattingDemoMenuItem.Index = 1;
            this.addTextFormattingDemoMenuItem.Text = "Add text formatting demo";
            this.addTextFormattingDemoMenuItem.Click += new System.EventHandler(this.addTextFormattingDemoMenuItem_Click);
            // 
            // embeddedWebBrowserMenuItem
            // 
            this.embeddedWebBrowserMenuItem.Index = 2;
            this.embeddedWebBrowserMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.browserInfoMenuItem,
            this.copyHistoryHtmlMenuItem,
            this.copyPreviewHtmlMenuItem});
            this.embeddedWebBrowserMenuItem.Text = "Embedded web browser";
            // 
            // browserInfoMenuItem
            // 
            this.browserInfoMenuItem.Index = 0;
            this.browserInfoMenuItem.Text = "Browser info";
            this.browserInfoMenuItem.Click += new System.EventHandler(this.browserInfoMenuItem_Click);
            // 
            // copyHistoryHtmlMenuItem
            // 
            this.copyHistoryHtmlMenuItem.Index = 1;
            this.copyHistoryHtmlMenuItem.Text = "Copy history HTML";
            this.copyHistoryHtmlMenuItem.Click += new System.EventHandler(this.copyHistoryHtmlMenuItem_Click);
            // 
            // copyPreviewHtmlMenuItem
            // 
            this.copyPreviewHtmlMenuItem.Index = 2;
            this.copyPreviewHtmlMenuItem.Text = "Copy preview HTML";
            this.copyPreviewHtmlMenuItem.Click += new System.EventHandler(this.copyPreviewHtmlMenuItem_Click);
            // 
            // enableStatsMenuItem
            // 
            this.enableStatsMenuItem.Index = 3;
            this.enableStatsMenuItem.Text = "Enable stats";
            this.enableStatsMenuItem.Click += new System.EventHandler(this.enableStatsMenuItem_Click);
            // 
            // fakeSendingMessageMenuItem
            // 
            this.fakeSendingMessageMenuItem.Index = 4;
            this.fakeSendingMessageMenuItem.Text = "Fake sending message";
            this.fakeSendingMessageMenuItem.Click += new System.EventHandler(this.fakeSendingMessageMenuItem_Click);
            // 
            // reloadCssMenuItem
            // 
            this.reloadCssMenuItem.Index = 5;
            this.reloadCssMenuItem.Shortcut = System.Windows.Forms.Shortcut.CtrlF1;
            this.reloadCssMenuItem.Text = "Reload CSS";
            this.reloadCssMenuItem.Click += new System.EventHandler(this.reloadCssMenuItem_Click);
            // 
            // spamMessagesMenuItem
            // 
            this.spamMessagesMenuItem.Index = 6;
            this.spamMessagesMenuItem.Text = "Spam messages";
            this.spamMessagesMenuItem.Click += new System.EventHandler(this.spamMessagesMenuItem_Click);
            // 
            // statsTimer
            // 
            this.statsTimer.Interval = 500;
            this.statsTimer.Tick += new System.EventHandler(this.statsTimer_Tick);
            // 
            // ConversationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 545);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Menu = this.mainMenu1;
            this.Name = "ConversationForm";
            this.Text = "ConversationForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConversationForm_FormClosed);
            this.Load += new System.EventHandler(this.ConversationForm_Load);
            this.Shown += new System.EventHandler(this.ConversationForm_Shown);
            this.ResizeEnd += new System.EventHandler(this.ConversationForm_ResizeEnd);
            this.SizeChanged += new System.EventHandler(this.ConversationForm_SizeChanged);
            this.tabControl1.ResumeLayout(false);
            this.editorTab.ResumeLayout(false);
            this.editorTab.PerformLayout();
            this.previewTab.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.WebBrowser history;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage editorTab;
        private System.Windows.Forms.TextBox input;
        private System.Windows.Forms.TabPage previewTab;
        private System.Windows.Forms.WebBrowser preview;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem debugMenuItem;
        private System.Windows.Forms.MenuItem addFakeMessagesMenuItem;
        private System.Windows.Forms.MenuItem addTextFormattingDemoMenuItem;
        private System.Windows.Forms.MenuItem embeddedWebBrowserMenuItem;
        private System.Windows.Forms.MenuItem browserInfoMenuItem;
        private System.Windows.Forms.MenuItem copyHistoryHtmlMenuItem;
        private System.Windows.Forms.MenuItem copyPreviewHtmlMenuItem;
        private System.Windows.Forms.MenuItem fakeSendingMessageMenuItem;
        private System.Windows.Forms.MenuItem reloadCssMenuItem;
        private System.Windows.Forms.MenuItem spamMessagesMenuItem;
        private System.Windows.Forms.MenuItem changeFontMenuItem;
        private System.Windows.Forms.MenuItem emojiMappingMenuItem;
        private System.Windows.Forms.Timer statsTimer;
        private System.Windows.Forms.MenuItem enableStatsMenuItem;
    }
}