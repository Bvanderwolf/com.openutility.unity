using System;
using OpenUtility.Data.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace OpenUtility.Samples.Data
{
    public class ListManager : MonoBehaviour
    {
        [Header("Project References")]
        [SerializeField]
        private ScriptablePool _listEntryPool;

        [SerializeField]
        private ScriptablePool _emptyEntryPool;

        [Header("Settings")]
        [SerializeField]
        private int _fullListCount;

        [SerializeField]
        private int _maxSearchResults = 5;

        [Header("Scene References")]
        [SerializeField]
        private ScrollRect _scrollView;

        [SerializeField]
        private TMP_Dropdown _sortDropdown;

        [SerializeField]
        private TMP_Dropdown _typeDropdown;

        private void Awake()
        {
            _listEntryPool.SetParent(_scrollView.content);
            _emptyEntryPool.SetParent(_scrollView.content);
        }

        private void Start()
        {
            RefreshListFromTypeDropdown((int)ListEntryType.UNPUBLISHED);
        }

        public void SortAndEnableTilesFromSearchQuery(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                OrderFromSortDropdown(_sortDropdown.value);
            }
            else
            {
               int queryCount = _listEntryPool.References.SortFromQuery(query, _maxSearchResults);
               
               _listEntryPool.References.OrderBySiblingIndex();
               _listEntryPool.References.SetActive(false, skip: queryCount);
               
               CreateLeftoverEmptyEntriesFromSorting(queryCount);
            }
        }

        public void OrderFromSortDropdown(int value)
        {
            switch ((ListEntrySortType)value)
            {
                case ListEntrySortType.TITLE:
                    OrderByTitle();
                    break;
                
                case ListEntrySortType.DATE:
                    OrderByDate();
                    break;
            }
        }

        public void OrderByTitle()
        {
            _listEntryPool.References.SortByName();
            _listEntryPool.References.OrderBySiblingIndex();
        }

        public void OrderByDate()
        {
            ListEntryProvider provider = GetComponent<ListEntryProvider>();
            ListEntryData[] data = provider.GetData((ListEntryType)_typeDropdown.value);
            
            using var pool = ListPool<ListEntryData>.Get(out var list);
            list.AddRange(data);
            list.Sort((lhs, rhs) =>
            {
                try
                {
                    DateTime lhd = DateTime.ParseExact(lhs.timestamp, "yyyy-MM-dd HH:mm:ss", null);
                    DateTime rhd = DateTime.ParseExact(rhs.timestamp, "yyyy-MM-dd HH:mm:ss", null);

                    return DateTime.Compare(rhd, lhd);
                }
                catch (FormatException ex)
                {
                    Debug.LogError($"Error parsing date string: {ex.Message}");
                    throw;
                }
            });

            ListEntryData[] sorted = list.ToArray();
            
            ClearListEntries();
            CreateListEntries(sorted);
        }

        public void RefreshListFromTypeDropdown(int value)
        {
            ListEntryProvider provider = GetComponent<ListEntryProvider>();
            ListEntryData[] data = provider.GetData((ListEntryType)value);
            
            ClearListEntries();
            CreateListEntries(data);
        }

        private void ClearListEntries()
        {
            _listEntryPool.References.Release();
            _emptyEntryPool.References.Release();
        }

        private void CreateLeftoverEmptyEntriesFromSorting(int queryCount)
        {
            _emptyEntryPool.References.Release();
            
            for (int i = 0; i < (_fullListCount - queryCount); i++)
                _emptyEntryPool.Get();
        }

        private void CreateListEntries(ListEntryData[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                ListEntry entry = _listEntryPool.GetComponent<ListEntry>();
                entry.SetData(data[i]);
            }

            int leftOverEntries = _fullListCount - data.Length;
            for (int i = 0; i < leftOverEntries; i++)
                _emptyEntryPool.Get();
        }

        private void OnRectTransformDimensionsChange()
        {
            Debug.Log("Screensize changed, adjusting entry size");
            
            CalculateEntrySize();
        }

        private void CalculateEntrySize()
        {
            float contentHeight = _scrollView.GetComponent<RectTransform>().rect.height;
            float contentSpacing = _scrollView.content.GetComponent<VerticalLayoutGroup>().spacing;
            float totalContentSpacing = contentSpacing * (_fullListCount - 1);

            float height = (contentHeight - totalContentSpacing ) / _fullListCount;

            foreach (ListEntry entry in _listEntryPool.References.GetComponents<ListEntry>())
                entry.SetHeight(height);
            
            foreach (EmptyEntry entry in _emptyEntryPool.References.GetComponents<EmptyEntry>())
                entry.SetHeight(height);
        }
    }
}
