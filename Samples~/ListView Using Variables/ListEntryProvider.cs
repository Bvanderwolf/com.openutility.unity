using System;
using UnityEngine;
using UnityEngine.Pool;

namespace OpenUtility.Samples.Data
{
    public enum ListEntrySortType
    {
        DATE,
        TITLE,
    }
    
    public enum ListEntryType
    {
        UNPUBLISHED,
        PUBLISHED,
    }
    
    [Serializable]
    public struct ListEntryData
    {
        public string title;
        public string timestamp;
        public bool published;
    }
    
    public class ListEntryProvider : MonoBehaviour
    {
        [SerializeField]
        private ListEntryData[] _data;

        public ListEntryData[] GetData(ListEntryType type)
        {
            bool gatherPublished = type == ListEntryType.PUBLISHED;
            
            using var pool = ListPool<ListEntryData>.Get(out var list);
            
            for (int i = 0; i < _data.Length; i++)
                if (_data[i].published == gatherPublished)
                    list.Add(_data[i]);

            return (list.ToArray());
        }
    }
}