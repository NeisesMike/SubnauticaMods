using System;
using System.Reflection;

namespace BZCommon
{
    public static class ReflectionHelper
    {
        public static object GetPrivateField<T>(this T instance, string fieldName, BindingFlags bindingFlags = BindingFlags.Default) where T : class
        {
            return instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | bindingFlags).GetValue(instance);
        }

        public static void SetPrivateField<T>(this T instance, string fieldName, object value, BindingFlags bindingFlags = BindingFlags.Default) where T : class
        {
            instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.SetField | bindingFlags).SetValue(instance, value);
        }

        public static object GetPrivateProperty<T>(this T instance, string propertyName, BindingFlags bindingFlags = BindingFlags.Default) where T : class
        {
            return instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | bindingFlags).GetValue(instance, null);
        }

        public static void SetPrivateProperty<T>(this T instance, string propertyName, object value, BindingFlags bindingFlags = BindingFlags.Default) where T : class
        {
            instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | bindingFlags).SetValue(instance, value);
        }

        public static void InvokePrivateMethod<T>(this T instance, string methodName, BindingFlags bindingFlags = BindingFlags.Default, params object[] parms) where T : class
        {
            instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | bindingFlags).Invoke(instance, parms);
        }

        public static void CloneFieldsInto<T>(this T original, T copy) where T : class
        {
            FieldInfo[] fieldsInfo = typeof(T).GetFields(BindingFlags.Instance);

            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                if (fieldInfo.GetType().IsClass)
                {
                    var origValue = fieldInfo.GetValue(original);
                    var copyValue = fieldInfo.GetValue(copy);

                    origValue.CloneFieldsInto(copyValue);                    
                }
                else
                {
                    var value = fieldInfo.GetValue(original);
                    fieldInfo.SetValue(copy, value);
                }                
            }
        }

        
        public static bool IsNamespaceExists(string desiredNamespace)
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

                for (int i = 0; i < assemblies.Length; i++)
                {
                    Type[] types = assemblies[i].GetTypes();

                    for (int j = 0; j < types.Length; j++)
                    {
                        if (types[j].Namespace == desiredNamespace)
                        {
                            return true;
                        }
                    }
                }                
            }
            catch
            {
                return false;
            }

            return false;
        }


        public static object GetAssemblyClassPublicField(string className, string fieldName)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    if (types[j].FullName == className)
                    {
                        return types[j].GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(types[j]);
                    }
                }
            }
            return null;
        }
    }
}
