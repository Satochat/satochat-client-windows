using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Satochat.Client {
    public partial class LogForm : Form, ILogWindow {
        public LogForm() {
            InitializeComponent();
        }

        public void AddLine(string line) {
            logLines.Items.Add(string.Format("[{0}] {1}", DateTimeOffset.Now, line));
            if (scrollAutomaticallyMenuItem.Checked) {
                logLines.TopIndex = logLines.Items.Count - 1;
            }
        }

        public void ShowWindow(IWin32Window owner, bool show) {
            if (show && !Visible) {
                Show();
            } else if (!show && Visible) {
                Hide();
            }
        }

        public bool IsShowing() {
            return Visible;
        }

        private void LogForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Hide();
            }
        }

        private void copyAllMenuItem_Click(object sender, EventArgs e) {
            List<string> lines = new List<string>();
            foreach (string line in logLines.Items) {
                lines.Add(line);
            }

            string text = string.Join(Environment.NewLine, lines);
            Clipboard.SetText(text);
        }

        private void copySelectionMenuItem_Click(object sender, EventArgs e) {
            List<string> lines = new List<string>();
            foreach (string line in logLines.SelectedItems) {
                lines.Add(line);
            }

            string text = string.Join(Environment.NewLine, lines);
            Clipboard.SetText(text);
        }

        private void logLines_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                logContextMenu.Show(logLines, e.Location);
            }
        }

        private void scrollAutomaticallyMenuItem_Click(object sender, EventArgs e) {
            scrollAutomaticallyMenuItem.Checked = !scrollAutomaticallyMenuItem.Checked;
        }
    }
}
