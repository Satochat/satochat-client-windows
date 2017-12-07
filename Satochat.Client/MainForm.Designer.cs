namespace Satochat.Client {
    partial class MainForm {
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
            this.friends = new System.Windows.Forms.ListBox();
            this.pingCheckTimer = new System.Windows.Forms.Timer(this.components);
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.toolsMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.viewPublicKeyMenuItem = new System.Windows.Forms.MenuItem();
            this.createNewKeyPairMenuItem = new System.Windows.Forms.MenuItem();
            this.setNicknameMenuItem = new System.Windows.Forms.MenuItem();
            this.enableSoundsMenuItem = new System.Windows.Forms.MenuItem();
            this.debugMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.loginMenuItem = new System.Windows.Forms.MenuItem();
            this.removeLoginMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.deleteKeyPairMenuItem = new System.Windows.Forms.MenuItem();
            this.restartClientMenuItem = new System.Windows.Forms.MenuItem();
            this.showLogMenuItem = new System.Windows.Forms.MenuItem();
            this.changeServerMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.checkForUpdatesMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.aboutAppMenuItem = new System.Windows.Forms.MenuItem();
            this.friendsContextMenu = new System.Windows.Forms.ContextMenu();
            this.converseMenuItem = new System.Windows.Forms.MenuItem();
            this.setFriendNicknameMenuItem = new System.Windows.Forms.MenuItem();
            this.SuspendLayout();
            // 
            // friends
            // 
            this.friends.Dock = System.Windows.Forms.DockStyle.Fill;
            this.friends.FormattingEnabled = true;
            this.friends.IntegralHeight = false;
            this.friends.Location = new System.Drawing.Point(0, 0);
            this.friends.Name = "friends";
            this.friends.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.friends.Size = new System.Drawing.Size(284, 261);
            this.friends.TabIndex = 0;
            this.friends.DoubleClick += new System.EventHandler(this.friends_DoubleClick);
            this.friends.KeyDown += new System.Windows.Forms.KeyEventHandler(this.friends_KeyDown);
            this.friends.MouseUp += new System.Windows.Forms.MouseEventHandler(this.friends_MouseUp);
            // 
            // pingCheckTimer
            // 
            this.pingCheckTimer.Interval = 10000;
            this.pingCheckTimer.Tick += new System.EventHandler(this.pingCheckTimer_Tick);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.toolsMenuItem,
            this.debugMenuItem,
            this.menuItem1});
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.Index = 0;
            this.toolsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem10,
            this.setNicknameMenuItem,
            this.enableSoundsMenuItem});
            this.toolsMenuItem.Text = "Tools";
            this.toolsMenuItem.Popup += new System.EventHandler(this.toolsMenuItem_Popup);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 0;
            this.menuItem10.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.viewPublicKeyMenuItem,
            this.createNewKeyPairMenuItem});
            this.menuItem10.Text = "Keys";
            // 
            // viewPublicKeyMenuItem
            // 
            this.viewPublicKeyMenuItem.Index = 0;
            this.viewPublicKeyMenuItem.Text = "View public key";
            this.viewPublicKeyMenuItem.Click += new System.EventHandler(this.viewPublicKeyMenuItem_Click);
            // 
            // createNewKeyPairMenuItem
            // 
            this.createNewKeyPairMenuItem.Index = 1;
            this.createNewKeyPairMenuItem.Text = "Create new key pair";
            this.createNewKeyPairMenuItem.Click += new System.EventHandler(this.createNewKeyPairMenuItem_Click);
            // 
            // setNicknameMenuItem
            // 
            this.setNicknameMenuItem.Index = 1;
            this.setNicknameMenuItem.Text = "Set nickname";
            this.setNicknameMenuItem.Click += new System.EventHandler(this.setNicknameMenuItem_Click);
            // 
            // enableSoundsMenuItem
            // 
            this.enableSoundsMenuItem.Index = 2;
            this.enableSoundsMenuItem.Text = "Enable sounds";
            this.enableSoundsMenuItem.Click += new System.EventHandler(this.enableSoundsMenuItem_Click);
            // 
            // debugMenuItem
            // 
            this.debugMenuItem.Index = 1;
            this.debugMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem7,
            this.menuItem13,
            this.restartClientMenuItem,
            this.showLogMenuItem,
            this.changeServerMenuItem});
            this.debugMenuItem.Text = "Debug";
            this.debugMenuItem.Popup += new System.EventHandler(this.debugMenuItem_Popup);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 0;
            this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.loginMenuItem,
            this.removeLoginMenuItem});
            this.menuItem7.Text = "Account";
            // 
            // loginMenuItem
            // 
            this.loginMenuItem.Index = 0;
            this.loginMenuItem.Text = "Log in";
            this.loginMenuItem.Click += new System.EventHandler(this.loginMenuItem_Click);
            // 
            // removeLoginMenuItem
            // 
            this.removeLoginMenuItem.Index = 1;
            this.removeLoginMenuItem.Text = "Remove login";
            this.removeLoginMenuItem.Click += new System.EventHandler(this.removeLoginMenuItem_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 1;
            this.menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.deleteKeyPairMenuItem});
            this.menuItem13.Text = "Keys";
            // 
            // deleteKeyPairMenuItem
            // 
            this.deleteKeyPairMenuItem.Index = 0;
            this.deleteKeyPairMenuItem.Text = "Delete key pair";
            this.deleteKeyPairMenuItem.Click += new System.EventHandler(this.deleteKeyPairMenuItem_Click);
            // 
            // restartClientMenuItem
            // 
            this.restartClientMenuItem.Index = 2;
            this.restartClientMenuItem.Text = "Restart client";
            this.restartClientMenuItem.Click += new System.EventHandler(this.restartClientMenuItem_Click);
            // 
            // showLogMenuItem
            // 
            this.showLogMenuItem.Index = 3;
            this.showLogMenuItem.Text = "Show log";
            this.showLogMenuItem.Click += new System.EventHandler(this.showLogMenuItem_Click);
            // 
            // changeServerMenuItem
            // 
            this.changeServerMenuItem.Index = 4;
            this.changeServerMenuItem.Text = "Change server";
            this.changeServerMenuItem.Click += new System.EventHandler(this.changeServerMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 2;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.checkForUpdatesMenuItem,
            this.menuItem15,
            this.aboutAppMenuItem});
            this.menuItem1.Text = "Help";
            // 
            // checkForUpdatesMenuItem
            // 
            this.checkForUpdatesMenuItem.Index = 0;
            this.checkForUpdatesMenuItem.Text = "Check for updates";
            this.checkForUpdatesMenuItem.Click += new System.EventHandler(this.checkForUpdatesMenuItem_Click);
            // 
            // menuItem15
            // 
            this.menuItem15.Index = 1;
            this.menuItem15.Text = "-";
            // 
            // aboutAppMenuItem
            // 
            this.aboutAppMenuItem.Index = 2;
            this.aboutAppMenuItem.Text = "About Satochat";
            this.aboutAppMenuItem.Click += new System.EventHandler(this.aboutAppMenuItem_Click);
            // 
            // friendsContextMenu
            // 
            this.friendsContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.converseMenuItem,
            this.setFriendNicknameMenuItem});
            this.friendsContextMenu.Popup += new System.EventHandler(this.friendsContextMenu_Popup);
            // 
            // converseMenuItem
            // 
            this.converseMenuItem.Index = 0;
            this.converseMenuItem.Text = "Converse";
            this.converseMenuItem.Click += new System.EventHandler(this.converseMenuItem_Click);
            // 
            // setFriendNicknameMenuItem
            // 
            this.setFriendNicknameMenuItem.Index = 1;
            this.setFriendNicknameMenuItem.Text = "Set nickname";
            this.setFriendNicknameMenuItem.Click += new System.EventHandler(this.setFriendNicknameMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.friends);
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox friends;
        private System.Windows.Forms.Timer pingCheckTimer;
        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem toolsMenuItem;
        private System.Windows.Forms.MenuItem debugMenuItem;
        private System.Windows.Forms.MenuItem showLogMenuItem;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem loginMenuItem;
        private System.Windows.Forms.MenuItem removeLoginMenuItem;
        private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem deleteKeyPairMenuItem;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem viewPublicKeyMenuItem;
        private System.Windows.Forms.MenuItem createNewKeyPairMenuItem;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem checkForUpdatesMenuItem;
        private System.Windows.Forms.MenuItem aboutAppMenuItem;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.MenuItem setNicknameMenuItem;
        private System.Windows.Forms.MenuItem restartClientMenuItem;
        private System.Windows.Forms.MenuItem enableSoundsMenuItem;
        private System.Windows.Forms.ContextMenu friendsContextMenu;
        private System.Windows.Forms.MenuItem setFriendNicknameMenuItem;
        private System.Windows.Forms.MenuItem converseMenuItem;
        private System.Windows.Forms.MenuItem changeServerMenuItem;
    }
}