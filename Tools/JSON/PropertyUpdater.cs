using System;
using System.Diagnostics;
using System.Linq;

namespace InazumaDSEditor.Tools.JSON
{
    public static class PropertyUpdater
    {
        public static void UpdateProperties<T>(ref T target, T source) where T : struct
        {
            var properties = typeof(T).GetProperties();

            // Create a copy of the target to modify
            object boxedTarget = target;

            foreach (var prop in properties)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    var sourceValue = prop.GetValue(source);

                    if (sourceValue is null) // Skip null values in the source
                    {
                        continue;
                    }

                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(boxedTarget, sourceValue);
                    }
                    else if (prop.PropertyType.IsValueType)
                    {
                        prop.SetValue(boxedTarget, sourceValue); // Update value type
                    }
                    else
                    {
                        prop.SetValue(boxedTarget, sourceValue); // Update reference types
                    }
                }
            }

            // Assign the modified struct back to the target
            target = (T)boxedTarget;
        }
    }

}
