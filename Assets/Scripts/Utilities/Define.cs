namespace Minimax.Utilities
{
    public static class Define
    {
#region CLOUD_KEYS
        public static readonly string DeckCloudKey = "decks";
#endregion
        
#region CACHE_KEYS
        public static readonly string DeckDtoCollectionCacheKey = "deckDtoCollection";
        public static readonly string CurrentDeckNameCacheKey = "currentDeckName";
        public static readonly string CurrentDeckIdCacheKey = "currentDeckId";
#endregion

#region POPUP_KEYS
        public static readonly string InvalidDeckPopup = "invalidDeck";
        public static readonly string GameStartedPopup = "gameStarted";
        public static readonly string ExitDeckBuilderPopup = "exitDeckBuilder";
        public static readonly string FindingMatchPopup = "findingMatch";
        public static readonly string SavingDeckPopup = "savingDeck";
        public static readonly string SelectDeckPopup = "selectDeck";
        public static readonly string DeleteDeckPopup = "deleteDeck";
#endregion

#region NETWORK_SETTINGS
        public static readonly int TimeOutSeconds = 30;
        public static readonly int MaxConnectPayloadSize = 1024;
        public static readonly int TimeBeforeFirstAttempt = 1;
        public static readonly int TimeBetweenReconnectionAttempts = 5;
        public static readonly int MaxReconnectionAttempts = 2;
        public static readonly string MatchMakingQueueName = "default-queue";
        public static readonly int MaxConnectedPlayers = 2;
#endregion

#region GamePlay
        public static readonly int MaxHandCardCount = 10;
#endregion
    }
}