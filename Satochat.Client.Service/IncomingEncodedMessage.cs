namespace Satochat.Client.Service {
    public class IncomingEncodedMessage {
        public string Uuid { get; }
        public string Author { get; }
        public string Digest { get; }
        public string Payload { get; }
        public string Iv{ get; }
        public string Key { get; }

        public IncomingEncodedMessage(string uuid, string author, string digest, string payload, string iv, string key) {
            Uuid = uuid;
            Author = author;
            Digest = digest;
            Payload = payload;
            Iv = iv;
            Key = key;
        }
    }
}
