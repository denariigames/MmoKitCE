using System;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public struct WeightedRandomizerItem<T>
    {
        public T item;
        public float weight;
    }

    /// <summary>
    /// Static class to improve readability
    /// Example:
    /// <code>
    /// var selected = WeightedRandomizer.From(weights).TakeOne();
    /// </code>
    /// </summary>
    public static class WeightedRandomizer
    {
        public static WeightedRandomizer<T> From<T>(List<WeightedRandomizerItem<T>> items, float noResultWeight = 0f)
        {
            return new WeightedRandomizer<T>(items, noResultWeight);
        }
    }

    public class WeightedRandomizer<T>
    {
        private static Random _random = new Random();
        public float NoResultWeight { get; set; }
        private List<WeightedRandomizerItem<T>> _items;

        /// <summary>
        /// Instead of calling this constructor directly,
        /// consider calling a static method on the WeightedRandomizer (non-generic) class
        /// for a more readable method call, i.e.:
        /// 
        /// <code>
        /// var selected = WeightedRandomizer.From(weights).TakeOne();
        /// </code>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="noResultWeight"></param>
        public WeightedRandomizer(List<WeightedRandomizerItem<T>> items, float noResultWeight = 0f)
        {
            _items = items;
            NoResultWeight = noResultWeight;
        }

        /// <summary>
        /// Randomizes one item
        /// </summary>
        /// <param name="seed"></param>
        /// <returns>The randomized item.</returns>
        public T TakeOne(int seed = 0)
        {
            if (_items == null || _items.Count <= 0)
                return default;

            // Sums all spawn rates
            float totalWeight = 0;
            foreach (WeightedRandomizerItem<T> item in _items)
            {
                totalWeight += item.weight;
            }

            // Randomizes a number from Zero to Sum
            Random random = _random;
            if (seed != 0)
                random = new Random(seed);

            float randomValue = (float)(random.NextDouble() * (totalWeight + NoResultWeight));
            foreach (WeightedRandomizerItem<T> item in _items)
            {
                if (randomValue < item.weight)
                    return item.item;
                randomValue -= item.weight;
            }

            return default; // Fallback if something goes wrong
        }
    }
}
