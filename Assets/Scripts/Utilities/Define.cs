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
    }
}