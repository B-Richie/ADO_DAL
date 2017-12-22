using System;
using System.Data.SqlClient;

namespace AutomobileApp.DAL
{
    public static class Extensions
    {
        public static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)) ? true : false;
        }

        public static T GetValue<T>(this SqlDataReader dr, string column)
        {
            var conversionType = typeof(T);
            Type underlyingType = null;
            var value = dr[column];

            if (IsNullable(conversionType))
            {
                if (value == null)
                    return default(T);

                underlyingType = Nullable.GetUnderlyingType(conversionType);
            }

            if (dr.IsDBNull(dr.GetOrdinal(column)))
                return default(T);

            switch (underlyingType != null ? underlyingType.Name.ToString() : conversionType.Name.ToString())
            {
                case "String":
                    break;
                case "Int32":
                    break;
                case "Boolean":
                    switch (value.ToString())
                    {
                        case "Y":
                            value = true;
                            break;
                        case "N":
                            value = false;
                            break;
                        case "0":
                            value = false;
                            break;
                        case "1":
                            value = true;
                            break;
                        case "T":
                            value = true;
                            break;
                        case "F":
                            value = false;
                            break;
                        default:
                            if (conversionType.Name.ToString() == "Nullable`1")
                                value = null;
                            else
                                value = false;
                            break;
                    }
                    break;
                case "DateTime":
                    break;
            }
            if (underlyingType != null)
                return (T)Convert.ChangeType(value, underlyingType);
            else
                return (T)Convert.ChangeType(value, conversionType);
        }
    }

}