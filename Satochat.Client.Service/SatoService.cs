using Newtonsoft.Json;
using Satochat.Shared.Event;
using Satochat.Shared.Codec;
using Satochat.Shared.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Satochat.Client.Service.Api;
using Satochat.Shared.Domain;
using Satochat.Shared.Util;
using Satochat.Shared.ViewModels;

namespace Satochat.Client.Service {
    public class SatoService {
        //public static SatoService Instance;

        private readonly SatoApi _api;
        private readonly SatoApiEventHandler _eventHandler = new SatoApiEventHandler();
        private readonly SatoApiEventFactory _eventFactory = new SatoApiEventFactory();
        private readonly MessageEncoder _messageEncoder;
        private SatoKeyPair _keyPair;
        private string _userUuid;
        private string _userNickname;
        private readonly Dictionary<string, Conversation> _conversations = new Dictionary<string, Conversation>();
        //private readonly List<Friend> _friends = new List<Friend>();
        //private readonly Dictionary<string, string> _friendNicknames = new Dictionary<string, string>();
        private DateTimeOffset? _lastEventTime;
        private readonly object _lastEventTimeLock = new object();

        private readonly Dictionary<string, Func<string, Task>> _eventTypeMap = new Dictionary<string, Func<string, Task>>();

        public readonly SatoServiceEvent Events = new SatoServiceEvent();

        public SatoService(SatoApi api) {
            var publicKeyStore = new PublicKeyStore(this);

            _api = api;
            _messageEncoder = new MessageEncoder(publicKeyStore);
            _eventHandler.EventReceivedEvent += _eventHandler_EventReceivedEventAsync;

            _eventTypeMap.Add(FriendEvent.FriendList.Name, async data => Events.RaiseFriendListReceivedEvent(this, new SatoServiceEvent.FriendListReceivedEventArg(await handleFriendListReceived(data))));
            _eventTypeMap.Add(MessageEvent.NewMessageAvailable.Name, async data => Events.RaiseNewMessageAvailableEvent(this, new SatoServiceEvent.NewMessageAvailableEventArg(await handleNewMessageAvailable(data))));
            _eventTypeMap.Add(MessageEvent.UserReceivedMessage.Name, async data => await handleUserReceivedMessage(data));
            _eventTypeMap.Add(MessageEvent.UserTypingInConversation.Name, async data => await handleUserTyping(data));
        }

        private async Task<FriendEvent.FriendList> handleFriendListReceived(string data) {
            var eventData = JsonConvert.DeserializeObject<FriendEvent.FriendList>(data);
            /*foreach (var uuid in eventData.Friends) {
                _friends.Add(new Friend(uuid));
            }*/

            return new FriendEvent.FriendList(eventData.Friends);
        }

        private async Task<SatoServiceEvent.NewMessageAvailable> handleNewMessageAvailable(string data) {
            var eventData = JsonConvert.DeserializeObject<MessageEvent.NewMessageAvailable>(data);
            string uuid = eventData.Message;
            var result = await _api.GetMessageAsync(uuid);
            var incoming = result.Message;
            var encodedMessage = new IncomingEncodedMessage(incoming.Uuid, incoming.Author, incoming.Digest, incoming.Payload, incoming.Iv, incoming.Key);
            IncomingMessage incomingMessage;
            try {
                incomingMessage = _messageEncoder.DecodeAsync(encodedMessage, _keyPair.GetPrivateKey()).Result;
            } catch (Exception ex) {
                // FIXME: use proper exception
                throw new SatoApiException("Failed to decode incoming message with UUID " + uuid, ex);
            }

            if (!_conversations.TryGetValue(result.Conversation, out var conversation)) {
                var converationResult = await _api.GetConversationAsync(result.Conversation);
                var participants = converationResult.Participants;
                conversation = new Conversation(participants);
                _conversations[result.Conversation] = conversation;
            }
            
            conversation.AddMessage(new Message {
                AuthorUuid = incomingMessage.GetAuthor(),
                Content = incomingMessage.GetContent(),
                DeliveryStatus = MessageDeliveryStatus.Delivered,
                RecipientUuid = GetUserUuid(),
                Uuid = encodedMessage.Uuid,
                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(incoming.Timestamp)
            });
            
            await NotifyMessageReceivedAsync(encodedMessage.Uuid);
            //_logWindow.AddLine($"Failed to notify that message with UUID {arg.EventData.Uuid} was received: {ex.Message}");

            var serviceEvent = new SatoServiceEvent.NewMessageAvailable(encodedMessage.Uuid, conversation, incomingMessage.GetAuthor(), incomingMessage.GetContent());
            return serviceEvent;
        }

        private async Task handleUserReceivedMessage(string data) {
            var eventData = JsonConvert.DeserializeObject <MessageEvent.UserReceivedMessage>(data);
            if (!_conversations.TryGetValue(eventData.Conversation, out var conversation)) {
                // TODO: log unknown conversation
                // keep in mind that when the app restarts, the conversation may initially be unknown
                return;
            }

            var message = conversation.GetMessage(eventData.Message);
            if (message == null) {
                // TODO: log unknown message
                // for this to work, we must make sure that the messages are actually tracked in the conversation (retrieved from server or from local storage)
                return;
            }

            conversation.SetMessageDeliveryStatus(message.Uuid, MessageDeliveryStatus.Delivered);
        }
        
