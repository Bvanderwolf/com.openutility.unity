using System;
using System.Collections.Generic;
using OpenUtility.Data;
using OpenUtility.Data.Pooling;
using UnityEngine;
using UnityEngine.Pool;

namespace OpenUtility.Samples.Data
{
    public static class PoolGameObjectListExtensions
    {
        /// <summary>
        /// Sorts the list using a fuzzy search algorithm to determine the order of the game objects in the list based on the provided query.
        /// Note that this will not change the sibling index of the game objects. Use OrderBySiblingIndex() after this if you want to reorder them in the hierarchy as well.
        /// </summary>
        public static int SortFromQuery(this PoolGameObjectList poolGameObjectList, string query, int maxResults)
        {
            var instance = (List<PoolGameObject>)poolGameObjectList.GetValue();
            var results = FuzzySearch.Search(instance, query, maxResults);

            for (int i = 0; i < results.Length; i++)
            {
                PoolGameObject result = results[i];
                int positionInList = instance.IndexOf(result);

                if (positionInList == i) 
                    continue;

                PoolGameObject temp = instance[i];
                instance[i] = result;
                instance[positionInList] = temp;
            }

            return (results.Length);
        }
        
        /// <summary>
        /// Sorts the list by name of game object. Note that this will not change the sibling index of the game objects.
        /// Use OrderBySiblingIndex() after this if you want to reorder them in the hierarchy as well.
        /// </summary>
        public static void SortByName(this PoolGameObjectList poolGameObjectList) => poolGameObjectList.Sort((lhs, rhs) => string.CompareOrdinal(lhs.name, rhs.name));

        /// <summary>
        /// Sorts the list using the provided comparison. Note that this will not change the sibling index of the game objects.
        /// Use OrderBySiblingIndex() after this if you want to reorder them in the hierarchy
        /// </summary>
        public static void Sort(this PoolGameObjectList poolGameObjectList, Comparison<PoolGameObject> comparison)
        {
            IList<PoolGameObject> instance = poolGameObjectList.GetValue();

            if (instance is List<PoolGameObject> list)
            {
                list.Sort(comparison);
            }
            else
            {
                using PooledObject<List<PoolGameObject>> pool = ListPool<PoolGameObject>.Get(out list);
                
                list.Capacity = instance.Count;
                list.AddRange(instance);
                list.Sort(comparison);
                
                instance.Clear();
                
                for (int i = 0; i < list.Count; i++)
                    instance.Add(list[i]);
            }
        }

        /// <summary>
        /// Changes the sibling index of each game object to match its index in the list.
        /// This is useful for sorted ui elements that need to be ordered in the hierarchy to render correctly.
        /// </summary>
        public static void OrderBySiblingIndex(this PoolGameObjectList poolGameObjectList)
        {
            IList<PoolGameObject> instance = poolGameObjectList.GetValue();
            
            if (instance.Count == 0)
                return;

            for (int i = 0; i < instance.Count; i++)
                instance[i].transform.SetSiblingIndex(i);
        }

        /// <summary>
        /// Sets the active state of each game object in the list. Optionally, you can specify a number of elements
        /// to skip at the start of the list and/or a count of how many elements to set active after skipping. Skipped
        /// elements are set to the opposite of the provided active state.
        /// </summary>
        public static void SetActive(this PoolGameObjectList poolGameObjectList, bool active, int? skip = null, int? count = null)
        {
            IList<PoolGameObject> instance = poolGameObjectList.GetValue();

            int startIndex = skip ?? 0;
            int endIndex = count.HasValue ? Math.Min(startIndex + count.Value, instance.Count) : instance.Count;

            for (int i = 0; i < endIndex; i++)
            {
                bool skipped = i < startIndex;
                bool state = skipped ? !active : active;
                
                instance[i].gameObject.SetActive(state);
            }
        }
        
        /// <summary>
        /// Returns components of type T for each game object in the list that has one.
        /// </summary>
        public static T[] GetComponents<T>(this PoolGameObjectList poolGameObjectList) where T : Component
        {
            IList<PoolGameObject> instance = poolGameObjectList.GetValue();

            using PooledObject<List<T>> pool = ListPool<T>.Get(out List<T> list);
            list.Capacity = instance.Count;

            for (int i = 0; i < instance.Count; i++)
                if (instance[i].TryGetComponent(out T component))
                    list.Add(component);

            return (list.ToArray());
        }
    }
}
