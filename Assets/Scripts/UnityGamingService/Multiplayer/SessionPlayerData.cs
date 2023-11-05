namespace Minimax.UnityGamingService.Multiplayer
{
    public struct SessionPlayerData : ISessionPlayerData
    {
        public string PlayerName;

        /// <summary>
        /// 플레이어 번호, 현재 어떤 플레이어의 턴인지 또는 카드나 유닛의 소유권을 확인할 때 사용합니다.
        /// </summary>
        public int PlayerNumber;

        public SessionPlayerData(ulong clientId, string name, int playerNumber, bool isConnected = false)
        {
            ClientID     = clientId;
            PlayerName   = name;
            PlayerNumber = playerNumber;
            IsConnected  = isConnected;
        }

        public void Reinitialize()
        {
        }

        public bool  IsConnected { get; set; }
        public ulong ClientID    { get; set; }
    }
}