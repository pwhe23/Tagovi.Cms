using System;
using System.Linq;
using System.Reflection;

namespace Site
{
    public static class Ext
    {
        public static bool HasValue(this String txt)
        {
            return !String.IsNullOrWhiteSpace(txt);
        }

        public static bool HasValue(this DateTime date)
        {
            return date != DateTime.MinValue;
        }

        public static bool HasValue(this int? number)
        {
            return number.HasValue && number.Value != 0;
        }

        public static bool IsDate(Object date)
        {
            if (date == null) return false;
            DateTime dt;
            if (!DateTime.TryParse(date.ToString(), out dt)) return false;
            if ((dt < DateTime.Today.AddYears(-100)) || (dt > DateTime.Today.AddYears(50))) return false;
            return true;
        }

        public static bool Contains(this String txt, params string[] arr)
        {
            if (!txt.HasValue() || arr.Length == 0) return false;
            return arr.Any(txt.Contains);
        }

        public static string Or(this Object obj, params object[] others)
        {
            var value = obj == null ? string.Empty : obj.ToString();
            if (value.HasValue()) return value;
            foreach (var item in others)
            {
                value = item == null ? string.Empty : item.ToString();
                if (value.HasValue()) return value;
            }
            return string.Empty;
        }

        public static T To<T>(this Object obj)
        {
            return (T)To(obj, typeof(T), null);
        }

        public static T To<T>(this Object obj, T def)
        {
            return (T)To(obj, typeof(T), def);
        }

        public static Object To(this Object obj, Type type)
        {
            return To(obj, type, null);
        }

        public static Object To(this Object obj, Type type, Object def)
        {
            if (obj != null && obj.GetType() == type) return obj;
            if (obj is DBNull) obj = null;
            if (type.IsGenericType)
            {
                if (obj == null || (obj is String && obj.Equals(String.Empty))) return def;
                var nullableConverter = new System.ComponentModel.NullableConverter(type);
                type = nullableConverter.UnderlyingType;
            }
            else if (!type.IsClass && (obj == null || (obj is String && obj.ToString() == String.Empty)))
            {
                return def;
            }
            if (obj is String && type.FullName.Contains("Boolean"))
            {
                if ((obj as String).HasValue() && obj.ToString().Contains("True", "true", "on", "ON")) return true;
                else return false;
            }
            else if (obj != null && type == typeof(int))
            {
                int result;
                return int.TryParse(obj.ToString().Replace(",", ""), out result) ? result : def;
            }
            else if (obj is DateTime && type.FullName.Contains("String"))
            {
                if ((DateTime)obj == DateTime.MinValue) return null;
            }
            else if (type.FullName.Contains("DateTime") && obj is String && obj != null)
            {
                if (obj.ToString().HasValue() && !Ext.IsDate(obj.ToString()))
                    return null;
                //throw new FormatException("Invalid date '" + obj + "', please enter the format MM/DD/YYYY"); // return Activator.CreateInstance(type);
            }
            else if (type == typeof(decimal))
            {
                if (obj is String && (string)obj != null)
                {
                    if ((obj as String).Contains("$")) obj = obj.ToString().Replace("$", "");
                    if ((obj as string).Contains("(")) obj = "-" + obj.ToString().Replace("(", "").Replace(")", "");
                }
                Decimal value;
                return Decimal.TryParse(obj.Or(), out value) ? value : def;
            }
            else if (obj != null && type.IsInstanceOfType(obj))
            {
                return obj;
            }
            try
            {
                return Convert.ChangeType(obj, type);
            }
            catch (FormatException ex)
            {
                throw new FormatException(string.Format("Cannot convert '{0}' to '{1}'", obj, type), ex);
            }
        }

        public static T Get<T>(this Object obj, String name)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            if (obj == null) return default(T);
            var prop = obj.GetType().GetProperty(name, flags);
            if (prop != null) return prop.GetValue(obj, null).To<T>();
            var field = obj.GetType().GetField(name, flags);
            if (field != null) return field.GetValue(obj).To<T>();
            return default(T);
        }
        public static Object Get(this Object obj, String name)
        {
            if (obj == null) return null;
            var prop = obj.GetType().GetProperty(name);
            if (prop == null) throw new Exception(name + " property not found");
            return Get(obj, prop);
        }
        public static Object Get(this Object obj, PropertyInfo prop)
        {
            return prop.GetValue(obj, null);
        }

        public static void Set(this Object obj, string name, object value)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
            var prop = obj.GetType().GetProperty(name, flags);
            if (prop != null)
            {
                prop.SetValue(obj, value.To(prop.PropertyType), null);
                return;
            }
            var field = obj.GetType().GetField(name, flags);
            if (field != null)
            {
                field.SetValue(obj, value.To(field.FieldType));
                return;
            }
        }
        public static void Set(this Object obj, PropertyInfo prop, object value)
        {
            if (prop != null)
            {
                if (prop.PropertyType.IsPrimitive && value == null) return;
                prop.SetValue(obj, value.To(prop.PropertyType), null);
            }
        }
        public static void Set(this Object obj, FieldInfo field, object value)
        {
            if (field != null)
            {
                field.SetValue(obj, value.To(field.FieldType));
            }
        }
    };
}
