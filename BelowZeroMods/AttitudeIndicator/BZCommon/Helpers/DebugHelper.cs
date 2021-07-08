using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BZCommon.Helpers
{
    public static class DebugHelper
    {
        public static void DebugObjectTransforms(Transform transform, string space = "")
        {
            Console.WriteLine($"{space}--{transform.name}");

            foreach (Transform child in transform)
            {
                DebugObjectTransforms(child, space + "   |");
            }
        }

        public static void DebugComponent(Component component)
        {
            List<string> keywords = new List<string>();

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.IgnoreReturn;

            keywords.Add("Properties:");

            foreach (PropertyInfo propertyInfo in component.GetType().GetProperties(bindingFlags))
            {
                keywords.Add($"{propertyInfo.Name}  [{propertyInfo.GetValue(component, bindingFlags, null, null, null).ToString()}]");
            }

            keywords.Add("Fields:");

            foreach (FieldInfo fieldInfo in component.GetType().GetFields(bindingFlags))
            {
                keywords.Add($"{fieldInfo.Name}  [{fieldInfo.GetValue(component).ToString()}]");
            }

            foreach (string key in keywords)
            {
                BZLogger.Log($"{key}");
            }
        }
    }
}
