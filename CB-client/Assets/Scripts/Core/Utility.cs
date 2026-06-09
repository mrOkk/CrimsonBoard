using System.Collections.Generic;

namespace CrimsonBoard
{
    public static class Utility
    {
        public static void Shuffle<T>(this IList<T> list, System.Random rng)
        {
            var n = list.Count;

            for (var i = 0; i < n; i++)
            {
                var j = rng.Next(i, n);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        public static void Shuffle<T>(this T[] arr, System.Random rng)
        {
            var n = arr.Length;

            for (var i = 0; i < n; i++)
            {
                var j = rng.Next(i, n);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
        }
    }
}
