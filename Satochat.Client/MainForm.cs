using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Satochat.Client.Emoji;
using Satochat.Client.Properties;
using Satochat.Client.Service;
using Satochat.Client.Service.Api;
using Satochat.Client.Sound;
using Satochat.Client.Util;
using Satochat.Shared.Crypto;

namespace Satochat.Client {
    public partial class MainForm : Form {
        private readonly SatoService _service;
        private readonly ILogWindow _logWindow = new LogForm();
        private readonly Dictionary<string, ConversationForm> _conversationForms = new Dictionary<string, ConversationForm>();
        private readonly BindingList<Friend> _friends = new BindingList<Friend>();
        private readonly Dictionary<string, string> _friendNicknames = new Dictionary<string, string>();
        private readonly EmojiManager _emojiManager;
        private readonly SoundManager _soundManager = new SoundManager();
        private CancellationTokenSource _cancellationToken = new CancellationTokenSource();
        private Task _listenEventsTask;

        public MainForm(SatoService service) {
            InitializeComponent();
            var settings = Settings.Default;

            _emojiManager = new EmojiManager(PathUtil.GetPathRelativeToApp("assets\\emoji"));
            setupFriendsDataSource();
            loadFriendNicknames();

            _service = service;
            _service.Events.EventReceivedEvent += Events_EventReceivedEvent;
            _service.Events.ExceptionOccurredEvent += Events_ExceptionOccurredEvent;
            _service.Events.FriendListReceivedEvent += Events_FriendListReceivedEvent;
            _service.Events.ConversationInitiatedEvent += Events_ConversationOpenedEvent;
            _service.Events.NewMessageAvailableEvent += Events_NewMessageAvailableEvent;

            _service.SetNickname(settings.Nickname);

            loadSizeAndPosition();
        }

        #region UI event handlers

        private void MainForm_Shown(object sender, EventArgs e) {
            var settings = Settings.Default;

            autoUpdateApp(true);

            if (String.IsNullOrEmpty(settings.PrivateKey)) {
                if (!showKeygen()) {
                    return;
                }

                settings.PublicKeyUploaded = false;
                settings.Save();
            }

            loadKeys();
            showLogin();
        }

        private async void Form_LoginSuccessEvent(object sender) {
            ((LoginForm)sender).Close();
            Show();

            var settings = Settings.Default;
            if (!settings.PublicKeyUploaded) {
                await _service.PutPublicKeyAsync();
                settings.PublicKeyUploaded = true;
                settings.Save();
            }

            Action<string> msg = text => {
                BeginInvoke(new MethodInvoker(() => {
                    MessageBox.Show(text);
                }));
            };

            startListenEventsTask();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            enableDebugFeatures(Settings.Default.EnableDebug);

            var assembly = Assembly.GetAssembly(typeof(Program));

            Version version;
            if (ApplicationDeployment.IsNetworkDeployed) {
                var deploy = ApplicationDeployment.CurrentDeployment;
                version = deploy.CurrentVersion;
            } else {
                version = assembly.GetName().Version;
            }

            var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);
            var name = fileVersion.ProductName;
            Text = string.Format("{0} {1}", name, version);

            pingCheckTimer.Enabled = true;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            pingCheckTimer.Enabled = false;
            _cancellationToken.Cancel();

            // FIXME: Stream.ReadAsync() does not respect the cancellation token, so waiting here can cause a long delay.
            /*try {
                _listenEventsTask?.Wait();
            } catch (TaskCanceledException) {
            } catch (OperationCanceledException) {
            } catch (AggregateException) {
            }*/
            
            var forms = new List<Form>();
            foreach (Form form in Application.OpenForms) {
                forms.Add(form);
            }

            forms.Reverse();
            foreach (Form form in forms) {
                if (form != this) {
                    form.Close();
                }
            }
        }

        private void pingCheckTimer_Tick(object sender, EventArgs e) {
            DateTimeOffset? lastEventTime = _service.GetLastEventTime();
            if (lastEventTime == null) {
                _logWindow.AddLine("The first event has not yet been received; it is too early to reconnect.");
                return;
            }

            var now = DateTimeOffset.Now;
            if (now - lastEventTime.Value < TimeSpan.FromSeconds(20)) {
                return;
            }

            _logWindow.AddLine("Have not received an event for a while. Attempting to reconnect.");
            startListenEventsTask();
        }

        private void MainForm_ResizeEnd(object sender, EventArgs e) {
            saveSizeAndPosition();
        }

