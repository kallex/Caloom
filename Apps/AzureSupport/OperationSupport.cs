using System;
using System.Collections.Generic;
using System.IO;
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
            if(operationType == null)
                throw new InvalidDataException("Operation type not found: " + operationTypeName);
            Type parameterType = TypeSupport.GetTypeByName(parameterTypeName);
            if(parameterType == null)
                throw new InvalidDataException("Operation parameter type not found: " + parameterTypeName);
            var parameters = Activator.CreateInstance(parameterType);
            foreach (var par in parameterValues)
            {
                var fieldName = par.Item1;
                var fieldValue = par.Item2;
                var field = parameterType.GetField(par.Item1);
                if(field == null)
                    throw new InvalidDataException("Parameter invalid field name: " + fieldName);
                field.SetValue(parameters, fieldValue);
            }
            var method = operationType.GetMethod("Execute", BindingFlags.Public | BindingFlags.Static);
            if(method == null)
                throw new InvalidDataException("Execute method not found in operation class: " + operationTypeName);
            method.Invoke(null, new object[] { parameters });
        }
    }
}