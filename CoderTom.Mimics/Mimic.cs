using System;
using System.Collections.Generic;
using System.IO;
using CoderTom.Mimics.Utilities;

namespace CoderTom.Mimics
{
    public static class Mimic
    {
        private static IEqualityComparer<object> comparer = new EqualityByXmlSerialisation();
        private static ICopier<object> copier = new CopyByXmlSerialisation();

        internal static ICopier<object> GetCopier()
        {
            return copier;
        }

        private static Dictionary<object, MimeProxy> proxyObjectsAndProxies = new Dictionary<object, MimeProxy>();

        #region proxies
        public static T Stub<T>(History history) where T : class
        {
            var interceptor = new CallReplayingInterceptor(history);
            return CreateAndStoreProxy<T>(interceptor);
        }

        public static T Record<T>(T instance) where T : class
        {
            var interceptor = new CallRecordingInterceptor();
            return CreateAndStoreProxy<T>(interceptor, instance);
        }

        public static T Cache<T>(T instance, History history) where T : class
        {
            var interceptor = new CallCachingInterceptor(history);
            return CreateAndStoreProxy<T>(interceptor, instance);
        }

        public static BehaviourVerifier GetBehaviourVerifier(History history)
        {
            return new BehaviourVerifier(comparer, history);
        }

        /// <summary>
        /// This call is in beta
        /// </summary>
        internal static bool ConfirmSameCallsWereMade(object proxyObject, MockOptions options = MockOptions.Default_SameCallsSameNumberOfTimesInAnyOrder)
        {
            return proxyObjectsAndProxies[proxyObject].ConfirmSameCallsWereMade(options);
        }
        #endregion proxies

        #region history
        public static History GetCachedHistory(object proxyObject)
        {
            var proxy = proxyObjectsAndProxies[proxyObject];
            return proxy.CombineThisRunLogAndPastRunLog();
        }

        public static History GetHistory(object proxyObject)
        {
            var proxy = proxyObjectsAndProxies[proxyObject];
            return proxy.GetThisRunLog();
        }

        public static History GetHistory(BehaviourVerifier verifier)
        {
            var history = new History();
            foreach (var item in verifier.VerificationLog)
            {
                history.Calls.Add(item.ActualCall);
            }

            return history;
        }

        public static string SerialiseHistory(object proxy)
        {
            return SerialiseHistory(GetHistory(proxy));
        }

        public static string SerialiseCachedHistory(object proxy)
        {
            return SerialiseHistory(GetCachedHistory(proxy));
        }

        public static string SerialiseHistory(History history)
        {
            return XmlSerialiser.SerialiseWithoutNamespacesAndHeaderWithLinebreaks(history, typeof(History));
        }

        public static History DeserialiseHistory(string historyXml)
        {
            return (History)XmlSerialiser.Deserialise(historyXml, typeof(History));
        }

        public static History HistoryFromFile(string filename)
        {
            return DeserialiseHistory(File.ReadAllText(filename));
        }

        public static void SaveCachedHistoryToFile(object proxy, string filePath)
        {
            var serialised = Mimic.SerialiseCachedHistory(proxy);
            File.WriteAllText(filePath, serialised);
        }

        public static void SaveHistoryToFile(object proxyObject, string filePath)
        {
            var serialised = Mimic.SerialiseHistory(proxyObject);
            File.WriteAllText(filePath, serialised);
        }
        #endregion history

        private static T CreateAndStoreProxy<T>(MimeInterceptor interceptor) where T : class
        {
            ThrowIfNotInterface(typeof(T));

            var proxy = new MimeProxy<T>(interceptor);
            proxyObjectsAndProxies.Add(proxy.Object, proxy);
            return proxy.Object;
        }

        private static T CreateAndStoreProxy<T>(MimeInterceptor interceptor, T instance) where T : class
        {
            ThrowIfNotInterface(typeof (T));

            var proxy = new MimeProxy<T>(interceptor, instance);
            proxyObjectsAndProxies.Add(proxy.Object, proxy);
            return proxy.Object;
        }

        private static void ThrowIfNotInterface(Type type)
        {
            var isInterface = type.IsInterface;
            if (!isInterface)
            {
                throw new TypeArgumentMustBeAnInterfaceException(type);
            }
        }
    }
}
