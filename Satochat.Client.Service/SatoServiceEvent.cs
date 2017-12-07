using Satochat.Shared.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Satochat.Shared.Domain;

namespace Satochat.Client.Service {
    public class SatoServiceEvent {
        public class EventReceivedEventArg {
            public string Name { get; }
            public string Data { get; }

            public EventReceivedEventArg(string name, string data) {
                Name = name;
                Data = data;
            }
        }

        public delegate void RaiseEventReceivedEventHandler(object sender, EventReceivedEventArg arg);
        public event RaiseEventReceivedEventHandler EventReceivedEvent;
        public void RaiseEventReceivedEvent(object sender, EventReceivedEventArg arg) => EventReceivedEvent?.Invoke(sender, arg);

        public class ExceptionOccurredEventArg {
            public Exception Exception { get; }

            public ExceptionOccurredEventArg(Exception exception) {
                Exception = exception;
            }
        }

        public delegate void ExceptionOccurredEventHandler(object sender, ExceptionOccurredEventArg arg);
        public event ExceptionOccurredEventHandler ExceptionOccurredEvent;
        public void RaiseExceptionOccurredEvent(object sender, ExceptionOccurredEventArg arg) => ExceptionOccurredEvent?.Invoke(sender, arg);

        public class FriendListReceivedEventArg {
            public FriendEvent.FriendList EventData { get; }

            public FriendListReceivedEventArg(FriendEvent.FriendList eventData) {
                EventData = eventData;
            }
        }

        public delegate void FriendListReceivedEventHandler(object sender, FriendListReceivedEventArg arg);
        public event FriendListReceivedEventHandler FriendListReceivedEvent;
        public void RaiseFriendListReceivedEvent(object sender, FriendListReceivedEventArg arg) => FriendListReceivedEvent?.Invoke(sender, arg);

        public class ConversationInitiatedEventArg {
            public Conversation Conversation { get; }

            public ConversationInitiatedEventArg(Conversation conversation) {
                Conversation = conversation;
            }
        }

        public delegate void ConversationInitiatedEventHandler(object sender, ConversationInitiatedEventArg arg);
        public event ConversationInitiatedEventHandler ConversationInitiatedEvent;
        public void ConversationInitiatedOpenedEvent(object sender, ConversationInitiatedEventArg arg) => ConversationInitiatedEvent?.Invoke(sender, arg);

        public class NewMessageAvailable {
            public string Uuid { get; }
            public Conversation Conversation { get; }
            public string Author { get; }
            public MessageContent Content { get; }

            public NewMessageAvailable(string uuid, Conversation conversation, string author, MessageContent content) {
                Uuid = uuid;
                Conversation = conversation;
                Author = author;
                Content = content;
            }
        }

        public class NewMessageAvailableEventArg {
            public NewMessageAvailable EventData { get; }

            public NewMessageAvailableEventArg(NewMessageAvailable eventData) {
                EventData = eventData;
            }
        }

        public delegate void NewMessageAvailableEventHandler(object sender, NewMessageAvailableEventArg arg);
        public event NewMessageAvailableEventHandler NewMessageAvailableEvent;
        public void RaiseNewMessageAvailableEvent(object sender, NewMessageAvailableEventArg arg) => NewMessageAvailableEvent?.Invoke(sender, arg);
    }
}
