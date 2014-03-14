using System;
using System.Collections.Generic;
using System.Reflection;
using AzureSupport;

namespace TheBall
{
    public static class OperationSupport
    {
        public static void ExecuteOperation(string operationTypeName, params Tuple<string, object>[] parameterValues)
        {
            string parameterTypeName = operationTypeName + "Parameters";
            Type operationType = TypeSupport.GetTypeByName(operationTypeName);
            Type parameterType = TypeSupport.GetTypeByName(parameterTypeName);
            var parameters = Activator.CreateInstance(parameterType);
            foreach (var par in parameterValues)
            {
                var prop = parameterType.GetProperty(par.Item1);
                prop.SetValue(parameters, par.Item2);
            }
            var method = operationType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, new object[] { parameters });
        }
    }
}