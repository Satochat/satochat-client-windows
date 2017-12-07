using System;
using System.Windows.Forms;

namespace Satochat.Client {
    public partial class TextInputDialog : Form {
        private string _inputText;

        public TextInputDialog(string prompt, string initialInput = "") {
            InitializeComponent();
            this.prompt.Text = prompt;
            this.input.Text = initialInput;
        }

        private void ok_Click(object sender, EventArgs e) {
            _inputText = input.Text;
        }

        public string GetInput() => _inputText;
    }
}