        #region Main menu

        private void viewPublicKeyMenuItem_Click(object sender, EventArgs e) {
            var settings = Settings.Default;
            var publicKey = SatoPublicKey.FromPem(settings.PublicKey);
            var form = new ViewPublicKeyForm(publicKey);
            form.ShowDialog(this);
        }

        private async void createNewKeyPairMenuItem_Click(object sender, EventArgs e) {
            var settings = Settings.Default;
            settings.PublicKey = null;
            settings.PrivateKey = null;
            settings.PublicKeyUploaded = false;
            settings.Save();

            if (!showKeygen()) {
                return;
            }
            
            loadKeys();

            try {
                await _service.PutPublicKeyAsync();
            } catch (SatoApiException ex) {
                handleException(ex);
            }
        }

        private void setNicknameMenuItem_Click(object sender, EventArgs e) {
            showSetNickname();
        }

        private void loginMenuItem_Click(object sender, EventArgs e) {
            showLogin();
        }

        private void removeLoginMenuItem_Click(object sender, EventArgs e) {
            var settings = Settings.Default;
            settings.Username = null;
            settings.Password = null;
            settings.Save();
            _service.SetCredential(null, null);
        }

        private void restartClientMenuItem_Click(object sender, EventArgs e) {
            restartClient();
        }

        private void showLogMenuItem_Click(object sender, EventArgs e) {
            showLogMenuItem.Checked = !showLogMenuItem.Checked;
            _logWindow.ShowWindow(this, showLogMenuItem.Checked);
        }

        private void debugMenuItem_Popup(object sender, EventArgs e) {
            showLogMenuItem.Checked = _logWindow.IsShowing();
        }

        private void checkForUpdatesMenuItem_Click(object sender, EventArgs e) {
            autoUpdateApp(false);
        }

        private void aboutAppMenuItem_Click(object sender, EventArgs e) {
            var form = new AboutForm();
            form.Show(this);
        }

        private void enableSoundsMenuItem_Click(object sender, EventArgs e) {
            enableSoundsMenuItem.Checked = !enableSoundsMenuItem.Checked;
            var settings = Settings.Default;
            settings.EnableSounds = enableSoundsMenuItem.Checked;
            settings.Save();
        }

        private void deleteKeyPairMenuItem_Click(object sender, EventArgs e) {
            var settings = Settings.Default;
            settings.PrivateKey = null;
            settings.PublicKey = null;
            settings.PublicKeyUploaded = false;
            settings.Save();
            _service.SetKeys(null, null);
        }

        private void toolsMenuItem_Popup(object sender, EventArgs e) {
            enableSoundsMenuItem.Checked = Settings.Default.EnableSounds;
        }

        private void changeServerMenuItem_Click(object sender, EventArgs e) {
            var settings = Settings.Default;
            var form = new TextInputDialog("Please enter a new URL for the API endpoint.", settings.ApiEndpoint);
            if (form.ShowDialog() != DialogResult.OK) {
                return;
            }

            settings.ApiEndpoint = form.GetInput();
            settings.Save();
            restartClient();
        }
        
        #endregion

        #region Friends
        
        private async void friends_DoubleClick(object sender, EventArgs e) {
            await openConversationWithSelectedFriends();
        }

