using System;
using System.Collections;

namespace Repository.Common
{
    public static class Verify
    {
        public static void Argument(bool condition, string argumentName)
        {
            if (!condition)
            {
                throw new ArgumentException(null, argumentName);
            }
        }

        public static void NotNull<T>(T argument, string argumentName)
            where T : class
        {
            NotNullGeneric(argument, argumentName);
        }

        public static void NotNullGeneric<T>(T argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void NotNullOrEmpty<TEnumerable>(TEnumerable argument, string argumentName)
            where TEnumerable : class, IEnumerable
        {
            NotNull(argument, argumentName);

            if (!argument.GetEnumerator().MoveNext())
            {
                throw new ArgumentException($"{argumentName} was empty.", argumentName);
            }
        }

        public static void ValidState(bool condition, string message = null)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}