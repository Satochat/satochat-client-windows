namespace Satochat.Client.Service {
    public class Friend {
        public string Uuid { get; }
        public string Nickname { get; set; }
        public string DisplayNickname => string.IsNullOrEmpty(Nickname) ? Uuid : Nickname;

        public Friend(string uuid) {
            Uuid = uuid;
        }

        public Friend(string uuid, string nickname) {
            Uuid = uuid;
            Nickname = nickname;
        }
    }
}
