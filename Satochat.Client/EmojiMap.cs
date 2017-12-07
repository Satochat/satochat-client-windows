using System;
using System.Windows.Forms;
using Satochat.Client.Emoji;

namespace Satochat.Client {
    public partial class EmojiMap : Form {
        private readonly EmojiManager _emojiManager;

        public EmojiMap(EmojiManager emojiManager) {
            InitializeComponent();
            _emojiManager = emojiManager;
        }

        private void EmojiMap_Shown(object sender, EventArgs e) {
            loadMap();
        }
        
        private void loadMap() {
            textBox.Text = _emojiManager.GetShortcutMappingsAsText();
        }

        private void saveMap() {
            _emojiManager.SetShortcutMappingsFromText(textBox.Text);
        }

        private void ok_Click(object sender, EventArgs e) {
            saveMap();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