        private void friends_MouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                friendsContextMenu.Show(friends, e.Location);
            }
        }

        private async void friends_KeyDown(object sender, KeyEventArgs e) {
            if (e.Modifiers != Keys.None || e.KeyCode != Keys.Enter) {
                return;
            }
            
            e.SuppressKeyPress = true;
            await openConversationWithSelectedFriends();
        }

        #region Friends context menu

        private void friendsContextMenu_Popup(object sender, EventArgs e) {
            bool hasSelection = friends.SelectedItem != null;
            converseMenuItem.Enabled = hasSelection;
            setFriendNicknameMenuItem.Enabled = hasSelection;
        }

        private async void converseMenuItem_Click(object sender, EventArgs e) {
            await openConversationWithSelectedFriends();
        }

        private void setFriendNicknameMenuItem_Click(object sender, EventArgs e) {
            int savedCount = 0;

            var selectedFriends = new List<Friend>();
            foreach (var friendObject in friends.SelectedItems) {
                selectedFriends.Add((Friend)friendObject);
            }

            foreach (var friend in selectedFriends) {
                var form = new TextInputDialog(string.Format("Please enter a new nickname for {0}.", friend.DisplayNickname), friend.Nickname);
                if (form.ShowDialog(this) != DialogResult.OK) {
                    return;
                }
                
                string nickname = form.GetInput();
                friend.Nickname = nickname;
                _friendNicknames[friend.Uuid] = nickname;
                
                saveFriendNicknames();
                ++savedCount;
            }

            if (savedCount > 0) {
                setupFriendsDataSource();
            }
        }

        #endregion

        #endregion
        
        #endregion

        #region Service event handlers
        
        private void Events_NewMessageAvailableEvent(object sender, SatoServiceEvent.NewMessageAvailableEventArg arg) {
            Invoke(new MethodInvoker(() => {
                string conversationUuid = arg.EventData.Conversation.Uuid;
                if (!_conversationForms.TryGetValue(conversationUuid, out var conversationForm)) {
                    conversationForm = new ConversationForm(_friends, _service, arg.EventData.Conversation, _emojiManager);
                    conversationForm.FormClosed += (o, args) => {
                        _conversationForms.Remove(conversationUuid);
                    };
                    _conversationForms[conversationUuid] = conversationForm;
                }

                if (!conversationForm.Visible) {
                    conversationForm.Show();
                }

                //conversationForm.Activate();

                var settings = Settings.Default;

                if (!conversationForm.IsActive()) {
                    conversationForm.Flash();
                    if (settings.EnableSounds) {
                        _soundManager.PlayAlias(SoundAlias.InstantMessageReceiver);
                    }
                }
            }));
        }

        private void Events_ConversationOpenedEvent(object sender, SatoServiceEvent.ConversationInitiatedEventArg arg) {
            BeginInvoke(new MethodInvoker(() => {
                string conversationUuid = arg.Conversation.Uuid;

                if (!_conversationForms.TryGetValue(conversationUuid, out var conversationForm)) {
                    conversationForm = new ConversationForm(_friends, _service, arg.Conversation, _emojiManager);
                    conversationForm.FormClosed += (o, args) => {
                        _conversationForms.Remove(conversationUuid);
                    };
                    _conversationForms[conversationUuid] = conversationForm;
                }
                
                if (!conversationForm.Visible) {
                    conversationForm.Show();
                }

                conversationForm.Activate();
            }));
        }

        private void Events_EventReceivedEvent(object sender, SatoServiceEvent.EventReceivedEventArg arg) {
            BeginInvoke(new MethodInvoker(() => {
                _logWindow.AddLine($"Event received: Name = {arg.Name}; Data = {arg.Data}");
            }));
        }

        private void Events_ExceptionOccurredEvent(object sender, SatoServiceEvent.ExceptionOccurredEventArg arg) {
            BeginInvoke(new MethodInvoker(() => {
                _logWindow.AddLine(arg.Exception.ToString());
            }));
        }

        private void Events_FriendListReceivedEvent(object sender, SatoServiceEvent.FriendListReceivedEventArg arg) {
            BeginInvoke(new MethodInvoker(() => {
                _logWindow.AddLine("Friend list received");
                _friends.Clear();

                foreach (var uuid in arg.EventData.Friends) {
                    var friend = _friendNicknames.TryGetValue(uuid, out string nickname) ? new Friend(uuid, nickname) : new Friend(uuid);
                    _friends.Add(friend);
                }
            }));
        }

        #endregion

        #region Private methods
        
        private void loadKeys() {
            var settings = Settings.Default;
            var privateKey = SatoPrivateKey.FromPem(settings.PrivateKey);
            var publicKey = SatoPublicKey.FromPem(settings.PublicKey);
            _service.SetKeys(privateKey, publicKey);
        }

        private bool showKeygen() {
            var keygenForm = new KeygenForm();
            var result = keygenForm.ShowDialog(this);
            return result == DialogResult.OK;
        }

        private void showLogin() {
            var loginForm = new LoginForm(_service);
            loginForm.LoginSuccessEvent += Form_LoginSuccessEvent;
            loginForm.ShowDialog(this);
        }

        private void startListenEventsTask() {
            if (_listenEventsTask != null && _listenEventsTask.Status == TaskStatus.Running) {
                _logWindow.AddLine("Attempted to start listening for events but the previous task is still running.");
                return;
            }

            _logWindow.AddLine("Starting task to listen for events...");

            _listenEventsTask = Task.Factory.StartNew(async () => {
                BeginInvoke(new MethodInvoker(() => {
                    _logWindow.AddLine("Calling service method ListenEvents()...");
                }));

                try {
                    await _service.ListenEvents(_cancellationToken.Token);
                } catch (TaskCanceledException) {
                } catch (OperationCanceledException) {
                } catch (AggregateException ex) {
                    BeginInvoke(new MethodInvoker(() => {
                        _logWindow.AddLine("Aggregate exception during service call ListenEvents(): " + ex.InnerException?.Message);
                    }));
                } catch (Exception ex) {
                    BeginInvoke(new MethodInvoker(() => {
                        _logWindow.AddLine("Exception during service call ListenEvents(): " + ex.Message);
                    }));
                }

                BeginInvoke(new MethodInvoker(() => {
                    _logWindow.AddLine("Exiting startListenEventsTask().");
                }));
            });
        }

        private void handleException(SatoApiException ex) {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void showSetNickname() {
            var form = new TextInputDialog("Please enter a new nickname for yourself.", _service.GetNickname());
            if (form.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            var settings = Settings.Default;

            string nickname = form.GetInput();
            settings.Nickname = nickname;
            settings.Save();
            _service.SetNickname(nickname);
        }

        private void saveFriendNicknames() {
            var settings = Settings.Default;
            if (settings.FriendNicknames == null) {
                settings.FriendNicknames = new StringCollection();
            } else {
                settings.FriendNicknames.Clear();
            }

            foreach (var pair in _friendNicknames) {
                string uuid = pair.Key;
                string nickname = pair.Value;
                string line = uuid + "," + nickname;
                settings.FriendNicknames.Add(line);
            }

            settings.Save();
        }

        private void loadFriendNicknames() {
            _friendNicknames.Clear();

            var settings = Settings.Default;
            if (settings.FriendNicknames == null) {
                settings.FriendNicknames = new StringCollection();
                settings.Save();
                return;
            }

            foreach (string line in settings.FriendNicknames) {
                var parts = line.Split(',');
                if (parts.Length != 2) {
                    continue;
                }

                string uuid = parts[0];
                string nickname = parts[1];
                _friendNicknames[uuid] = nickname;
            }
        }

        private void setupFriendsDataSource() {
            friends.DataSource = null;
            friends.DisplayMember = nameof(Friend.DisplayNickname);
            friends.ValueMember = nameof(Friend.Uuid);
            friends.DataSource = _friends;
        }

        private void autoUpdateApp(bool silent) {
            if (!ApplicationDeployment.IsNetworkDeployed) {
                if (!silent) {
                    MessageBox.Show("This application was not installed in a way that supports updates.", "Unsupported",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return;
            }

            var deploy = ApplicationDeployment.CurrentDeployment;

            try {
                if (!deploy.CheckForUpdate()) {
                    if (!silent) {
                        MessageBox.Show("No updates are available.", "Update", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    return;
                }

                if (!deploy.Update()) {
                    if (!silent) {
                        MessageBox.Show("Unable to apply update right now.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return;
                }

                Application.Restart();
            } catch (Exception ex) {
                if (!silent) {
                    MessageBox.Show("Unable to apply update: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadSizeAndPosition() {
            var settings = Settings.Default;
            Size = settings.MainForm_InitialSize;

            if (settings.MainForm_HasInitialPosition) {
                StartPosition = FormStartPosition.Manual;
                Location = settings.MainForm_InitialPosition;
            } else {
                StartPosition = FormStartPosition.WindowsDefaultLocation;
            }
        }

        private void saveSizeAndPosition() {
            if (WindowState != FormWindowState.Normal) {
                return;
            }

            var settings = Settings.Default;
            settings.MainForm_InitialSize = Size;
            settings.MainForm_InitialPosition = Location;
            settings.MainForm_HasInitialPosition = true;
            settings.Save();
        }

        private async Task openConversationWithSelectedFriends() {
            var friendObjects = new HashSet<string>();
            foreach (var friendObject in friends.SelectedItems) {
                var friend = (Friend)friendObject;
                friendObjects.Add(friend.Uuid);
            }

            if (friendObjects.Count == 0) {
                return;
            }

            try {
                await _service.OpenConversationAsync(friendObjects);
            } catch (SatoApiException ex) {
                handleException(ex);
            }
        }

        private void restartClient() {
            Application.Restart();
        }

        private void enableDebugFeatures(bool enableDebug) {
            debugMenuItem.Visible = enableDebug;
        }

        #endregion
    }
}
