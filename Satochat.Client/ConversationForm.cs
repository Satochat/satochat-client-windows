using Satochat.Client.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Satochat.Client.Emoji;
using Satochat.Client.Service.Api;
using Satochat.Client.Util;
using Satochat.Shared.Domain;
using System.Linq;
using System.Threading;
using Satochat.Client.Properties;

namespace Satochat.Client {
    public partial class ConversationForm : Form {
        private static readonly string DefaultTitle = "Conversation";

        private static readonly string HistoryLayoutHtmlPath = PathUtil.GetPathRelativeToApp("assets\\html\\conversation\\layout\\history.html");
        private static readonly string PreviewLayoutHtmlPath = PathUtil.GetPathRelativeToApp("assets\\html\\conversation\\layout\\preview.html");
        private static readonly string EmojiHtml = File.ReadAllText(PathUtil.GetPathRelativeToApp("assets\\html\\conversation\\emoji.html"));
        private static readonly string MessageHtml = File.ReadAllText(PathUtil.GetPathRelativeToApp("assets\\html\\conversation\\message.html"));
        
        private static readonly string PatternForCodeBlockOrOtherTextFormat = @"(?:^(```)(?:\r?\n))(.+?)(?:(?:\r?\n)\1\r?$)|^(.*?)\r?$";
        private static readonly string PatternForInlineTextFormat = @"(`[^\r\n]+?`|\*[^\r\n]+?\*|_[^\r\n]+?_)";
        private static readonly string PatternForInlineInnerTextFormat = @"([`*_])(.+?)\1";
        //private static readonly string PatternForInlineInnerTextFormatSplit = @"([`*_].+?\1)";
        private static readonly string MessageLineHtml = @"<div class=""line"">{{text}}</div>";

        private readonly IEnumerable<Friend> _friends;
        private readonly SatoService _service;
        private readonly Conversation _conversation;
        private readonly EmojiManager _emojiManager;
        private bool _fakeSend;
        private DateTimeOffset _lastMessageTime = DateTimeOffset.MinValue;
        private int _messagesSent;
        private int _messagesReceived;
        private DateTimeOffset _lastTimeNotifiedTyping = DateTimeOffset.MinValue;

        public ConversationForm(IEnumerable<Friend> friends, SatoService service, Conversation conversation, EmojiManager emojiManager) {
            InitializeComponent();
            _friends = friends;
            _service = service;
            _conversation = conversation;
            _emojiManager = emojiManager;
            
            loadSizeAndPosition();
        }

        #region UI events handlers

        private async void input_KeyDown(object sender, KeyEventArgs e) {
            if (e.Modifiers != Keys.None || e.KeyCode != Keys.Enter) {
                return;
            }

            if (input.ReadOnly) {
                    return;
            }

            e.Handled = true;
            e.SuppressKeyPress = true;

            if (!canSendCurrentMessage()) {
                return;
            }

            var content = new MessageContent(input.Text);
            input.ReadOnly = true;

            try {
                await sendMessage(content);
                input.Clear();
            } catch (SatoApiException ex) {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } finally {
                input.ReadOnly = false;
            }
        }

        private void ConversationForm_Shown(object sender, EventArgs e) {
            input.Focus();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e) {
            if (tabControl1.SelectedTab == previewTab) {
                updatePreview(input.Text);
                updateInputHeight();
            } else if (tabControl1.SelectedTab == editorTab) {
                updateInputHeight();
                tabControl1.Focus();
                editorTab.Focus();
                input.Focus();
            }
        }

        private void ConversationForm_ResizeEnd(object sender, EventArgs e) {
            saveSizeAndPosition();
        }

        private async void ConversationForm_Load(object sender, EventArgs e) {
            enableDebugFeatures(Settings.Default.EnableDebug);

            await browserNavigate(history, HistoryLayoutHtmlPath);
            await browserNavigate(preview, PreviewLayoutHtmlPath);

            var font = Properties.Settings.Default.Font;
            if (font != null) {
                input.Font = font;
            }
            
            updateInputHeight();

            _conversation.UserTyping += Conversation_UserTyping;
            _conversation.MessageAdded += ConversationOnMessageAdded;
            _conversation.MessageDeliveryStatusChanged += onMessageDeliveryStatusChanged;
            loadMessages(DateTimeOffset.MinValue);
        }

