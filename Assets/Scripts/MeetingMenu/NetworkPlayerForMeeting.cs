using MLAPI.Serialization;

namespace MeetingMenu {
    public struct NetworkPlayerForMeeting: INetworkSerializable {
        public ulong clientId;
        public string playerName;
        public bool alive;

        public NetworkPlayerForMeeting(ulong clientId, string playerName, bool alive) {
            this.clientId = clientId;
            this.playerName = playerName;
            this.alive = alive;
        }

        public void NetworkSerialize(NetworkSerializer serializer) {
            serializer.Serialize(ref clientId);
            serializer.Serialize(ref playerName);
            serializer.Serialize(ref alive);
        }
    }
}