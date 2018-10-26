using System;
using System.Collections.Generic;
using System.Linq;

namespace SIS.MvcFramework
{
    public static class ObjectMapper
    {
        public static T To<T>(this object source)
            where T : new()
        {
            var destination = new T();
            var destinationProperties = destination.GetType().GetProperties();

            foreach (var destinationProperty in destinationProperties)
            {
                if (destinationProperty.SetMethod == null)
                {
                    continue;
                }

                var sourceProperty = source.GetType().GetProperties()
                    .FirstOrDefault(x => x.Name.ToLower() == destinationProperty.Name.ToLower());
                if (sourceProperty?.GetMethod != null)
                {
                    var sourceValue = sourceProperty.GetMethod.Invoke(source, new object[0]);
                    if (sourceValue is IEnumerable<object> || sourceValue is object[])
                    {
                        var destinationCollection = sourceValue;
                        destinationProperty.SetMethod.Invoke(destination, new[] { destinationCollection });
                    }
                    else
                    {
                        var destinationValue = TryParse(sourceValue.ToString(), destinationProperty.PropertyType);
                        destinationProperty.SetMethod.Invoke(destination, new[] { destinationValue });
                    }
                }
            }

            return destination;
        }

        public static object TryParse(string stringValue, Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            object value = null;
            switch (typeCode)
            {
                case TypeCode.Int32:
                    if (int.TryParse(stringValue, out var intValue)) value = intValue;
                    break;
                case TypeCode.Char:
                    if (char.TryParse(stringValue, out var charValue)) value = charValue;
                    break;
                case TypeCode.Int64:
                    if (long.TryParse(stringValue, out var longValue)) value = longValue;
                    break;
                case TypeCode.Double:
                    if (double.TryParse(stringValue, out var doubleValue)) value = doubleValue;
                    break;
                case TypeCode.Decimal:
                    if (decimal.TryParse(stringValue, out var decimalValue)) value = decimalValue;
                    break;
                case TypeCode.DateTime:
                    if (DateTime.TryParse(stringValue, out var dateTimeValue)) value = dateTimeValue;
                    break;
                case TypeCode.String:
                    value = stringValue;
                    break;
            }

            return value;
        }
    }
}
