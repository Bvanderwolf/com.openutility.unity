using System;
using System.Collections.Generic;
using OpenUtility.Data;

public enum SearchType
{
    /// <summary>
    /// Does not allow for results with a length bigger than the search query.
    /// </summary>
    AutoComplete,
    
    /// <summary>
    /// Allows for any result as long as it matches the query closely.
    /// </summary>
    SearchQuery
}

public static class FuzzySearch
{
    public static int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) 
            return b.Length;
        
        if (string.IsNullOrEmpty(b)) 
            return a.Length;

        int[,] costs = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++) costs[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) costs[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                costs[i, j] = Math.Min(
                    Math.Min(costs[i - 1, j] + 1, costs[i, j - 1] + 1),
                    costs[i - 1, j - 1] + cost);
            }
        }

        return costs[a.Length, b.Length];
    }

    /// <summary>
    /// Returns a list of string that closely match the query using Levenshtein Distance.
    /// </summary>
    /// <param name="items">The items to pick from.</param>
    /// <param name="query">The query to base the results on.</param>
    /// <param name="maxResults">The maximum amount of results to take.</param>
    /// <param name="errorMargin">Represents how many incorrect characters it accepts relative to the length of the item.
    /// For example: 3 means to accept 1 incorrect character every 3 characters.</param>
    /// <param name="searchType">The search type to use for the search.</param>
    /// <returns></returns>
    public static string[] Search(List<string> items, string query, int maxResults, int errorMargin = 3, SearchType searchType = SearchType.AutoComplete)
    {
        if (string.IsNullOrEmpty(query))
            return (items.ToArray());
        
        if (maxResults < 1)
            return (Array.Empty<string>());

        string queryToLower = query.ToLower();
        int maxAllowedDistance = Math.Max(1, query.Length / errorMargin);
        SortedList<int, List<string>> sorted = new SortedList<int, List<string>>(items.Count);

        foreach (string item in items)
        {
            if (searchType == SearchType.AutoComplete && query.Length > item.Length)
                continue;
            
            int distance = GetDistanceBetweenItemAndQuery(item.ToLower(), queryToLower);
            if (distance > maxAllowedDistance)
                continue;

            if (!sorted.ContainsKey(distance))
                sorted[distance] = new List<string>();
            
            sorted[distance].Add(item);
        }

        List<string> results = new List<string>(maxResults);
        IList<List<string>> values = sorted.Values;
        
        for (int i = 0; i < values.Count; i++)
        {
            for (int j = 0; j < values[i].Count; j++)
            {
                results.Add(values[i][j]);

                if (results.Count == maxResults)
                    return (results.ToArray());
            }
        }

        return (results.ToArray());
    }

    /// <summary>
    /// Returns a list of string that closely match the query using Levenshtein Distance.
    /// Implement the IObjectIdentifier interface on your items to use this method.
    /// </summary>
    /// <param name="items">The items implementing IObjectIdentifier to pick from.</param>
    /// <param name="query">The query to base the results on.</param>
    /// <param name="maxResults">The maximum amount of results to take.</param>
    /// <param name="errorMargin">Represents how many incorrect characters it accepts relative to the length of the item.
    /// For example: 3 means to accept 1 incorrect character every 3 characters.</param>
    /// <param name="searchType">The search type to use for the search.</param>
    /// <returns></returns>
    public static T[] Search<T>(List<T> items, string query, int maxResults, int errorMargin = 3, SearchType searchType = SearchType.AutoComplete) where T : IObjectIdentifier
    {
        if (string.IsNullOrEmpty(query))
            return (items.ToArray());
        
        if (maxResults < 1)
            return (Array.Empty<T>());
        
        string queryToLower = query.ToLower();
        int maxAllowedDistance = Math.Max(1, query.Length / errorMargin);
        SortedList<int, List<T>> sorted = new SortedList<int, List<T>>(items.Count);

        foreach (T item in items)
        {
            string identifier = item.Identifier;
            if (searchType == SearchType.AutoComplete && query.Length > identifier.Length)
                continue;
            
            int distance = GetDistanceBetweenItemAndQuery(identifier.ToLower(), queryToLower);
            if (distance > maxAllowedDistance)
                continue;

            if (!sorted.ContainsKey(distance))
                sorted[distance] = new List<T>();
            
            sorted[distance].Add(item);
        }

        List<T> results = new List<T>(maxResults);
        IList<List<T>> values = sorted.Values;
        for (int i = 0; i < values.Count; i++)
        {
            for (int j = 0; j < values[i].Count; j++)
            {
                results.Add(values[i][j]);

                if (results.Count == maxResults)
                    return (results.ToArray());
            }
        }

        return (results.ToArray());
    }
    
    /// <summary>
    /// Returns a list of string that closely match the query using Levenshtein Distance.
    /// Implement the IObjectIdentifier interface on your items to use this method.
    /// </summary>
    /// <param name="items">The items implementing IObjectIdentifier to pick from.</param>
    /// <param name="query">The query to base the results on.</param>
    /// <param name="maxResults">The maximum amount of results to take.</param>
    /// <param name="errorMargin">Represents how many incorrect characters it accepts relative to the length of the item.
    /// For example: 3 means to accept 1 incorrect character every 3 characters.</param>
    /// <param name="searchType">The search type to use for the search.</param>
    /// <returns></returns>
    public static T[] Search<T>(T[] items, string query, int maxResults, int errorMargin = 3, SearchType searchType = SearchType.AutoComplete) where T : IObjectIdentifier
    {
        if (string.IsNullOrEmpty(query))
            return (items);
        
        if (maxResults < 1)
            return (Array.Empty<T>());
        
        string queryToLower = query.ToLower();
        int maxAllowedDistance = Math.Max(1, query.Length / errorMargin);
        SortedList<int, List<T>> sorted = new SortedList<int, List<T>>(items.Length);

        foreach (T item in items)
        {
            string identifier = item.Identifier;
            if (searchType == SearchType.AutoComplete && query.Length > identifier.Length)
                continue;
            
            int distance = GetDistanceBetweenItemAndQuery(identifier.ToLower(), queryToLower);
            if (distance > maxAllowedDistance)
                continue;

            if (!sorted.ContainsKey(distance))
                sorted[distance] = new List<T>();
            
            sorted[distance].Add(item);
        }

        List<T> results = new List<T>(maxResults);
        IList<List<T>> values = sorted.Values;
        for (int i = 0; i < values.Count; i++)
        {
            for (int j = 0; j < values[i].Count; j++)
            {
                results.Add(values[i][j]);

                if (results.Count == maxResults)
                    return (results.ToArray());
            }
        }

        return (results.ToArray());
    }

    private static int GetDistanceBetweenItemAndQuery(string item, string query)
    {
        if (item == query)
            return (0);

        if (item.StartsWith(query))
            return (1);

        return (LevenshteinDistance(item, query));
    }
}
