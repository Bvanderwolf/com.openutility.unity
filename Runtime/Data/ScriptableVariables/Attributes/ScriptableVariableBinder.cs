using System;

namespace OpenUtility.Data
{
    /// <summary>
    /// Use to mark a binding class for a ScriptableVariableBinding type. Add the type of component
    /// to bind to in the constructor (e.g. TMP_InputField).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class ScriptableVariableBinder : Attribute
    {
        public Type TypeOfComponentToBindTo { get; }
        public Type TypeOfValue { get; }
        
        /// <summary>
        /// A display name for the binding option in the inspector. Should be unique compered to other display names.
        /// </summary>
        public string DisplayName { get; set; }

        public Type TypeOfScriptableVariable
        {
            get
            {
                if (_typeOfScriptableVariable != null)
                    return (_typeOfScriptableVariable);

                if (TypeOfValue == typeof(int))
                    return (typeof(ScriptableInt));

                if (TypeOfValue == typeof(float))
                    return (typeof(ScriptableFloat));

                if (TypeOfValue == typeof(string))
                    return (typeof(ScriptableString));

                throw new NotImplementedException($"No default scriptable variable implementation available for type {TypeOfValue.Name}. Make sure to the the 'typeOfScriptableVariable' parameter.");
            }
        }
        
        private readonly Type _typeOfScriptableVariable;
        
        public ScriptableVariableBinder(Type typeOfComponentToBindTo, Type typeOfValue, Type typeOfScriptableVariable = null)
        {
            TypeOfComponentToBindTo = typeOfComponentToBindTo;
            TypeOfValue = typeOfValue;
            _typeOfScriptableVariable = typeOfScriptableVariable;
        }
    }
}
