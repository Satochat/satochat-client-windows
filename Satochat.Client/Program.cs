using System;
using System.Windows.Forms;
using Satochat.Client.Service;
using Satochat.Client.Service.Api;
using Satochat.Client.Properties;

namespace Satochat.Client {
    static class Program {
        private static SatoService _service;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var settings = Settings.Default;
            if (Control.ModifierKeys == Keys.Shift) {
                DialogResult answer = MessageBox.Show("Debug features can be enabled to help investigate a problem when necessary, but should be used with care.\n\nWould you like to enable debug features?", "Debug", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (answer) {
                    case DialogResult.Yes:
                        settings.EnableDebug = true;
                        break;
                    case DialogResult.No:
                        settings.EnableDebug = false;
                        break;
                    default:
                        return;
                }
            }

            string endpoint = Settings.Default.ApiEndpoint;
            var api = new SatoApi(endpoint);
            _service = new SatoService(api);

            Application.Run(new MainForm(_service));
        }
    }
}
