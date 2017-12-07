namespace Satochat.Client.Service.Api {
    public class Credential {
        public string Username { get; private set; }
        public string Password { get; private set; }

        public Credential(string username, string password) {
            Username = username;
            Password = password;
        }
    }
}
