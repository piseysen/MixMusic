using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace MixMusic.Serialization
{
    /// <summary>
    /// Helper class for serialization.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Deserializes the specified XML.
        /// </summary>
        /// <typeparam name="T">Target type.</typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(string xml)
        {
            using (var stream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;

                var deserializer = new DataContractSerializer(typeof(T));

                return (T)deserializer.ReadObject(stream);
            }
        }

        /// <summary>
        /// Serializes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The string.</returns>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static string Serialize(object obj)
        {
            using (var memoryStream = new MemoryStream())
            using (var reader = new StreamReader(memoryStream))
            {
                var serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;

                return reader.ReadToEnd();
            }
        }
    }
}
