using System;
using System.ComponentModel;
using System.Data;

namespace SWPCCBilling2.Infrastructure
{
    public static class DbCommandExtensions
    {
        public static IDbDataParameter AddParameter(this IDbCommand command, string name, object value)
        {
            IDbDataParameter parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static IDbCommand AddParameters(this IDbCommand command, object paramObj)
        {
            if (paramObj != null)
                foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(paramObj))
                    AddParameter(command, descriptor.Name, descriptor.GetValue(paramObj));

            return command;
        }

        public static IDbCommand AddParametersWithArray(this IDbCommand command, object[] array)
        {
            if (array != null)
            {
                foreach (object obj in array)
                    AddParameter(command, null, obj);
            }

            return command;
        }

        public static IDbDataParameter AddOutputParameter(this IDbCommand command, string name, DbType dbType)
        {
            IDbDataParameter parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.Output;
            parameter.DbType = dbType;

            command.Parameters.Add(parameter);

            return parameter;
        }

        public static IDbDataParameter AddOutputParameter(this IDbCommand command, string name, DbType dbType, int size)
        {
            IDbDataParameter parameter = command.CreateParameter();

            parameter.ParameterName = name;
            parameter.Direction = ParameterDirection.Output;
            parameter.DbType = dbType;
            parameter.Size = size;

            command.Parameters.Add(parameter);

            return parameter;
        }
    }
}
