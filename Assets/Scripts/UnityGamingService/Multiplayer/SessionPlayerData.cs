
namespace Minimax.UnityGamingService.Multiplayer
{
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName;
        public int PlayerNumber;
        
        public SessionPlayerData(ulong clientId, string name, int playerNumber, bool isConnected = false)
        {
            ClientID = clientId;
            PlayerName = name;
            PlayerNumber = playerNumber;
            IsConnected = isConnected;
        }
        
        public void Reinitialize() { }

        public bool IsConnected { get; set; }
        public ulong ClientID { get; set; }
    }
}
