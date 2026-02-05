using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OpenUtility.Data
{
    [Serializable]
    public struct IntRange
    {
        [SerializeField]
        private int _from;

        [SerializeField]
        private int _to;
        
        public int From => _from;
        public int To => _to;
        public int Length => Mathf.Abs(_to - _from);

        public bool IsValid() => _to != _from;

        public IntRange(int from, int to)
        {
            _from = from;
            _to = to;
        }

        /// <summary>
        /// Returns the percentage the given value is from 'to' to 'from'
        /// </summary>
        public float GetPercentage(int value) => Mathf.InverseLerp(_from, _to, value);

        /// <summary>
        /// Returns the value from 'to' to 'from' based on given percentage
        /// </summary>
        public int GetValue(float percentage) => Mathf.RoundToInt(Mathf.Lerp(_from, _to, percentage));

        /// <summary>
        /// Returns a random value in this range.
        /// </summary>
        public int GetRandom() => Random.Range(_from, _to);

        /// <summary>
        /// Maps a value from this range to a given one.
        /// </summary>
        public int MapTo(IntRange target, int value) => Map(this, target, value);

        /// <summary>
        /// Maps a value from one float range to another.
        /// </summary>
        public static int Map(IntRange from, IntRange to, int value)
        {
            float percentage = Mathf.InverseLerp(from._from, from._to, value);
            return (Mathf.RoundToInt(Mathf.Lerp(to._from, to._to, percentage)));
        }
    }
}
