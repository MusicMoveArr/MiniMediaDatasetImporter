using System.Xml;
using System.Xml.Serialization;

namespace MiniMediaDatasetImporter.Helpers;

public class ReadXmlHelper
{
    public static async Task ReadByLineAsync<T>(string file, string elementName, Func<T, Task> action)
    {
        using XmlReader reader = XmlReader.Create(file);
        XmlSerializer serializer = new XmlSerializer(typeof(T));

        while (reader.Read())
        {
            try
            {
                if (reader.NodeType == XmlNodeType.Element && string.Equals(reader.Name, elementName))
                {
                    T entity = (T)serializer.Deserialize(reader);
                    await action(entity);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}