        private void ConversationForm_FormClosed(object sender, FormClosedEventArgs e) {
            _conversation.MessageDeliveryStatusChanged -= onMessageDeliveryStatusChanged;
            _conversation.MessageAdded -= ConversationOnMessageAdded;
            _conversation.UserTyping -= Conversation_UserTyping;
        }

        private async void input_TextChanged(object sender, EventArgs e) {
            updateInputHeight();

            if (input.TextLength == 0) {
                return;
            }

            var currentTime = DateTimeOffset.UtcNow;
            if (currentTime - _lastTimeNotifiedTyping >= TimeSpan.FromSeconds(1)) {
                _lastTimeNotifiedTyping = currentTime;
                try {
                    await _service.NotifyTypingInConversationAsync(_conversation);
                } catch (SatoApiException) {
                    // Does not matter
                }
            }
        }

        private void ConversationForm_SizeChanged(object sender, EventArgs e) {
            updateInputHeight();
        }

        private void history_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            handleBrowserNavigating(sender, e);
        }

        private void preview_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            handleBrowserNavigating(sender, e);
        }
        
        private void statsTimer_Tick(object sender, EventArgs e) {
            Text = string.Format("[Sent: {0}] [Recv: {1}]", _messagesSent, _messagesReceived);
        }

        #region Main menu

        private void changeFontMenuItem_Click(object sender, EventArgs e) {
            var settings = Properties.Settings.Default;

            var dialog = new FontDialog {
                ShowEffects = false,
                ShowApply = false,
                ShowColor = false,
                FontMustExist = true,
                Font = settings.Font
            };

            if (dialog.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            var font = dialog.Font;
            settings.Font = font;
            settings.Save();

            setBrowserFont(history, font, input.Font);
            setBrowserFont(preview, font, input.Font);
            input.Font = font;
        }

        private void emojiMappingMenuItem_Click(object sender, EventArgs e) {
            var form = new EmojiMap(_emojiManager);
            form.Show();
        }

        private void addFakeMessagesMenuItem_Click(object sender, EventArgs e) {
            var authors = new List<string> {
                "Alice",
                "Bob"
            };

            var baseTime = DateTime.Now;

            for (int i = 1; i <= 10; ++i) {
                for (int authorIndex = 0; authorIndex < authors.Count; ++authorIndex) {
                    bool self = authorIndex == 0;
                    string author = authors[authorIndex];
                    bool multiline = i % 2 == 0;
                    string extraLine = multiline ? Environment.NewLine + Environment.NewLine + Environment.NewLine + "Another line." : "";
                    var content = new MessageContent($"Test message #{i}." + extraLine);
                    var time = baseTime.AddSeconds(i);
                    string messageHtml = htmlForMessage(self, author, Guid.NewGuid().ToString(), content, time, MessageDeliveryStatus.Delivered);
                    addMessageInternal(messageHtml);
                }
            }
        }

        private void addTextFormattingDemoMenuItem_Click(object sender, EventArgs e) {
            string[] messages = {
                @"```
This is a code block. It is below the time and name.
Emoji and other text formatting symbols can be written here:

\pc [emoji:custom throw pc] `asd` *asd* _asd_
```

Outside of a code block, the same line looks like this:

\pc [emoji:custom throw pc] `asd` *asd* _asd_

Some code can also be written inline like `\pc [emoji:throw pc] *asd* _asd_` more easily.

*Bold and _italic_* _can be *combined*_.

Inline formatting does not support multiple lines:

`a
b`
",
                @"URLs turn into hyperlinks: https://www.steffenl.com
... Unless you do not want to: `https://www.steffenl.com`",
                @"URL with text formatting is not supported: *_ https://www.steffenl.com _*",
                @"*_Same with emoji :)_*, but outside is fine. :)",
                @"HTML is safely encoded: <script>alert('Not annoying at all.')</script>",
                @"Emoji shortcut ohyes (`ohyes`) maps to [emoji:custom ohyes] (`[emoji:custom ohyes]`) which contains the common word (`ohyes`). This is processed only once, and does not turn into `[emoji:[emoji:ohyes]]`.
Shortcuts that have multiple aliases also do not get processed multiple times: :d (`:d`) :D (`:D`)"
            };

            foreach (string msg in messages) {
                addMessageInternal(htmlForMessage(true, "Alice", Guid.NewGuid().ToString(), new MessageContent(msg), DateTime.Now, MessageDeliveryStatus.Delivered));
            }
        }

        private void browserInfoMenuItem_Click(object sender, EventArgs e) {
            var browser = history;
            string info = "Version: " + browser.Version;
            MessageBox.Show(info, "Browser info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void copyHistoryHtmlMenuItem_Click(object sender, EventArgs e) {
            var html = history.Document?.Body?.InnerHtml ?? "";
            setClipboardText(html);
        }

        private void copyPreviewHtmlMenuItem_Click(object sender, EventArgs e) {
            var html = preview.Document?.Body?.InnerHtml ?? "";
            setClipboardText(html);
        }

        private void fakeSendingMessageMenuItem_Click(object sender, EventArgs e) {
            fakeSendingMessageMenuItem.Checked = !fakeSendingMessageMenuItem.Checked; 
            _fakeSend = fakeSendingMessageMenuItem.Checked;
        }

        private void reloadCssMenuItem_Click(object sender, EventArgs e) {
            reloadCss(history);
            reloadCss(preview);
        }

        private async void spamMessagesMenuItem_Click(object sender, EventArgs e) {
            bool gotInput = false;
            int count = 0;

            while (!gotInput) {
                var inputDialog = new TextInputDialog("How many?", "1");
                if (inputDialog.ShowDialog(this) != DialogResult.OK) {
                    return;
                }

                if (!int.TryParse(inputDialog.GetInput(), out count)) {
                    continue;
                }

                gotInput = true;
            }

            DialogResult asyncAnswer = MessageBox.Show(this, "Send asynchronously (more stressful)?", "Stress", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (asyncAnswer != DialogResult.Yes && asyncAnswer != DialogResult.No) {
                return;
            }

            bool useAsync = asyncAnswer == DialogResult.Yes;
            
            for (int i = 1; i <= count; ++i) {
                var content = new MessageContent($"Debug message #{i}.");
                if (useAsync) {
                    sendMessage(content);
                } else {
                    await sendMessage(content);
                }
            }
        }

        private void enableStatsMenuItem_Click(object sender, EventArgs e) {
            bool enable = enableStatsMenuItem.Checked = !enableStatsMenuItem.Checked;
            statsTimer.Enabled = enable;
            if (!enable) {
                Text = DefaultTitle;
            }
        }

        #endregion

        #endregion

        #region Event handlers

        private void Conversation_UserTyping(object sender, Conversation.UserTypingEventArg arg) {
            Invoke(new MethodInvoker(() => {
                var doc = history.Document;
                var lastElement = doc?.GetElementById("bottom");
                if (doc == null || lastElement == null) {
                    return;
                }

                var args = new object[] { arg.UserUuid, _friends.Single(f => f.Uuid == arg.UserUuid).DisplayNickname };
                doc.InvokeScript("onUserTyping", args);
                lastElement.ScrollIntoView(true);
            }));
        }

        private void onMessageDeliveryStatusChanged(object sender, Conversation.MessageDeliveryStatusChangedEventArg e) {
            Invoke(new MethodInvoker(() => {
                //MessageBox.Show($"Delivery changed for message {e.Message.Uuid} to recipient {e.Message.RecipientUuid}: {e.Status}");
                // FIXME: must make sure browser's document is ready

                var doc = history.Document;
                if (doc == null) {
                    return;
                }

                var args = new object[] { e.Message.Uuid, e.Status.ToString() };
                doc.InvokeScript("onMessageDeliveryStatusChanged", args);
            }));
        }

        private void ConversationOnMessageAdded(object sender, Conversation.AddedEventArg e) {
            Invoke(new MethodInvoker(() => {
                var msg = e.Message;
                string selfUuid = _service.GetUserUuid();
                bool isSelf = msg.AuthorUuid == selfUuid;
                string author = isSelf ? _service.GetDisplayNickname() : _friends.SingleOrDefault(f => f.Uuid == msg.AuthorUuid)?.DisplayNickname ?? msg.AuthorUuid;
                string html = htmlForMessage(isSelf, author, msg.Uuid, msg.Content, msg.Timestamp, msg.DeliveryStatus);
            
                addMessageInternal(html);

                incrementMessageStats(isSelf);
            }));
        }

        #endregion

        #region Public methods

        #endregion

        #region Private methods
        
        private void addMessageInternal(string messageHtml) {
            var doc = history.Document;
            var body = doc?.Body;
            var element = doc?.CreateElement("div");
            var messagesElement = doc?.GetElementById("messages");
            var lastElement = doc.GetElementById("bottom");
            var window = doc?.Window;
            if (doc == null || body == null || element == null || messagesElement == null || lastElement == null || window == null) {
                // TODO: handle this
                return;
            }
            
            element.InnerHtml = messageHtml;
            messagesElement.AppendChild(element);
            lastElement.ScrollIntoView(true);
        }

        private string htmlForMessage(bool authorIsSelf, string author, string messageUuid, MessageContent content, DateTimeOffset time, MessageDeliveryStatus deliveryStatus) {
            string deliveryStatusStr = deliveryStatus == MessageDeliveryStatus.Delivered ? "delivered" : "";
            string timeStr = HtmlUtil.HtmlEncode(time.LocalDateTime.ToLongTimeString());
            author = HtmlUtil.HtmlEncode(author);
            string contentHtml = messageContentToHtml(content.Body);
            
            string messageHtml = MessageHtml
                .Replace("{{delivery_status}}", deliveryStatusStr)
                .Replace("{{message_uuid}}", messageUuid)
                .Replace("{{time}}", timeStr)
                .Replace("{{author}}", author)
                .Replace("{{author_self}}", authorIsSelf ? "self" : "")
                .Replace("{{content}}", contentHtml);

            return messageHtml;
        }

        private string replaceEmoji(string text) {
            string pattern = @"\[emoji:([0-9A-Za-z_\- ()<>]+)\]";
            text = Regex.Replace(text, pattern, m => {
                var emojiTextGroup = m.Groups[0];
                var emojiCodeGroup = m.Groups[1];

                string emojiText = emojiTextGroup.Value;
                //string emojiCode = emojiCodeGroup.Value.Replace(" ", "_");
                string emojiCode = emojiCodeGroup.Value;
                string emojiFilePath = _emojiManager.GetEmojiPath(emojiCode);

                string replacement;
                if (string.IsNullOrEmpty(emojiFilePath)) {
                    replacement = emojiText;
                } else {
                    replacement = EmojiHtml
                        .Replace("{{path}}", HtmlUtil.HtmlEncode(emojiFilePath))
                        .Replace("{{text}}", emojiText);
                }

                return replacement;
            });
            return text;
        }

        private string replaceUrls(string text) {
            text = Regex.Replace(text, @"(https?://[\S]+)", @"<a href=""$1"">$1</a>");
            return text;
        }

        private string replaceTextFormatting(string text) {
            // TODO: *** = bold and italic
            var map = new Dictionary<string, Func<string, string>> {
                { "_", t => $"<em>{t}</em>" },
                { "*", t => $"<strong>{t}</strong>" },
                { "```", t => $"<code class=\"block\">{t}</code>" },
                { "`", t => $"<code>{t}</code>" }
            };

            string inlineEvaluator(Match m) {
                if (!m.Success) {
                    return m.Groups[0].Value;
                }

                if (map.TryGetValue(m.Groups[1].Value, out var func2)) {
                    return func2(m.Groups[2].Value);
                }
                
                return m.Groups[0].Value;
            }

            string inlineEvaluatorRecursive(string t, int depth) {
                if (depth >= 5) {
                    return t;
                }

                if (Regex.IsMatch(t, PatternForInlineInnerTextFormat)) {
                    t = Regex.Replace(t, PatternForInlineInnerTextFormat, inlineEvaluator);
                } else {
                    return t;
                }
                
                if (string.IsNullOrEmpty(t)) {
                    return t;
                }

                return inlineEvaluatorRecursive(t, depth + 1);
            }

            string replaceInline(string t) {
                var m = Regex.Match(t, PatternForInlineInnerTextFormat);
                if (m.Success) {
                    t = inlineEvaluatorRecursive(t, 0);
                } else {
                    t = replaceUrls(t);
                    t = _emojiManager.ReplaceShortcuts(t);
                    t = replaceEmoji(t);
                }

                return t;
            }

            string codeBlockOrOtherEvaluator(Match m) {
                if (m.Groups[3].Success) {
                    var parts = Regex.Split(m.Groups[3].Value, PatternForInlineTextFormat);
                    parts = parts.Select(replaceInline).ToArray();
                    string t = string.Join("", parts);
                    t = string.IsNullOrEmpty(t) ? Environment.NewLine : t;
                    t = MessageLineHtml.Replace("{{text}}", t);
                    return t;
                }

                if (map.TryGetValue(m.Groups[1].Value, out var func)) {
                    string t = func(m.Groups[2].Value);
                    return t;
                }

                string t2 = m.Groups[0].Value;
                return t2;
            }

            text = Regex.Replace(text, PatternForCodeBlockOrOtherTextFormat, codeBlockOrOtherEvaluator, RegexOptions.Multiline | RegexOptions.Singleline);
            return text;
        }

        private string messageContentToHtml(string text) {
            // We need to decode everything we want decoded, because the opposite is hard to get right
            text = HtmlUtil.HtmlEncode(text);
            text = replaceTextFormatting(text);
            return text;
        }

        private bool canSendCurrentMessage() {
            return !string.IsNullOrEmpty(input.Text);
        }

        private async Task sendMessage(MessageContent content) {
            if (_fakeSend) {
                addMessageInternal(htmlForMessage(true, _service.GetDisplayNickname(), Guid.NewGuid().ToString(), content, DateTimeOffset.Now.DateTime, MessageDeliveryStatus.Delivered));
                return;
            }
            
            await _service.SendMessageAsync(_conversation, content);
        }

        private void openUrl(string url) {
            /*if (MessageBox.Show(this, "The following URL will be opened:\n\n" + url, "Open URL",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) {
                Process.Start(url);
            }*/
            
            Process.Start(url);
        }

        private void setBrowserFont(WebBrowser browser, Font font, Font defaultFont) {
            var body = browser?.Document?.Body;
            if (body == null) {
                return;
            }

            font = font ?? defaultFont;

            if (font == null) {
                return;
            }

            string fontFamily = font.FontFamily.Name;
            string fontSize = font.Size.ToString(CultureInfo.InvariantCulture);

            var builder = new StringBuilder();

            builder
                .Append($"font-family: '{fontFamily}';")
                .Append($"font-size: {fontSize}pt;");

            if (font.Bold) {
                builder.Append("font-weight: bold;");
            }

            if (font.Italic) {
                builder.Append("font-style: italic;");
            }

            body.Style = builder.ToString();
        }

        private void updatePreview(string content) {
            var doc = preview.Document;
            var body = doc?.Body;
            var element = doc?.CreateElement("div");
            var window = doc?.Window;
            if (doc == null || body == null || element == null || window == null) {
                // TODO: handle this
                return;
            }

            string html = messageContentToHtml(content);

            body.InnerHtml = "";
            element.InnerHtml = html;
            body.AppendChild(element);

            element.ScrollIntoView(true);
        }

        private void updateInputHeight() {
            if (tabControl1.SelectedTab == editorTab) {
                updateInputHeightFromEditor();
            } else if (tabControl1.SelectedTab == previewTab) {
                updateInputHeightFromPreview();
            }
        }

        private void updateInputHeightFromEditor() {
            int physicalLineCount = 0;
            while (input.GetFirstCharIndexFromLine(physicalLineCount) != -1) {
                ++physicalLineCount;
            }

            int lineHeight = input.Font.Height;
            int maxLines = (int)Math.Floor(ClientSize.Height / (double)Math.Max(1, lineHeight)) / 2 - 1;
            int minLines = 3;
            int lineCount = Math.Min(maxLines, Math.Max(minLines, physicalLineCount));
            var tabsSizeDiff = tabControl1.Size - editorTab.Size;
            var inputSizeDiff = input.Size - input.ClientSize;

            var newInputClientSize = new Size(input.ClientSize.Width, lineHeight * lineCount);
            if (newInputClientSize.Height != input.ClientSize.Height) {
                var tabsClientSize = new Size(int.MaxValue, newInputClientSize.Height + tabsSizeDiff.Height + inputSizeDiff.Height);
                tabControl1.ClientSize = tabsClientSize;
            }
        }

        private void updateInputHeightFromPreview() {
            var body = preview.Document?.Body;
            if (body == null) {
                return;
            }

            var clientSize = body.ClientRectangle.Size;
            var tabsSizeDiff = tabControl1.Size - previewTab.Size;
            int minHeight = 0; //_tabsClientSizeForEditor.Height;
            int maxHeight = (int)Math.Floor(ClientSize.Height / 2.0);
            // FIXME: problem when starting with a small height and resizing, and long lines without word wrapping (code block).
            // the vertical scroll bar will not disappear
            // if a horizontal scrollbar is visible, we need to add its height to the height of the tab control so that there
            // is room for the content and the scroll bar
            int height = Math.Min(maxHeight, Math.Max(minHeight, clientSize.Height + tabsSizeDiff.Height));

            if (tabControl1.ClientSize.Height != height) {
                var tabsClientSize = new Size(int.MaxValue, height);
                tabControl1.ClientSize = tabsClientSize;
            }
        }

        private async void reloadCss(WebBrowser browser) {
            string bodyHtml = browser.Document?.Body?.InnerHtml ?? "";
            await browserNavigate(browser, browser.Url);
            var body = browser.Document?.Body;
            if (body != null) {
                body.InnerHtml = bodyHtml;
            }
        }

        private void loadSizeAndPosition() {
            var settings = Properties.Settings.Default;
            Size = settings.ConversationForm_InitialSize;

            if (settings.ConversationForm_HasInitialPosition) {
                StartPosition = FormStartPosition.Manual;
                Location = settings.ConversationForm_InitialPosition;
            } else {
                StartPosition = FormStartPosition.WindowsDefaultLocation;
            }
        }

        private void saveSizeAndPosition() {
            if (WindowState != FormWindowState.Normal) {
                return;
            }

            var settings = Properties.Settings.Default;
            settings.ConversationForm_InitialSize = Size;
            settings.ConversationForm_InitialPosition = Location;
            settings.ConversationForm_HasInitialPosition = true;
            settings.Save();
        }

        private void handleBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (e.Url.IsFile) {
                return;
            }

            e.Cancel = true;
            openUrl(e.Url.OriginalString);
        }

        private void setClipboardText(string text) {
            if (string.IsNullOrEmpty(text)) {
                Clipboard.Clear();
            } else {
                Clipboard.SetText(text);
            }
        }

        private void loadMessages(DateTimeOffset afterTime) {
            string selfUuid = _service.GetUserUuid();
            var filteredMessages = _conversation.GetMessages().Where(m => m.Timestamp > afterTime).OrderBy(m => m.Timestamp).ToArray();
            var mostRecentMessage = filteredMessages.LastOrDefault();
            if (mostRecentMessage != null) {
                _lastMessageTime = mostRecentMessage.Timestamp;
            }
            
            var builder = new StringBuilder();
            foreach (var msg in filteredMessages) {
                bool isSelf = msg.AuthorUuid == selfUuid;
                string author = isSelf ? _service.GetDisplayNickname() : _friends.SingleOrDefault(f => f.Uuid == msg.AuthorUuid)?.DisplayNickname ?? msg.AuthorUuid;
                builder.Append(htmlForMessage(isSelf, author, msg.Uuid, msg.Content, msg.Timestamp, msg.DeliveryStatus));
                
                incrementMessageStats(isSelf);
            }
            
            addMessageInternal(builder.ToString());
        }

        private void incrementMessageStats(bool isSelf) {
            if (isSelf) {
                ++_messagesSent;
            } else {
                ++_messagesReceived;
            }
        }

        private async Task browserNavigate(WebBrowser browser, string url) {
            await browserNavigate(browser, new Uri(url));
        }

        private async Task browserNavigate(WebBrowser browser, Uri url) {
            var resetEvent = new ManualResetEvent(false);
            
            WebBrowserDocumentCompletedEventHandler handler = null;
            handler = (sender, e) => {
                browser.DocumentCompleted -= handler;
                resetEvent.Set();
            };
            browser.DocumentCompleted += handler;
            browser.Navigate(url);

            await Task.Run(() => {
                resetEvent.WaitOne();
            });
                
            setBrowserFont(browser, Properties.Settings.Default.Font, input.Font);
        }

        private void enableDebugFeatures(bool enableDebug) {
            debugMenuItem.Visible = enableDebug;
        }

        #endregion
    }
}
