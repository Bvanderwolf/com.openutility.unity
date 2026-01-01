namespace OpenUtility.Data
{
    public static class ScriptableVariableExtensions
    {
        public static void Toggle(this ScriptableBool variable)
        {
            bool newValue = !variable.GetValue();
            variable.SetValue(newValue);
        }
    }
}
