using OpenUtility.Exceptions;

namespace OpenUtility.Data
{
    public static class ScriptableVariableExtensions
    {
        public static void Toggle(this ScriptableBool variable)
        {
            bool newValue = !variable.GetValue();
            variable.SetValue(newValue);
        }
        
        public static void SetValue(this ScriptableBool variable, int value)
        {
            ThrowIf.NotBool(value, out bool result);
            
            variable.SetValue(result);
        }

        public static void SetValueWithoutNotify(this ScriptableBool variable, int value)
        {
            ThrowIf.NotBool(value, out bool result);
            
            variable.SetValueWithoutNotify(result);
        }

        public static int GetValue(this ScriptableBool variable)
        {
            int value = variable.GetValue() ? 1 : 0;
            return (value);
        }
    }
}
