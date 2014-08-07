using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SWPCCBilling2.Infrastructure
{
    public static class DbConnectionExtensions
    {
        public static IDbCommand CreateStoredProcedure(this IDbConnection connection, string name)
        {
            IDbCommand cmd = connection.CreateCommand();

            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;

            return cmd;
        }

        public static IDbCommand CreateStoredProcedure(this IDbConnection connection, string name, object paramObj)
        {
            IDbCommand cmd = connection.CreateCommand();

            cmd.CommandText = name;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.AddParameters(paramObj);

            return cmd;
        }

        public static IDbCommand CreateCommand(this IDbConnection connection, string text)
        {
            IDbCommand cmd = connection.CreateCommand();

            cmd.CommandText = text;
            cmd.CommandType = CommandType.Text;

            return cmd;
        }

        public static IDbCommand CreateCommand(this IDbConnection connection, string text, object paramObj)
        {
            IDbCommand cmd = connection.CreateCommand();

            cmd.CommandText = text;
            cmd.CommandType = CommandType.Text;

            if (paramObj != null)
            {
                if (paramObj.GetType().IsArray)
                    cmd.AddParametersWithArray((object[])paramObj);
                else
                    cmd.AddParameters(paramObj);
            }

            return cmd;
        }

        public static int Execute(this IDbConnection con, string text, object paramObj = null)
        {
            IDbCommand cmd = CreateCommand(con, text, paramObj);
            return cmd.ExecuteNonQuery();
        }

        public static object ExecuteScalar(this IDbConnection con, string text, object paramObj = null)
        {
            IDbCommand cmd = CreateCommand(con, text, paramObj);
            return cmd.ExecuteScalar();
        }

        public static T ExecuteScalar<T>(this IDbConnection con, string text, object paramObj = null)
        {
            IDbCommand cmd = CreateCommand(con, text, paramObj);
            return (T)cmd.ExecuteScalar();
        }

        public static IEnumerable<T> Query<T>(this IDbConnection con, string text, object paramObj = null)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof (T));

            IDbCommand cmd = CreateCommand(con, text, paramObj);
            var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                var result = (T)Activator.CreateInstance(typeof(T));

                for (int i = 0; i < dr.FieldCount; i++)
                {
                    PropertyDescriptor property = properties
                        .Cast<PropertyDescriptor>()
                        .SingleOrDefault(p => p.Name == dr.GetName(i));

                    if (property == null)
                        continue;

                    object sourceValue = dr.GetValue(i);
                    object targetValue = null;

					if (sourceValue != null && sourceValue.GetType() != typeof(DBNull))
                    {
                        Type conversionType = property.PropertyType;

                        if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var converter = new NullableConverter(property.PropertyType);
                            conversionType = converter.UnderlyingType;
                        }

                        targetValue = Convert.ChangeType(sourceValue, conversionType);
                    }

                    if (targetValue != null)
                        property.SetValue(result, targetValue);
                }

                yield return result;
            }

            dr.Close();
        }
    }

}