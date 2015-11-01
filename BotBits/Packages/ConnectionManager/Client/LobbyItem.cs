using System.Diagnostics;
using System.Threading.Tasks;
using PlayerIOClient;

namespace BotBits
{
    [DebuggerDisplay("{Id}: {Name}")]
    public sealed class LobbyItem
    {
        private readonly ILoginClient _client;

        public LobbyItem(ILoginClient client, RoomInfo roomInfo)
        {
            this._client = client;


            this.Id = roomInfo.Id;
            this.Online = roomInfo.OnlineUsers;
            foreach (var data in roomInfo.RoomData)
            {
                switch (data.Key)
                {
                    case "name":
                        this.Name = data.Value;
                        break;
                    case "plays":
                        this.Plays = int.Parse(data.Value);
                        break;
                    case "woots":
                        this.Woots = int.Parse(data.Value);
                        break;
                    case "owned":
                        this.Owned = bool.Parse(data.Value);
                        break;
                    case "needskey":
                        this.HasCode = (data.Value == "yep");
                        break;
                    case "IsFeatured":
                        this.Featured = bool.Parse(data.Value);
                        break;
                }
            }
        }

        public int Online { get; private set; }
        public string Id { get; }
        public string Name { get; }
        public int Plays { get; private set; }
        public int Woots { get; private set; }
        public bool Owned { get; private set; }
        public bool HasCode { get; private set; }
        public bool Featured { get; private set; }

        public void JoinRoom()
        {
            this.JoinRoomAsync().Wait();
        }

        public Task JoinRoomAsync()
        {
            return this._client.JoinRoomAsync(this.Id);
        }
    }
}