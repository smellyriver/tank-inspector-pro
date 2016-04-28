using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace Smellyriver.TankInspector.Common.Serialization
{
    public static class Serializer
    {
        public static bool IsDeserializing { get; private set; }

        public static void EnsureDeserializing()
        {
            if (!IsDeserializing)
                throw new InvalidOperationException("this operation only valid when deserializing");
        }

        public static T Deserialize<T>(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            var stream = new StreamReader(filename);

            IsDeserializing = true;
            T result = (T)serializer.Deserialize(stream);
            IsDeserializing = false;

            stream.Close();

            return result;
        }

        public static void Serialize(object target, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(target.GetType());
            var stream = new StreamWriter(filename);
            serializer.Serialize(stream, target);
            stream.Close();
        }

        public static T Deserialize<T>(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            IsDeserializing = true;
            T result = (T)serializer.Deserialize(reader);
            IsDeserializing = false;

            return result;
        }

        public static void Serialize<T>(XmlWriter writer, T target)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(writer, target);
        }

        public static void DataContractSerialize(object target, string filename)
        {
            var settings = new XmlWriterSettings { Indent = true };
            using (var writer = XmlWriter.Create(filename, settings))
            {
                var serializer = new DataContractSerializer(target.GetType());
                serializer.WriteObject(writer, target);
            }
        }

        public static T DataContractDeserialize<T>(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(stream);
            }
        }

        public static void BinarySerialize(object target, string filename)
        {
            Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            try
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, target);
                stream.Close();
            }
            catch (Exception)
            {
                stream.Close();
                throw;
            }
        }

        public static T BinaryDeserialize<T>(string filename)
        {
            Stream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                IFormatter formatter = new BinaryFormatter();
                IsDeserializing = true;
                T obj = (T)formatter.Deserialize(stream);
                IsDeserializing = false;
                stream.Close();
                return obj;
            }
            catch (Exception)
            {
                stream.Close();
                throw;
            }
        }
    }
}