        private async Task handleUserTyping(string data) {
            var eventData = JsonConvert.DeserializeObject<MessageEvent.UserTypingInConversation>(data);
            if (!_conversations.TryGetValue(eventData.Conversation, out var conversation)) {
                // TODO: log unknown conversation
                // keep in mind that when the app restarts, the conversation may initially be unknown
                return;
            }

            conversation.RaiseUserTyping(eventData.User);
        }

        public async Task SendMessageAsync(Conversation conversation, MessageContent content) {
            var encodedMessages = new List<OutgoingEncodedMessage>();
            var recipients = new HashSet<string>(conversation.Participants);
            recipients.Remove(_userUuid);

            foreach (var recipient in recipients) {
                var message = new OutgoingMessage(recipient, content);
                var encodedMessage = await _messageEncoder.EncodeAsync(message);
                encodedMessages.Add(encodedMessage);
            }

            var model = new MessageViewModel.SendMessage();
            
            foreach (var msg in encodedMessages) {
                model.Variants.Add(new MessageViewModel.OutgoingEncodedMessage(msg.Recipient, msg.Digest, msg.Payload, msg.Iv, msg.Key));
            }

            var res = await _api.SendMessageAsync(model);

            foreach (var item in res.Items) {
                conversation.AddMessage(new Message {
                    AuthorUuid = GetUserUuid(),
                    Content = content,
                    DeliveryStatus = MessageDeliveryStatus.Undelivered,
                    RecipientUuid = item.Recipient,
                    Uuid = item.Message,
                    Timestamp = DateTimeOffset.Now // FIXME: use real timestamp
                });
            }
        }

        private async Task _eventHandler_EventReceivedEventAsync(object sender, SatoApiEventHandler.EventReceivedEventArg arg) {
            lock (_lastEventTimeLock) {
                _lastEventTime = DateTimeOffset.Now;
            }

            bool skipNotify = arg.Name == "ping" || arg.Name == "friend-list";
            if (!skipNotify && arg.Uuid != null) {
                // Notify before invoking other handlers
                try {
                    await NotifyEventReceivedAsync(arg.Uuid);
                } catch (SatoApiException ex) {
                    if (ex.StatusCode == HttpStatusCode.NotFound) {
                        // Don't process events that no longer exist on the server
                        return;
                    }

                    Events.RaiseExceptionOccurredEvent(this, new SatoServiceEvent.ExceptionOccurredEventArg(ex));
                    return;
                }
            }

            Events.RaiseEventReceivedEvent(this, new SatoServiceEvent.EventReceivedEventArg(arg.Name, arg.Data));

            try {
                Func<string, Task> call = null;
                if (_eventTypeMap.TryGetValue(arg.Name, out call)) {
                    await call.Invoke(arg.Data);
                }
            } catch (SatoApiException ex) {
                Events.RaiseExceptionOccurredEvent(this, new SatoServiceEvent.ExceptionOccurredEventArg(ex));
            }
        }

        public DateTimeOffset? GetLastEventTime() {
            lock (_lastEventTimeLock) {
                return _lastEventTime;
            }
        }

        public void SetKeys(SatoPrivateKey privateKey, SatoPublicKey publicKey) {
            _keyPair = new SatoKeyPair(privateKey, publicKey);
        }

        public void SetCredential(string username, string password) {
            _api.SetCredential(new Credential(username, password));
        }

        public async Task HelloAsync() {
            var whoami = await _api.WhoamiAsync();
            _userUuid = whoami.Uuid;
        }

        public string GetUserUuid() => _userUuid;

        public string GetDisplayNickname() => string.IsNullOrEmpty(_userNickname) ? _userUuid : _userNickname;

        public string GetNickname() => _userNickname;

        public async Task PutPublicKeyAsync() {
            await _api.PutPublicKeyAsync(_keyPair.GetPublicKey().ToPem());
        }

        public async Task<SatoPublicKey> GetPublicKey(string fingerprint) {
            string pem = (await _api.GetPublicKeyAsync(fingerprint)).Key;
            return SatoPublicKey.FromPem(pem);
        }

        public async Task ListenEvents(CancellationToken cancellationToken) {
            cancellationToken.ThrowIfCancellationRequested();
            await _api.ListenEvents(_eventHandler, cancellationToken);

            /*try {
                await _api.ListenEvents(_eventHandler, cancellationToken);
            } catch (Exception ex) {
                Events.RaiseExceptionOccurredEvent(this, new SatoServiceEvent.ExceptionOccurredEventArg(ex));
            } finally {
                cancellationToken.ThrowIfCancellationRequested();
            }*/
        }

        public async Task OpenConversationAsync(IEnumerable<string> friends) {
            var participants = new List<string>(friends) { _userUuid };
            string uuid = ConversationUtil.GenerateUuid(participants);

            if (!_conversations.TryGetValue(uuid, out var conversation)) {
                conversation = new Conversation(participants);
                _conversations[uuid] = conversation;
            }

            Events.ConversationInitiatedOpenedEvent(this, new SatoServiceEvent.ConversationInitiatedEventArg(conversation));
        }

        public void SetNickname(string nickname) {
            _userNickname = nickname;
        }

        public async Task NotifyMessageReceivedAsync(string uuid) {
            await _api.NotifyMessageReceivedAsync(uuid);
        }

        public async Task NotifyEventReceivedAsync(string uuid) {
            await _api.NotifyEventReceivedAsync(uuid);
        }

        public async Task NotifyTypingInConversationAsync(Conversation conversation) {
            await _api.NotifyTypingInConversationAsync(conversation.Uuid);
        }
    }
}
