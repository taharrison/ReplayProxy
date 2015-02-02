using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Castle.DynamicProxy;
using ReplayProxy.Utilities;

namespace ReplayProxy
{
    public class History
    {
        public List<LoggedCall> Calls { get; internal set; }

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

        internal void AddException(IInvocation invocation, Exception exception)
        {
            var asLoggedCall = ConvertToLoggedCall(invocation);
            asLoggedCall.ThrownExceptionType = exception.GetType().AssemblyQualifiedName;
            Calls.Add(asLoggedCall);
        }

        private LoggedCall ConvertToLoggedCall(IInvocation invocation)
        {
            Type t;
            if (invocation.ReturnValue != null)
                t = invocation.ReturnValue.GetType();
            else
            {
                t = invocation.Method.ReturnType;
            }
            var copier = Proxy.GetCopier();

            return new LoggedCall()
            {
                MethodName = invocation.Method.Name,
                MethodDeclaringTypeName = invocation.Method.DeclaringType.AssemblyQualifiedName,
                ReturnValue = copier.Copy(invocation.ReturnValue),
                ReturnType = t.AssemblyQualifiedName,
                Parameters = GetParameters(invocation)
            };
        }


        private Parameter[] GetParameters(IInvocation invocation)
        {
            var copier = Proxy.GetCopier();
            var output = new List<Parameter>();
            var methodParams = invocation.Method.GetParameters();
            foreach (var param in methodParams)
            {
                var type = invocation.Arguments[param.Position] != null
                    ? invocation.Arguments[param.Position].GetType()
                    : param.ParameterType;

                output.Add(
                    new Parameter()
                    {
                        Name = param.Name,
                        ParameterActualType = type.AssemblyQualifiedName,
                        ParameterExpectedType = param.ParameterType.AssemblyQualifiedName,
                        Value = copier.Copy(invocation.Arguments[param.Position])
                    });
            }
            return output.ToArray();
        }

        internal LoggedCall GetMatchingCall(IInvocation invocation)
        {
            var invokedParams = GetParameters(invocation);
            return Calls.Where(x => x.MethodName == invocation.Method.Name 
                && x.MethodDeclaringTypeName == invocation.Method.DeclaringType.AssemblyQualifiedName 
                && IsMatchingParams(invokedParams, x.Parameters))
                .FirstOrDefault();
        }

        private static bool IsMatchingParams(Parameter[] invokedParams, Parameter[] historyParams)
        {
            foreach (var param in invokedParams)
            {
                var matchedName = historyParams.FirstOrDefault(x => x.Name == param.Name);
                if (matchedName == null) return false;
                if (matchedName.ParameterActualType != param.ParameterActualType) return false;
                if (matchedName.ParameterExpectedType != param.ParameterExpectedType) return false;


                bool matchValueIsNull = matchedName.Value == null;
                bool parameterIsNull = param.Value == null;

                if (matchValueIsNull ^ parameterIsNull)
                    return false;

                if (!(matchValueIsNull || parameterIsNull)  
                    && XmlSerialiser.Serialise(matchedName.Value) != XmlSerialiser.Serialise(param.Value)) return false; // TODO: requires serialisation to be unique
            }

            foreach (var param in historyParams)
            {
                var matchedName = invokedParams.FirstOrDefault(x => x.Name == param.Name);
                if (matchedName == null) return false;
                if (matchedName.ParameterActualType != param.ParameterActualType) return false;
                if (matchedName.ParameterExpectedType != param.ParameterExpectedType) return false;
                
                bool matchValueIsNull = matchedName.Value == null;
                bool parameterIsNull = param.Value == null;

                if (matchValueIsNull ^ parameterIsNull)
                    return false;

                if (!(matchValueIsNull || parameterIsNull)  
                    && XmlSerialiser.Serialise(matchedName.Value) != XmlSerialiser.Serialise(param.Value)) return false; // TODO: requires serialisation to be unique
            }

            return true;
        }

        public T ThrowInstance<T>() where T : Exception, new()
        {
            var instance = new T();
            throw instance;
        }

        internal void ReplayMethodResult(IInvocation invocation)
        {
            var call = GetMatchingCall(invocation);
            if (call == null)
            {
                var asLoggedCall = ConvertToLoggedCall(invocation);
                var unexpectedCallException = new UnexpectedCallException("Unexpected Call: " + asLoggedCall.MethodName);
                throw unexpectedCallException;
            }
            if (!String.IsNullOrWhiteSpace(call.ThrownExceptionType ))
            {
                var exceptionType = Type.GetType(call.ThrownExceptionType);
                var thisType = this.GetType();
                var method1 = thisType.GetMethod("ThrowInstance");
                var method2 = method1.MakeGenericMethod(exceptionType);
                try
                {
                    method2.Invoke(this, new object[] {});
                }
                catch (TargetInvocationException e)
                {
                    throw e.InnerException;
                }
            }
            invocation.ReturnValue = call.ReturnValue;
        }

        internal static bool Equals(LoggedCall a, LoggedCall b)
        {
            return a.MethodName == b.MethodName && IsMatchingParams(a.Parameters, b.Parameters);
        }
    }
}
