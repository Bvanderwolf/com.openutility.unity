using System;
using System.Globalization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace OpenUtility.Data.Newtonsoft
{
    public static class JValueUtility 
    {
        public static bool BoolValue(this JToken token)
        {
            JValue value = token as JValue;
            if (value == null)
                return (false);

            if (value.Type != JTokenType.Boolean)
                return (false);

            return ((bool)value.Value);
        }
        
        public static T EnumValue<T>(this JToken token) where T : Enum
        {
            JValue value = token as JValue;
            if (value == null)
                return default;

            if (value.Type != JTokenType.Integer && value.Type != JTokenType.String)
                return default;

            if (value.Type == JTokenType.Integer)
            {
                long integer = (long)value.Value;
                return (T)Enum.ToObject(typeof(T), integer);
            }

            if (value.Type == JTokenType.String)
            {
                string stringValue = value.Value.ToString();
                return (T)Enum.Parse(typeof(T), stringValue, true);
            }

            return default;
        }

        /// <summary>
        /// Returns the value of the given jvalue as an integer.
        /// </summary>
        public static long IntValue(this JToken token)
        {
            JValue value = token as JValue;
            if (value == null)
                return (0);

            if (value.Type == JTokenType.Float)
            {
                switch (value.Value)
                {
                    case float floatValue:
                        return (Convert.ToInt64(floatValue));
                
                    case double doubleValue:
                        return (Convert.ToInt64(doubleValue));
                }
            }

            if (value.Type != JTokenType.Integer)
                return (0);
                    
            return ((long)value.Value);
        }
        
        /// <summary>
        /// Returns the value of the given jvalue as a float.
        /// </summary>
        public static float FloatValue(this JToken token)
        {
            JValue value = token as JValue;
            if (value == null)
                return (0.0f);
            
            if (value.Type == JTokenType.Integer)
                return ((long)value.Value);
            
            if (value.Type != JTokenType.Float)
                return (0.0f);

            switch (value.Value)
            {
                case float floatValue:
                    return (floatValue);
                
                case double doubleValue:
                    return ((float)doubleValue);
            }

            return (0.0f);
        }

        /// <summary>
        /// Returns the value of the given jvalue as a string.
        /// </summary>
        public static string StringValue(this JToken token)
        {
            JValue value = token as JValue;
            if (value == null)
                return (string.Empty);

            if (value.Value == null)
                return (string.Empty);

            return (string)value.Value;
        }
        
        /// <summary>
        /// Tries returning the value of the given JToken as the given type.
        /// Returns false if an exception happened.
        /// </summary>
        public static bool TryGetValue<T>(this JToken token, out T value)
        {
            try
            {
                value = token.Value<T>();
                return (true);
            }
            catch (Exception e)
            {
                Debug.LogError($"Caught exception when trying to retrieve value: {e.Message}");
                value = default;
                return (false);
            }
        }

        public static bool AllowsForIncrease(this JValue value)
        {
            return value.Type == JTokenType.Integer || value.Type == JTokenType.Float || value.Type == JTokenType.String;
        }
        
        public static bool AllowsForDecrease(this JValue value)
        {
            return value.Type == JTokenType.Integer || value.Type == JTokenType.Float;
        }

        public static JValue Add(this JValue lhs, JValue rhs)
        {
            if (!lhs.AllowsForIncrease() || !rhs.AllowsForIncrease())
                throw new InvalidOperationException("Cannot add values that do not allow for increase.");

            switch (lhs.Type)
            {
                case JTokenType.Float:
                    float lhsFloat = lhs.FloatValue();
                    float rhsFloat = rhs.FloatValue();
                    lhs.Value = (lhsFloat + rhsFloat);
                    break;
                
                case JTokenType.Integer:
                    long lhsInt = lhs.IntValue();
                    long rhsInt = rhs.IntValue();
                    lhs.Value = (lhsInt + rhsInt);
                    break;
                
                case JTokenType.String:
                    string lhsString = lhs.StringValue();
                    string rhsString = rhs.StringValue();
                    lhs.Value = (lhsString + rhsString);
                    break;
            }

            return (lhs);
        }

        public static JValue Subtract(this JValue lhs, JValue rhs)
        {
            if (!lhs.AllowsForDecrease() || !rhs.AllowsForDecrease())
                throw new InvalidOperationException("Cannot subtract values that do not allow for decrease.");
            
            switch (lhs.Type)
            {
                case JTokenType.Float:
                    float lhsFloat = lhs.FloatValue();
                    float rhsFloat = rhs.FloatValue();
                    lhs.Value = (lhsFloat - rhsFloat);
                    break;
                
                case JTokenType.Integer:
                    long lhsInt = lhs.IntValue();
                    long rhsInt = rhs.IntValue();
                    lhs.Value = (lhsInt - rhsInt);
                    break;
            }

            return (lhs);
        }
        
        /// <summary>
        /// Sets the value of the given JValue. Creates a new JValue if the given JValue is null.
        /// </summary>
        public static void SetValue(ref JValue jValue, object value)
        {
            if (jValue == null)
                jValue = new JValue(value);
            else
                jValue.Value = value;
        }
        
        /// <summary>
        /// Creates a JValue dynamically from given input into a primitive data type.
        /// </summary>
        public static JValue ParseToPrimitiveDataType(string input)
        {
            if (bool.TryParse(input, out bool boolValue))
                return new JValue(boolValue);

            if (int.TryParse(input, out int intValue))
                return new JValue(intValue);
            
            if (float.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                return new JValue(floatValue);

            return (new JValue(input));
        }
    }
}
