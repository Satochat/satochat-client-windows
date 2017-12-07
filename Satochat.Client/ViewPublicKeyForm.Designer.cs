namespace Satochat.Client {
    partial class ViewPublicKeyForm {
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fingerprint = new System.Windows.Forms.TextBox();
            this.key = new System.Windows.Forms.TextBox();
            this.copyKey = new System.Windows.Forms.Button();
            this.copyFingerprint = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fingerprint:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Key:";
            // 
            // fingerprint
            // 
            this.fingerprint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fingerprint.Location = new System.Drawing.Point(12, 25);
            this.fingerprint.Name = "fingerprint";
            this.fingerprint.ReadOnly = true;
            this.fingerprint.Size = new System.Drawing.Size(529, 20);
            this.fingerprint.TabIndex = 2;
            // 
            // key
            // 
            this.key.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.key.Location = new System.Drawing.Point(12, 64);
            this.key.Multiline = true;
            this.key.Name = "key";
            this.key.ReadOnly = true;
            this.key.Size = new System.Drawing.Size(529, 207);
            this.key.TabIndex = 3;
            // 
            // copyKey
            // 
            this.copyKey.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyKey.Location = new System.Drawing.Point(126, 277);
            this.copyKey.Name = "copyKey";
            this.copyKey.Size = new System.Drawing.Size(75, 23);
            this.copyKey.TabIndex = 4;
            this.copyKey.Text = "Copy key";
            this.copyKey.UseVisualStyleBackColor = true;
            this.copyKey.Click += new System.EventHandler(this.copyKey_Click);
            // 
            // copyFingerprint
            // 
            this.copyFingerprint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.copyFingerprint.Location = new System.Drawing.Point(12, 277);
            this.copyFingerprint.Name = "copyFingerprint";
            this.copyFingerprint.Size = new System.Drawing.Size(108, 23);
            this.copyFingerprint.TabIndex = 5;
            this.copyFingerprint.Text = "Copy fingerprint";
            this.copyFingerprint.UseVisualStyleBackColor = true;
            this.copyFingerprint.Click += new System.EventHandler(this.copyFingerprint_Click);
            // 
            // ok
            // 
            this.ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ok.Location = new System.Drawing.Point(466, 277);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 6;
            this.ok.Text = "OK";
            this.ok.UseVisualStyleBackColor = true;
            // 
            // ViewPublicKeyForm
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ok;
            this.ClientSize = new System.Drawing.Size(553, 312);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.copyFingerprint);
            this.Controls.Add(this.copyKey);
            this.Controls.Add(this.key);
            this.Controls.Add(this.fingerprint);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ViewPublicKeyForm";
            this.Text = "ViewPublicKeyForm";
            this.Shown += new System.EventHandler(this.ViewPublicKeyForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fingerprint;
        private System.Windows.Forms.TextBox key;
        private System.Windows.Forms.Button copyKey;
        private System.Windows.Forms.Button copyFingerprint;
        private System.Windows.Forms.Button ok;
    }
}