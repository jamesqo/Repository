using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Repository.Internal
{
    internal static class Verify
    {
        public static T NotNull<T>(T argument) where T : class => NotNull(argument, nameof(argument));

        public static T NotNull<T>(T argument, string argumentName) where T : class
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            return argument;
        }
    }
}