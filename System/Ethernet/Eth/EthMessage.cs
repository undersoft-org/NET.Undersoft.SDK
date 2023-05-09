using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Deal
{
    public static class BinaryMessage
    {
        public static EthMessage GetBinary(this EthMessage bank, ISerialBuffer buffer)
        {
            try
            {
                //MemoryStream ms = new MemoryStream(buffer.DeserialBlock);
                //BinaryFormatter binform = new BinaryFormatter();
                //EthMessage _bank = (EthMessage)binform.Deserialize(ms);
                //ms.Dispose();
                return null;//_bank;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EthMessage GetBinary(this EthMessage bank, Stream fromstream)
        {
            try
            {
                //BinaryFormatter binform = new BinaryFormatter();
                //return (EthMessage)binform.Deserialize(fromstream);
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int SetBinary(this EthMessage bank, ISerialBuffer buffer)
        {
            //int offset = buffer.BlockOffset;
            //MemoryStream ms = new MemoryStream();
            //ms.Write(new byte[offset], 0, offset);
            //BinaryFormatter binform = new BinaryFormatter();
            //binform.Serialize(ms, bank);
            //buffer.SerialBlock = ms.ToArray();
            //ms.Dispose();
            //return buffer.SerialBlock.Length;
            return default;
        }

        public static int SetBinary(this EthMessage bank, Stream tostream)
        {
            //if (tostream == null)
            //    tostream = new MemoryStream();
            //BinaryFormatter binform = new BinaryFormatter();
            //binform.Serialize(tostream, bank);
            //return (int)tostream.Length;
            return default;
        }
    }

    [Serializable]
    public class EthMessage : ISerialFormatter, IDisposable
    {
        private object content;
        private DirectionType direction;

        [NonSerialized]
        private EthTransfer transaction;

        public EthMessage()
        {
            content = new object();
            SerialCount = 0;
            DeserialCount = 0;
            direction = DirectionType.Receive;
        }

        public EthMessage(EthTransfer _transaction, DirectionType _direction, object message = null)
        {
            transaction = _transaction;
            direction = _direction;

            if (message != null)
                Content = message;
            else
                content = new object();

            SerialCount = 0;
            DeserialCount = 0;
        }

        public object Content
        {
            get { return content; }
            set { transaction.Manager.MessageContent(ref content, value, direction); }
        }

        public int DeserialCount { get; set; }

        public int ItemsCount
        {
            get
            {
                return (content != null) ? ((ISerialFormatter[])content).Sum(t => t.ItemsCount) : 0;
            }
        }

        public string Notice { get; set; }

        public int ObjectsCount
        {
            get { return (content != null) ? ((ISerialFormatter[])content).Length : 0; }
        }

        public int ProgressCount { get; set; }

        public int SerialCount { get; set; }

        public object Deserialize(
            ISerialBuffer buffer,
            SerialFormat serialFormat = SerialFormat.Binary
        )
        {
            if (serialFormat == SerialFormat.Binary)
                return this.GetBinary(buffer);
            else if (serialFormat == SerialFormat.Json)
                return this.GetJson(buffer);
            else
                return -1;
        }

        public object Deserialize(
            Stream fromstream,
            SerialFormat serialFormat = SerialFormat.Binary
        )
        {
            if (serialFormat == SerialFormat.Binary)
                return this.GetBinary(fromstream);
            else if (serialFormat == SerialFormat.Json)
                return this.GetJson(fromstream);
            else
                return -1;
        }

        public void Dispose()
        {
            content = null;
        }

        public object GetHeader()
        {
            if (direction == DirectionType.Send)
                return transaction.MyHeader.Content;
            else
                return transaction.HeaderReceived.Content;
        }

        public object[] GetMessage()
        {
            if (content != null)
                return (ISerialFormatter[])content;
            return null;
        }

        public int Serialize(
            ISerialBuffer buffor,
            int offset,
            int batchSize,
            SerialFormat serialFormat = SerialFormat.Binary
        )
        {
            if (serialFormat == SerialFormat.Binary)
                return this.SetBinary(buffor);
            else if (serialFormat == SerialFormat.Json)
                return this.SetJson(buffor);
            else
                return -1;
        }

        public int Serialize(
            Stream tostream,
            int offset,
            int batchSize,
            SerialFormat serialFormat = SerialFormat.Binary
        )
        {
            if (serialFormat == SerialFormat.Binary)
                return this.SetBinary(tostream);
            else if (serialFormat == SerialFormat.Json)
                return this.SetJson(tostream);
            else
                return -1;
        }
    }

    public static class JsonMessage
    {
        public static EthMessage GetJson(this EthMessage tmsg, ISerialBuffer buffer)
        {
            try
            {
                EthMessage trs = null;

                byte[] _fromarray = buffer.DeserialBlock;
                StringBuilder sb = new StringBuilder();

                sb.Append(_fromarray.ToChars(CharEncoding.UTF8));
                trs = tmsg.GetJsonObject(sb.ToString())["DealMessage"];

                _fromarray = null;
                sb = null;
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EthMessage GetJson(this EthMessage tmsg, Stream fromstream)
        {
            try
            {
                fromstream.Position = 0;
                byte[] array = new byte[4096];
                int read = 0;
                StringBuilder sb = new StringBuilder();
                while ((read = fromstream.Read(array, 0, array.Length)) > 0)
                {
                    sb.Append(array.Cast<char>());
                }
                EthMessage trs = tmsg.GetJsonObject(sb.ToString())["DealMessage"];
                sb = null;
                fromstream.Dispose();
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EthMessage GetJson(this EthMessage tmsg, string jsonstring)
        {
            try
            {
                EthMessage trs = tmsg.GetJsonObject(jsonstring)["DealMessage"];
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EthMessage GetJson(this EthMessage tmsg, StringBuilder stringbuilder)
        {
            try
            {
                StringBuilder sb = stringbuilder;
                EthMessage trs = tmsg.GetJsonObject(sb.ToString())["DealMessage"];
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void GetJsonBag(
            this EthMessage tmsg,
            string JsonString,
            IDictionary<string, object> _bag
        )
        {
            _bag.Add(JsonParser.FromJson(JsonString));
        }

        public static Dictionary<string, EthMessage> GetJsonObject(
            this EthMessage tmsg,
            IDictionary<string, object> _bag
        )
        {
            Dictionary<string, EthMessage> result = new Dictionary<string, EthMessage>();
            IDictionary<string, object> bags = _bag;
            foreach (KeyValuePair<string, object> bag in bags)
            {
                object inst = new object();
                IEnumerable<PropertyInfo> map = JsonParser.PrepareInstance(
                    out inst,
                    typeof(EthMessage)
                );
                JsonParser.DeserializeType(map, (IDictionary<string, object>)bag.Value, inst);
                EthMessage deck = (EthMessage)inst;
                result.Add(bag.Key, deck);
            }
            return result;
        }

        public static Dictionary<string, EthMessage> GetJsonObject(
            this EthMessage tmsg,
            string JsonString
        )
        {
            Dictionary<string, EthMessage> result = new Dictionary<string, EthMessage>();
            Dictionary<string, object> bags = new Dictionary<string, object>();
            tmsg.GetJsonBag(JsonString, bags);

            foreach (KeyValuePair<string, object> bag in bags)
            {
                object inst = new object();
                IEnumerable<PropertyInfo> map = JsonParser.PrepareInstance(
                    out inst,
                    typeof(EthMessage)
                );
                JsonParser.DeserializeType(map, (IDictionary<string, object>)bag.Value, inst);
                EthMessage deck = (EthMessage)inst;
                result.Add(bag.Key, deck);
            }
            return result;
        }

        public static int SetJson(this EthMessage tmsg, ISerialBuffer buffer)
        {
            buffer.SerialBlock = Encoding.UTF8.GetBytes(tmsg.SetJsonString());
            return buffer.SerialBlock.Length;
        }

        public static int SetJson(this EthMessage tmsg, Stream tostream)
        {
            BinaryWriter binwriter = new BinaryWriter(tostream);
            binwriter.Write(tmsg.SetJsonString());
            return (int)tostream.Length;
        }

        public static int SetJson(this EthMessage tmsg, StringBuilder stringbuilder)
        {
            stringbuilder.AppendLine(tmsg.SetJsonString());
            return stringbuilder.Length;
        }

        public static Dictionary<string, object> SetJsonBag(this EthMessage tmsg)
        {
            return new Dictionary<string, object>()
            {
                {
                    "DealMessage",
                    JsonParserProperties
                        .GetJsonProperties(typeof(EthMessage))
                        .Select(
                            k => new KeyValuePair<string, object>(k.Name, k.GetValue(tmsg, null))
                        )
                        .ToDictionary(k => k.Key, v => v.Value)
                }
            };
        }

        public static string SetJsonString(this EthMessage tmsg)
        {
            IDictionary<string, object> toJson = tmsg.SetJsonBag();
            return JsonParser.ToJson(toJson);
        }

        public static string SetJsonString(
            this EthMessage tmsg,
            IDictionary<string, object> jsonbag
        )
        {
            return JsonParser.ToJson(jsonbag);
        }
    }
}
