using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Satochat.Shared.Crypto;

namespace Satochat.Client {
    public partial class KeygenForm : Form {
        private bool _closingForm;

        public KeygenForm() {
            InitializeComponent();
        }

        private async void KeygenForm_Shown(object sender, EventArgs e) {
            await Task.Run(() => {
                var keyPair = SatoKeygen.NewKeyPair();
                var privateKey = keyPair.GetPrivateKey().ToPem();
                var publicKey = keyPair.GetPublicKey().ToPem();

                Invoke(new MethodInvoker(() => {
                    var settings = Properties.Settings.Default;
                    settings.PrivateKey = privateKey;
                    settings.PublicKey = publicKey;
                    settings.Save();
                    DialogResult = DialogResult.OK;
                    _closingForm = true;
                    Close();
                }));
            });
        }

        private void KeygenForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (!_closingForm && e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
            }
        }
    }
}
