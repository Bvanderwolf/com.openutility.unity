using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenUtility.Exceptions
{
    public static class ThrowIf
    {
        public static void NullOrEmpty(string str)
        {
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException("string is null or empty.");
        }

        public static void Empty(string str)
        {
            if (str is { Length: 0 })
                throw new ArgumentException("string is empty.");
        }

        public static void Null(Object obj)
        {
            if (obj == null)
                throw new NullReferenceException("System.Object reference not set to an instance of an object.");
        }

        public static void Negative(Single single)
        {
            if (single < 0)
                throw new ArgumentOutOfRangeException($"Value {single} is negative.");
        }

        public static void Negative(Int32 integer)
        {
            if (integer < 0)
                throw new ArgumentOutOfRangeException($"Value {integer} is negative.");
        }

        public static void Zero(Single single)
        {
            if (single == 0)
                throw new ArgumentOutOfRangeException("Value is zero.");
        }

        public static void Zero(Int32 integer)
        {
            if (integer == 0)
                throw new ArgumentOutOfRangeException("Value is zero.");
        }

        public static void ZeroOrNegative(Single single)
        {
            Zero(single);
            Negative(single);
        }

        public static void OutOfBounds<T>(T[] array, int index)
        {
            if (index < 0 || index >= array.Length)
                throw new IndexOutOfRangeException(
                    $"Index {index} is out of bounds for array of length {array.Length}.");
        }

        public static void EmptyArray<T>(T[] array)
        {
            if (array.Length == 0)
                throw new ArgumentException("Array is empty.");
        }

        public static void EmptyCollection(ICollection collection)
        {
            if (collection.Count == 0)
                throw new ArgumentException("Collection is empty.");
        }

        public static void EmptyEnumerable<T>(IEnumerable<T> collection)
        {
            if (!collection.Any())
                throw new ArgumentException("Collection is empty.");
        }
    }
}
