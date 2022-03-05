using System;
using System.Reflection;

namespace Mvp.Foundation.RulesEngine.Utils
{
    public static class ReflectionUtil
    {
		public static Type GetGenericType<T>(string typeName)
		{
			return GetGenericType(typeName, typeof(T));
		}

		public static Type GetGenericType(string typeName, Type genericArgumentType)
        {
            typeName = typeName.Trim();
            string text = string.Empty;
            int num = typeName.IndexOf(",", StringComparison.InvariantCulture);
            if (num >= 0)
            {
                text = typeName.Mid(num + 1).Trim();
                typeName = typeName.Left(num).Trim();
            }
            //Assembly assembly = LoadAssembly(text);
			Assembly assembly = Assembly.GetExecutingAssembly();
            if (assembly == null)
            {
                return null;
            }
            return assembly.GetType(typeName);
        }

		public static object CreateObject(Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
			if (constructor != null)
			{
				object[] emptyTypes = Type.EmptyTypes;
				object obj = constructor.Invoke(emptyTypes);
				if (obj != null)
				{
					return obj;
				}
			}
			return null;
		}
	}
}
