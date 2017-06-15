using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Common
{
    public static class Verify
    {
        public static void Argument(bool condition, string argumentName = null)
        {
            if (!condition)
            {
                throw new ArgumentException(null, argumentName);
            }
        }

        public static T NotNull<T>(T argument, string argumentName = null) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return argument;
        }

        public static TEnumerable NotNullOrEmpty<TEnumerable>(TEnumerable argument, string argumentName = null)
            where TEnumerable : class, IEnumerable
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (!argument.GetEnumerator().MoveNext())
            {
                throw new ArgumentException($"{argumentName} was empty.", argumentName);
            }

            return argument;
        }
    }
}