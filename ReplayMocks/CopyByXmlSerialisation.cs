namespace ReplayMocks
{
    public interface ICopier<T>
    {
        T Copy(T source);
    }

    public class CopyByXmlSerialisation : ICopier<object>
    {
        public object Copy(object source)
        {
            if (source == null)
                return null;
            var asXml = (string)XmlSerialiser.SerialiseWithoutNamespacesAndHeaderWithoutLinebreaks(source, source.GetType());

            return XmlSerialiser.Deserialise(asXml, source.GetType());
        }
    }
}