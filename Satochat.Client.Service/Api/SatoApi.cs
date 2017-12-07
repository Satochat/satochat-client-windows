using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Satochat.Shared.ViewModels;

namespace Satochat.Client.Service.Api {
    public class SatoApi : BaseSatoApi {
        private Credential _credential;
        
        public SatoApi(string endpoint) : base(endpoint) {
        }

        public void SetCredential(Credential credential) {
            _credential = credential;
        }

        public async Task<KeyViewModel.GetPublicKeyResult> GetPublicKeyAsync(string uuid) {
            return await parseGetPublicKeyReqAsync(await getPublicKeyReqAsync(uuid));
        }

        private async Task<HttpResponseMessage> getPublicKeyReqAsync(string uuid) {
            var parameters = new KeyViewModel.GetPublicKey(uuid).ToParameters();
            return await getAsync("key/public", parameters);
        }

        private async Task<KeyViewModel.GetPublicKeyResult> parseGetPublicKeyReqAsync(HttpResponseMessage res) {
            string json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<KeyViewModel.GetPublicKeyResult>(json);
            return obj;
        }

        public async Task PutPublicKeyAsync(string key) {
            await putPublicKeyReqAsync(key);
        }

        private async Task<HttpResponseMessage> putPublicKeyReqAsync(string key) {
            var json = JsonConvert.SerializeObject(new KeyViewModel.PutPublicKey(key));
            return await postAsync("key/public", json);
        }

        public async Task<WhoamiViewModel.ViewResult> WhoamiAsync() {
            return await parseWhoamiReqAsync(await whoamiReqAsync());
        }

        private async Task<HttpResponseMessage> whoamiReqAsync() {
            return await getAsync("whoami");
        }

        private async Task<WhoamiViewModel.ViewResult> parseWhoamiReqAsync(HttpResponseMessage res) {
            string json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<WhoamiViewModel.ViewResult>(json);
            return obj;
        }

        protected override Credential getCredential() {
            return _credential;
        }
        
        public async Task<MessageViewModel.GetMessageResult> GetMessageAsync(string uuid) {
            return await parseGetMessageReqAsync(await getMessageReqAsync(uuid));
        }

        private async Task<HttpResponseMessage> getMessageReqAsync(string uuid) {
            return await getAsync("message/" + uuid);
        }

        private async Task<MessageViewModel.GetMessageResult> parseGetMessageReqAsync(HttpResponseMessage res) {
            string json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<MessageViewModel.GetMessageResult>(json);
            return obj;
        }
        
        public async Task<ConversationViewModel.GetConversationResult> GetConversationAsync(string uuid) {
            return await parseGetConversationReqAsync(await getConversationReqAsync(uuid));
        }

        private async Task<HttpResponseMessage> getConversationReqAsync(string uuid) {
            return await getAsync("conversation/" + uuid);
        }

        private async Task<ConversationViewModel.GetConversationResult> parseGetConversationReqAsync(HttpResponseMessage res) {
            string json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ConversationViewModel.GetConversationResult>(json);
            return obj;
        }

        public async Task ListenEvents(SatoApiEventHandler handler, CancellationToken cancellationToken) {
            List<string> lines = new List<string>();
            var res = await getAsync("event", cancellationToken: cancellationToken);
            var buffer = new byte[1];
            var lineBuffer = new MemoryStream();

            using (var stream = await res.Content.ReadAsStreamAsync()) {
                while (true) {
                    cancellationToken.ThrowIfCancellationRequested();
                    int read = await stream.ReadAsync(buffer, 0, 1, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();

                    if (read == 0) {
                        // Maybe we got disconnected, so we'll exit for now and retry later
                        break;
                    }

                    if (buffer[0] == '\n') {
                        string line = Encoding.UTF8.GetString(lineBuffer.ToArray());
                        lineBuffer.SetLength(0);
                        if (line == "") {
                            string data = String.Join("\n", lines);
                            var model = JsonConvert.DeserializeObject<EventViewModel.PushEvent>(data);
                            await handler.RaiseEventReceivedEvent(this, new SatoApiEventHandler.EventReceivedEventArg(model.Uuid, model.Name, model.Data));
                            lines.Clear();
                        } else {
                            lines.Add(line);
                        }
                    } else {
                        lineBuffer.WriteByte(buffer[0]);
                    }
                }
            }
        }

        public async Task<MessageViewModel.SendMessageResult> SendMessageAsync(MessageViewModel.SendMessage model) {
            return await parseSendMessageReqAsync(await sendMessageReqAsync(model));
        }

        private async Task<HttpResponseMessage> sendMessageReqAsync(MessageViewModel.SendMessage model) {
            var json = JsonConvert.SerializeObject(model);
            return await postAsync("message", json);
        }

        private async Task<MessageViewModel.SendMessageResult> parseSendMessageReqAsync(HttpResponseMessage res) {
            string json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<MessageViewModel.SendMessageResult>(json);
            return obj;
        }

        public async Task NotifyMessageReceivedAsync(string uuid) {
            await postAsync("message/received/" + uuid);
        }

        public async Task NotifyEventReceivedAsync(string uuid) {
            await postAsync("event/received/" + uuid);
        }

        public async Task NotifyTypingInConversationAsync(string uuid) {
            await postAsync("conversation/typing/" + uuid);
        }
    }
}
