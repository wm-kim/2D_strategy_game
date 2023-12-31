namespace Minimax.GamePlay
{
    public static class IDFactory
    {
        private static int Count;

        public static int GetUniqueID()
        {
            // Count++ has to go first, otherwise - unreachable code.
            Count++;
            return Count;
        }

        public static void ResetIDs()
        {
            Count = 0;
        }
    }

    public interface IIdentifiable
    {
        int UID { get; }
    }
}