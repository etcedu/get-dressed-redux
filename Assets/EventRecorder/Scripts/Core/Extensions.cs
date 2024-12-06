using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SimcoachGames.EventRecorder
{
    public static class Extensions
    {
        public static T GetRandom<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static T GetRandom<T>(this List<T> array)
        {
            return array[Random.Range(0, array.Count)];
        }

        //WTP: This only works for axis aligned boxes I think?
        public static Vector3 GetRandomPointInBounds(this Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public static float Map(this float val, float originalMin, float originalMax, float newMin, float newMax)
        {
            return newMin + (val - originalMin) * (newMax - newMin) / (originalMax - originalMin);
        }

        private static System.Random rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}