using System.Linq;
using System.Reflection;

namespace Common.Reflection
{
    public static class ObjectExtensions
    {
        public static FieldInfo? GetNonPublicInstanceField(this object instance, string name)
        {
            return instance.GetField(BindingFlags.NonPublic | BindingFlags.Instance, name);
        }
        public static FieldInfo? GetField(this object instance, BindingFlags bindingAttr, string name)
        {
            var fields = instance.GetType().GetFields(bindingAttr);
            return fields.FirstOrDefault(e => e.Name == name);
        }

        public static T? GetNonPublicInstanceFieldValue<T>(this object instance, string name)
            where T : class
        {
            var field = GetNonPublicInstanceField(instance, name);
            if (field == null)
                return default;

            return field.GetFieldValue<T>(BindingFlags.NonPublic | BindingFlags.Instance, name);
        }
        public static T? GetFieldValue<T>(this object instance, BindingFlags bindingAttr, string name)
            where T : class
        {
            var field = GetField(instance, bindingAttr, name);
            if (field == null)
                return default;

            return field.GetValue(instance) as T;
        }
    }
}