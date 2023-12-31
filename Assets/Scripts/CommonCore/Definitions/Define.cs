namespace Minimax.Definitions
{
    public static class Define
    {
        #region AssetLabels

        public static readonly string CommonPopupAssetLabel = "CommonPopup";

        #endregion

        #region CLOUD_KEYS

        public static readonly string DeckCloud = "decks";

        #endregion

        #region CACHE_KEYS

        public static readonly string DeckDtoCollectionCache = "deckDtoCollection";
        public static readonly string CurrentDeckNameCache   = "currentDeckName";
        public static readonly string CurrentDeckIdCache     = "currentDeckId";

        #endregion

        #region POPUP_KEYS

        public static readonly string InvalidDeckPopup          = "invalidDeck";
        public static readonly string ExitDeckBuilderPopup      = "exitDeckBuilder";
        public static readonly string FindingMatchPopup         = "findingMatch";
        public static readonly string SavingDeckPopup           = "savingDeck";
        public static readonly string SelectDeckPopup           = "selectDeck";
        public static readonly string DeleteDeckPopup           = "deleteDeck";
        public static readonly string ReconnectingPopup         = "reconnecting";
        public static readonly string PlayerLostConnectionPopup = "playerLostConnection";
        public static readonly string ServerDisconnectedPopup   = "serverDisconnected";

        #endregion

        #region NETWORK_SETTINGS

        public static readonly int    WebTimeOutSeconds               = 30;
        public static readonly int    MaxConnectPayloadSize           = 1024;
        public static readonly int    TimeBeforeFirstAttempt          = 1;
        public static readonly int    TimeBetweenReconnectionAttempts = 4;
        public static readonly int    MaxReconnectionAttempts         = 2;
        public static readonly int    ServerWaitMsForClientReconnect  = 25000;
        public static readonly string MatchMakingQueueName            = "default-queue";
        public static readonly int    MaxConnectedPlayers             = 2;

        #endregion

        #region GamePlay

        public static readonly int MapSize              = 11;
        public static readonly int MaxHandCardCount     = 10;
        public static readonly int InitialHandCardCount = 3;

        #endregion

        #region SortingLayer

        public static readonly string MapOverlay = "MapOverlay";

        #endregion
    }
}