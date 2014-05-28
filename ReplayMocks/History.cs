using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Castle.DynamicProxy;

namespace ReplayMocks
{
    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string ParameterType { get; set; }
    }

    public class LoggedCall
    {
        public string MethodName { get; set; }

        public string ReturnValue { get; set; }

        public Parameter[] Parameters { get; set; }
    }

    public class History
    {
        public List<LoggedCall> Calls { get; set; }

        public History()
        {
            Calls = new List<LoggedCall>();
        }

        public LoggedCall GetCall(int p)
        {
            return Calls[p];
        }

        internal void Add(IInvocation invocation)
        {
            var asLoggedCall = ConvertToLoggedCall(invocation);
            Calls.Add(asLoggedCall);
        }

        private LoggedCall ConvertToLoggedCall(IInvocation invocation)
        {
            return new LoggedCall()
            {
                MethodName = invocation.Method.Name,
                ReturnValue = XmlSerialiser.SerialiseWithoutNamespacesAndHeaderWithLinebreaks(invocation.ReturnValue, invocation.Method.ReturnType),
                Parameters = GetParameters(invocation)
            };
        }

        private Parameter[] GetParameters(IInvocation invocation)
        {
            var output = new List<Parameter>();
            var methodParams = invocation.Method.GetParameters();
            foreach (var param in methodParams)
            {
                output.Add(
                    new Parameter()
                    {
                        Name = param.Name,
                        ParameterType = param.ParameterType.FullName,
                        Value = XmlSerialiser.SerialiseWithoutNamespacesAndHeaderWithLinebreaks(invocation.Arguments[param.Position], param.ParameterType)
                    });
            }
            return output.ToArray();
        }

        internal LoggedCall GetMatchingCall(IInvocation invocation)
        {
            var invokedParams = GetParameters(invocation);
            return Calls.Where(x => x.MethodName == invocation.Method.Name && IsMatchingParams(invokedParams, x.Parameters)).First();
        }

        private bool IsMatchingParams(Parameter[] invokedParams, Parameter[] historyParams)
        {
            foreach (var param in invokedParams)
            {
                var matchedName = historyParams.FirstOrDefault(x => x.Name == param.Name);
                if (matchedName == null) return false;
                if (matchedName.ParameterType != param.ParameterType) return false;
                if (matchedName.Value != param.Value) return false; // TODO: requires serialisation to be unique
            }

            foreach (var param in historyParams)
            {
                var matchedName = invokedParams.FirstOrDefault(x => x.Name == param.Name);
                if (matchedName == null) return false;
                if (matchedName.ParameterType != param.ParameterType) return false;
                if (matchedName.Value != param.Value) return false; // TODO: requires serialisation to be unique
            }

            return true;
        }




        internal void ReplayMethodResult(IInvocation invocation)
        {
            var call = GetMatchingCall(invocation);

            invocation.ReturnValue = XmlSerialiser.Deserialise(call.ReturnValue, invocation.Method.ReturnType);
        }
    }
}
