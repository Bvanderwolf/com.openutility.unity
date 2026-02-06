using OpenUtility.Data;

namespace OpenUtility.Samples.Data
{
    public static class ScriptableVariableExtensions
    {
        public static void Increment(this ScriptableInt variable, int amount = 1)
        {
            int current = variable.GetValue();
            variable.SetValue(current + amount);
        }
        
        public static void Decrement(this ScriptableInt variable, int amount = 1)
        {
            int current = variable.GetValue();
            variable.SetValue(current - amount);
        }
    }
}