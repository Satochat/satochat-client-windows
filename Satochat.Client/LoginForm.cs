using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Satochat.Client.Service;
using Satochat.Client.Service.Api;

namespace Satochat.Client {
    public partial class LoginForm : Form {
        private readonly SatoService _service;

        public delegate void LoginSuccessEventHandler(object sender);

        public event LoginSuccessEventHandler LoginSuccessEvent;

        public LoginForm(SatoService service) {
            InitializeComponent();
            _service = service;
        }

        private async void login_Click(object sender, EventArgs e) {
            bool success = await tryLogIn(username.Text, password.Text);
            if (!success) {
                return;
            }

            var settings = Properties.Settings.Default;
            settings.Username = username.Text;
            settings.Password = password.Text;
            settings.Save();

            raiseLoginSuccessEvent();
        }

        private async void LoginForm_Shown(object sender, EventArgs e) {
            var settings = Properties.Settings.Default;
            //var api = SatoApi.Instance;
            //api.CredentialEvent += credentialEvent;

            enableControls(false);

            if (String.IsNullOrEmpty(settings.Username) || String.IsNullOrEmpty(settings.Password)) {
                enableControls(true);
                return;
            }

            username.Text = settings.Username;
            password.Text = settings.Password;

            if (await tryLogIn(settings.Username, settings.Password)) {
                raiseLoginSuccessEvent();
            }
        }

        private void enableControls(bool enable) {
            username.Enabled = enable;
            password.Enabled = enable;
            login.Enabled = enable;
        }

        /*private void credentialEvent(object sender, SatoApi.CredentialEventArg arg) {
            var settings = Properties.Settings.Default;

            if (String.IsNullOrEmpty(settings.Username) || String.IsNullOrEmpty(settings.Password)) {
                arg.Username = username.Text;
                arg.Password = password.Text;
            } else {
                arg.Username = settings.Username;
                arg.Password = settings.Password;
            }
        }*/

        private async Task<bool> tryLogIn(string username, string password) {
            var settings = Properties.Settings.Default;
            //var service = SatoService.Instance;
            _service.SetCredential(username, password);
            enableControls(false);

            try {
                await _service.HelloAsync();
                return true;
            } catch (SatoApiException ex) {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                enableControls(true);
                return false;
            }
        }

        private void raiseLoginSuccessEvent() {
            LoginSuccessEvent?.Invoke(this);
        }
    }
}
