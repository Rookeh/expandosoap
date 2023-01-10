using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ExpandoSoap.Extensions
{
    public static class ExpandoExtensions
    {
        public static T ConvertTo<T>(this ExpandoObject expando)
        {
            expando = UnwrapExpando(expando, typeof(T));
            return (T)ConvertObject(expando, typeof(T));
        }

        private static object ConvertObject(ExpandoObject expando, Type targetType)
        {
            var constructed = Activator.CreateInstance(targetType);
            var expandoProps = (IDictionary<string, object>)expando;

            foreach (var property in expandoProps)
            {
                var targetProp = targetType.GetProperties()
                    .FirstOrDefault(p =>
                        p.Name.Equals(property.Key, StringComparison.InvariantCultureIgnoreCase));

                if (targetProp != null)
                {
                    if (property.Value is ExpandoObject)
                    {
                        var innerConstructed = ConvertObject((ExpandoObject)property.Value, targetProp.PropertyType);
                        targetProp.SetValue(constructed, innerConstructed);
                    }
                    else
                    {
                        var converted = Convert.ChangeType(property.Value, targetProp.PropertyType);
                        targetProp.SetValue(constructed, converted);
                    }
                }
            }

            return constructed;
        }

        private static ExpandoObject UnwrapExpando(ExpandoObject expando, Type type)
        {
            var expandoDict = (IDictionary<string, object>)expando;

            if (expandoDict.Keys.Count == 1 && expandoDict.Values.First() is ExpandoObject && TypeNameMatchesElementName(type.Name, expandoDict.Keys.First()))
            {
                return (ExpandoObject)expandoDict.Values.First();
            }
            else if (expandoDict.Values.Count == 1 && expandoDict.Values.First() is ExpandoObject)
            {
                return UnwrapExpando((ExpandoObject)expandoDict.Values.First(), type);
            }
            else
            {
                return null;
            }
        }

        private static bool TypeNameMatchesElementName(string typeName, string elementName)
        {
            if (elementName.Contains(':'))
            {
                elementName = elementName.Substring(elementName.IndexOf(':') + 1, elementName.Length - elementName.IndexOf(':') - 1);
            }

            return elementName.Equals(typeName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}