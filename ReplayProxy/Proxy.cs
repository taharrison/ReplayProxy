using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReplayProxy.Utilities;

namespace ReplayProxy
{
    public static class Proxy
    {
        private static IEqualityComparer<object> comparer = new EqualityByXmlSerialisation();
        private static ICopier<object> copier = new CopyByXmlSerialisation();

        internal static ICopier<object> GetCopier()
        {
            return copier;
        }

        private static Dictionary<object, ReplayProxy> proxyObjectsAndProxies = new Dictionary<object, ReplayProxy>();

        #region proxies
        public static T Replay<T>(History history) where T : class
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
        public static bool ConfirmSameCallsWereMade(object proxyObject, VerifierOptions options = VerifierOptions.Default_SameCallsSameNumberOfTimesInAnyOrder)
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
            var serialised = Proxy.SerialiseCachedHistory(proxy);
            File.WriteAllText(filePath, serialised);
        }

        public static void SaveHistoryToFile(object proxyObject, string filePath)
        {
            var serialised = Proxy.SerialiseHistory(proxyObject);
            File.WriteAllText(filePath, serialised);
        }
        #endregion history

        private static T CreateAndStoreProxy<T>(ReplayInterceptor interceptor) where T : class
        {
            var proxy = new ReplayProxy<T>(interceptor);
            proxyObjectsAndProxies.Add(proxy.Object, proxy);
            return proxy.Object;
        }
        private static T CreateAndStoreProxy<T>(ReplayInterceptor interceptor, T instance) where T : class
        {
            var proxy = new ReplayProxy<T>(interceptor, instance);
            proxyObjectsAndProxies.Add(proxy.Object, proxy);
            return proxy.Object;
        }
    }
}
