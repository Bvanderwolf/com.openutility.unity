using System;
using UnityEngine;

namespace OpenUtility.Data
{
    [Serializable]
    public struct FloatRange
    {
        public static FloatRange ZeroOne { get; } = new(0.0f, 1.0f);

        [SerializeField]
        private float _from;

        [SerializeField]
        private float _to;
        
        public float From => _from;
        public float To => _to;
        public float Length => Mathf.Abs(_to - _from);

        public bool IsValid() => _to != _from;

        public FloatRange(float from, float to)
        {
            _from = from;
            _to = to;
        }

        public float Map(FloatRange from, FloatRange to, float value)
        {
            float percentage = Mathf.InverseLerp(from._from, from._to, value);
            return (Mathf.Lerp(to._from, to._to, percentage));
        }

        /// <summary>
        /// Returns the percentage the given value is from 'to' to 'from'
        /// </summary>
        public float GetPercentage(float value) => Mathf.InverseLerp(_from, _to, value);
        
        /// <summary>
        /// Returns the value from 'to' to 'from' based on given percentage
        /// </summary>
        public float GetValue(float percentage) => Mathf.Lerp(_from, _to, percentage);

        /// <summary>
        /// Maps given value based on this range to a (0,1) range.
        /// </summary>
        public float MapTo01(float value) => Map(this, ZeroOne, value);
        
        /// <summary>
        /// Maps given value to this range, given that it is between 0 and 1.
        /// </summary>
        public float MapFrom01(float value) => Map(ZeroOne, this, value);
    }
}
