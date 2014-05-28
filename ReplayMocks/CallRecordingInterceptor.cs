using System.Diagnostics;
using System.Text;
using Castle.DynamicProxy;

namespace ReplayMocks
{
    public class CallRecordingInterceptor : IInterceptor
    {
        //private StringBuilder _log = new StringBuilder();
        //public string Log { get { return _log.ToString();  }}

        public bool AllowAccessToObject { get; set; }

        public History Log { get; set; }

        public CallRecordingInterceptor()
        {
            AllowAccessToObject = true;
            Log = new History();
        }

        public void Intercept(IInvocation invocation)
        {
            //AppendMethodCallToLog(invocation);

            if (AllowAccessToObject)
            {
                invocation.Proceed();

                Log.Add(invocation);
            }
        }

        private string SerialiseMethodName(IInvocation invocation)
        {
            return invocation.Method.Name;
        }

        //private void AppendMethodCallToLog(IInvocation invocation)
        //{
        //    _log.Append("Method Call: ");
        //    _log.Append(invocation.Method);
        //    _log.Append("(");
        //    var arguments = String.Join(", ", invocation.Arguments.Select(x => EncodeArgument(x)).ToArray());
        //    _log.Append(arguments);
        //    _log.AppendLine(");");
        //}

        //private string Stringify(object argument)
        //{
        //    if (argument == null)
        //    {
        //        return "{null}";
        //    }
        //
        //    var allowedTypes = new[] { typeof(string), typeof(byte), typeof(Array), typeof(IList) };
        //    if (!allowedTypes.Any(x => x.IsInstanceOfType(argument)))
        //    {
        //        throw new NotImplementedException("need to ensure we can uniquely serialise this type in the callloggingintercetpor " + argument.GetType().ToString());
        //    }
        //
        //    if (argument is Array)
        //    {
        //        var sb = new StringBuilder();
        //        sb.Append("Array");
        //
        //        return serialiseIEnumerable(argument, sb);
        //    }
        //
        //    if (argument is IList)
        //    {
        //        var sb = new StringBuilder();
        //        sb.Append("List");
        //
        //        return serialiseIEnumerable(argument, sb);
        //    }
        //
        //    return Encode(argument.ToString());
        //}

        //private string serialiseIEnumerable(object argument, StringBuilder sb)
        //{
        //    sb.Append("[");
        //    var isFirst = true;
        //    var asiEnumerable = argument as IEnumerable;
        //    foreach (var item in asiEnumerable)
        //    {
        //        if (!isFirst)
        //        {
        //            sb.Append(", ");
        //        }
        //        sb.Append(Stringify(item));
        //        isFirst = false;
        //    }
        //    sb.Append("]");
        //    return sb.ToString();
        //}

        //private string Encode(string input)
        //{
        //    return input.Replace(@"\", @"\\")
        //        .Replace(@"{", @"\{")
        //        .Replace(@"}", @"\}")
        //        .Replace(@"[", @"\[")
        //        .Replace(@"]", @"\]")
        //        .Replace(@",", @"\,")
        //        .Replace("\n", "\\n")
        //        .Replace("\r", "\\r");
        //}

        //private string EncodeArgument(object argument)
        //{
        //    var asText = Stringify(argument);
        //    return "{" + asText + "}";
        //}
    }
}