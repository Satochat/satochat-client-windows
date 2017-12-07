using System;
using System.Text;
using Newtonsoft.Json;

namespace Satochat.Client.Service {
    public class MessagePayload {
        public string Content { get; }
        public string Nonce { get; }

        public MessagePayload(string content, string nonce) {
            Content = content;
            Nonce = nonce;
        }

        public byte[] ToBytes() {
            var json = JsonConvert.SerializeObject(this);
            var bytes = Encoding.UTF8.GetBytes(json);
            return bytes;
        }

        public static MessagePayload FromBytes(byte[] bytes) {
            throw new NotImplementedException();
        }
    }
}
