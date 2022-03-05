using System;
using System.Reflection;

namespace Mvp.Foundation.RulesEngine.Utils
{
    public static class ReflectionUtil
    {
        //Currently this class only supports creating an object from the Executing Assembly
		public static Type GetGenericType(string typeName)
        {
            typeName = typeName.Trim();
            int num = typeName.IndexOf(",", StringComparison.InvariantCulture);
            if (num >= 0)
            {
                typeName = typeName.Left(num).Trim();
            }
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
