namespace Minimax.Utilities
{
    public static class Define
    {
#region CLOUD_SAVE_KEYS
        public static readonly string DeckCloudKey = "decks";
        public static readonly string CurrentDeckIdCloudKey = "currentDeckId";
#endregion
        
#region CACHE_KEYS
        public static readonly string DeckDtoCollectionCacheKey = "deckDtoCollection";
        public static readonly string CurrentDeckNameCacheKey = "currentDeckName";
        public static readonly string CurrentDeckIdCacheKey = "currentDeckId";
#endregion

#region NETWORK_SETTINGS
        public static readonly int TimeOutSeconds = 5;
        public static readonly int MaxConnectPayloadSize = 1024;
        public static readonly int TimeBeforeFirstAttempt = 1;
        public static readonly int TimeBetweenReconnectionAttempts = 5;
        public static readonly int MaxReconnectionAttempts = 2;
        public static readonly string MatchMakingQueueName = "default-queue";
#endregion

#region GamePlay
        public static readonly int MaxConnectedPlayers = 2;
        public static readonly int MaxHandCardCount = 10;
#endregion
    }
}