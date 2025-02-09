using System;
using System.Linq;
using System.Reflection;

namespace mikealpha{
    public static class GetDerivedClassNames
    {
        public static Type[] GetDerivedClasses<T>()
        {
            var assembly = Assembly.GetAssembly(typeof(T));

            var derivedTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract)
                .ToArray();

            return derivedTypes;
        }
    }
}
