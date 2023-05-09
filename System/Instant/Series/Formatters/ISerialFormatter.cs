namespace System
{
    using System.IO;

    public interface ISerialFormatter
    {
        int DeserialCount { get; set; }

        int ItemsCount { get; }

        int ProgressCount { get; set; }

        int SerialCount { get; set; }

        object Deserialize(ISerialBuffer buffer, SerialFormat serialFormat = SerialFormat.Binary);

        object Deserialize(Stream stream, SerialFormat serialFormat = SerialFormat.Binary);

        object GetHeader();

        object[] GetMessage();

        int Serialize(
            ISerialBuffer buffer,
            int offset,
            int batchSize,
            SerialFormat serialFormat = SerialFormat.Binary
        );

        int Serialize(
            Stream stream,
            int offset,
            int batchSize,
            SerialFormat serialFormat = SerialFormat.Binary
        );
    }
}
