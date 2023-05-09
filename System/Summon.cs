namespace System
{
    public static class Summon
    {
        public static object New(string fullyQualifiedName)
        {
            Type type = Type.GetType(fullyQualifiedName);
            if (type != null)
                return Activator.CreateInstance(type);

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(fullyQualifiedName);
                if (type != null)
                    return Activator.CreateInstance(type);
            }

            return null;
        }

        public static object New(string fullyQualifiedName, params object[] constructorParams)
        {
            Type type = Type.GetType(fullyQualifiedName);
            if (type != null)
                return Activator.CreateInstance(type, constructorParams);

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(fullyQualifiedName);
                if (type != null)
                    return Activator.CreateInstance(type, constructorParams);
            }

            return null;
        }

        public static object New(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public static object New(Type type, params object[] ctorArguments)
        {
            return Activator.CreateInstance(type, ctorArguments);
        }

        public static T New<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public static T New<T>(params object[] ctorArguments)
        {
            return (T)Activator.CreateInstance(typeof(T), ctorArguments);
        }
    }

    public static class SummonExtensions
    {
        public static object New(this Type type, params object[] ctorArguments)
        {
            return Summon.New(type, ctorArguments);
        }

        public static object New(this Type type)
        {
            return Summon.New(type);
        }

        public static T New<T>(this Type type, params object[] ctorArguments)
        {
            return (T)Summon.New(type, ctorArguments);
        }

        public static T New<T>(this Type type)
        {
            return (T)Summon.New(type);
        }

        public static T New<T>(this T objectType, params object[] ctorArguments)
        {
            return Summon.New<T>(ctorArguments);
        }

        public static T New<T>(this T objectType)
        {
            return Summon.New<T>();
        }
    }
}
