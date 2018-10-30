using System.Runtime.CompilerServices;

namespace SIS.HTTP.Common
{
    using System;

    public class CoreValidator
    {
        public static void ThrowIfNull(object obj, string name)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(name);
            }
        }

        public static void ThrowIfNullOrEmpty(string text, string name)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException($"{name} cannot be null or empty.", name);
            }
        }

        public static void ThrowIfAllNull(params object[] arguments)
        {
            foreach (var argument in arguments)
            {
                if (argument != null)
                {
                    return;
                }
            }

            throw new ArgumentException($"All arguments are null: {string.Join(", ", arguments)}.");
        }
    }
}