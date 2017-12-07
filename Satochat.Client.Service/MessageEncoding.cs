using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Satochat.Shared.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Security;
using Satochat.Client.Service.Api;
using Satochat.Shared.Domain;

namespace Satochat.Client.Service {
    public class MessageEncoder {
        private readonly PublicKeyStore _keyStore;

        public MessageEncoder(PublicKeyStore keyStore) {
            _keyStore = keyStore;
        }

        public async Task<IncomingMessage> DecodeAsync(IncomingEncodedMessage encodedMessage, SatoPrivateKey privateKey) {
            byte[] expectedDigest = Convert.FromBase64String(encodedMessage.Digest);
            byte[] ivBytes = Convert.FromBase64String(encodedMessage.Iv);
            byte[] encryptedSecret = Convert.FromBase64String(encodedMessage.Key);
            byte[] encryptedPayloadBytes = Convert.FromBase64String(encodedMessage.Payload);
            
            var wrapper = WrapperUtilities.GetWrapper("RSA/NONE/PKCS1PADDING");
            wrapper.Init(false, privateKey.GetKey());

            byte[] secretBytes = wrapper.Unwrap(encryptedSecret, 0, encryptedSecret.Length);
            var secretKey = new KeyParameter(secretBytes);
            var ivParam = new ParametersWithIV(secretKey, ivBytes);

            var aesCipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
            aesCipher.Init(false, ivParam);
            
            byte[] payloadBytesBuffer = new byte[aesCipher.GetOutputSize(encryptedPayloadBytes.Length)];
            int processedLength = aesCipher.ProcessBytes(encryptedPayloadBytes, payloadBytesBuffer, 0);
            processedLength += aesCipher.DoFinal(payloadBytesBuffer, processedLength);

            byte[] payloadBytes = new byte[processedLength];
            Buffer.BlockCopy(payloadBytesBuffer, 0, payloadBytes, 0, payloadBytes.Length);
            
            byte[] payloadDigest;
            using (var sha = SHA256.Create()) {
                payloadDigest = sha.ComputeHash(payloadBytes);
            }

            if (!payloadDigest.SequenceEqual(expectedDigest)) {
                throw new SatoApiException("Digest mismatch for incoming message");
            }

            var payload = JsonConvert.DeserializeObject<MessagePayload>(Encoding.UTF8.GetString(payloadBytes));
            var content = new MessageContent(payload.Content);
            var message = new IncomingMessage(encodedMessage.Author, content);
            return message;
        }

        public async Task<OutgoingEncodedMessage> EncodeAsync(OutgoingMessage message) {
            SatoPublicKey recipientPublicKey = await _keyStore.GetKeyAsync(message.GetRecipient());
            MessagePayload payload = new MessagePayload(message.GetContent().Body, createNonce());
            byte[] payloadBytes = payload.ToBytes();

            string payloadDigest;
            using (var sha = SHA256.Create()) {
                byte[] digest = sha.ComputeHash(payloadBytes);
                payloadDigest = Convert.ToBase64String(digest);
            }

            byte[] secretBytes = createRandom(32);
            byte[] ivBytes = createRandom(16);
            
            var secretKey = new KeyParameter(secretBytes);
            var ivParam = new ParametersWithIV(secretKey, ivBytes);
            var aesCipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
            aesCipher.Init(true, ivParam);

            byte[] encryptedPayloadBytes = new byte[aesCipher.GetOutputSize(payloadBytes.Length)];
            int processedLength = aesCipher.ProcessBytes(payloadBytes, encryptedPayloadBytes, 0);
            aesCipher.DoFinal(encryptedPayloadBytes, processedLength);

            var wrapper = WrapperUtilities.GetWrapper("RSA/NONE/PKCS1PADDING");
            wrapper.Init(true, recipientPublicKey.GetKey());
            byte[] encryptedSecret = wrapper.Wrap(secretBytes, 0, secretBytes.Length);

            string encodedSecret = Convert.ToBase64String(encryptedSecret);
            string encodedPayload = Convert.ToBase64String(encryptedPayloadBytes);
            string encodedIv = Convert.ToBase64String(ivBytes);

            var encodedMessage = new OutgoingEncodedMessage(message.GetRecipient(), payloadDigest, encodedPayload, encodedIv, encodedSecret);
            return encodedMessage;
        }

        private string createNonce() {
            byte[] nonce = createRandom(32);
            return Convert.ToBase64String(nonce);
        }

        private byte[] createRandom(int length) {
            using (var rng = RandomNumberGenerator.Create()) {
                var bytes = new byte[length];
                rng.GetBytes(bytes);
                return bytes;
            }
        }
    }
}
