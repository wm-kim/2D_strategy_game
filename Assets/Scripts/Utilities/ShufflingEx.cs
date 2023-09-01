using System.Collections.Generic;

namespace Minimax.Utilities
{
    public static class ShufflingEx
    {
        private static System.Random rng = new System.Random();
        
        public static void Shuffle<T>(this IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                (list[k], list[n]) = (list[n], list[k]);
            }  
        }
        
        public static void Shuffle<T>(this T[] array)  
        {  
            int n = array.Length;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                (array[k], array[n]) = (array[n], array[k]);
            }  
        }
    }
}