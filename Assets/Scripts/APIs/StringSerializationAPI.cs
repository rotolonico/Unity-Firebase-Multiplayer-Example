using System;
using FullSerializer;

namespace APIs
{
    public static class StringSerializationAPI {
        private static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize(Type type, object value) {
            // serialize the data
            fsData data;
            _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }

        public static object Deserialize(Type type, string serializedState)
        {
            if (serializedState == null) return null;
            
            serializedState = serializedState.Replace('|', '$'); // Because Firebase doesn't accept the $ symbol
            
            // step 1: parse the JSON data
            fsData data = fsJsonParser.Parse(serializedState);

            // step 2: deserialize the data
            object deserialized = null;
            _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

            return deserialized;
        }
    }
}
