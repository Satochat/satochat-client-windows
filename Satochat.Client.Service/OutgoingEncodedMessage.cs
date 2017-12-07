namespace Satochat.Client.Service {
    public class OutgoingEncodedMessage {
        public string Recipient { get; }
        public string Digest { get; }
        public string Payload { get; }
        public string Iv{ get; }
        public string Key { get; }

        public OutgoingEncodedMessage(string recipient, string digest, string payload, string iv, string key) {
            Recipient = recipient;
            Digest = digest;
            Payload = payload;
            Iv = iv;
            Key = key;
        }
    }
}
