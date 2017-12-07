using Satochat.Shared.Domain;

namespace Satochat.Client.Service {
    public class OutgoingMessage {
        private readonly string _recipient;
        private readonly MessageContent _content;

        public OutgoingMessage(string recipient, MessageContent content) {
            _recipient = recipient;
            _content = content;
        }

        public string GetRecipient() => _recipient;
        public MessageContent GetContent() => _content;
    }

    public class IncomingMessage {
        private readonly string _author;
        private readonly MessageContent _content;

        public IncomingMessage(string recipient, MessageContent content) {
            _author = recipient;
            _content = content;
        }

        public string GetAuthor() => _author;
        public MessageContent GetContent() => _content;
    }
}
