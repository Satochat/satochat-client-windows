using Satochat.Shared.Crypto;
using System;
using System.Windows.Forms;
using Satochat.Shared.Util;

namespace Satochat.Client {
    public partial class ViewPublicKeyForm : Form {
        private readonly SatoPublicKey _publicKey;
        
        public ViewPublicKeyForm(SatoPublicKey publicKey) {
            InitializeComponent();
            _publicKey = publicKey;
        }

        private void ViewPublicKeyForm_Shown(object sender, EventArgs e) {
            fingerprint.Text = _publicKey.GetFingerprint();
            key.Text = StringUtil.ConvertLineEndingsToSystem(_publicKey.ToPem());
        }

        private void copyFingerprint_Click(object sender, EventArgs e) {
            Clipboard.SetText(fingerprint.Text);
        }

        private void copyKey_Click(object sender, EventArgs e) {
            Clipboard.SetText(key.Text);
        }
    }
}
