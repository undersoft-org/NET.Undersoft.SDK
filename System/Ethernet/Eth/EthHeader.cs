using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace System.Deal
{
    public static class BinaryHeader
    {
        public static EthHeader GetBinary(this EthHeader bank, ISerialBuffer buffer)
        {
            try
            {
                //MemoryStream ms = new MemoryStream(buffer.DeserialBlock);
                //BinaryFormatter binform = new BinaryFormatter();
                //EthHeader _bank = (EthHeader)binform.Deserialize(ms);
                //ms.Dispose();
                //return _bank;
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EthHeader GetBinary(this EthHeader bank, Stream fromstream)
        {
            try
            {
                //BinaryFormatter binform = new BinaryFormatter();
                //return (EthHeader)binform.Deserialize(fromstream);
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static int SetBinary(this EthHeader bank, ISerialBuffer buffer)
        {
            //MemoryStream ms = new MemoryStream();
            //ms.Write(new byte[buffer.BlockOffset], 0, buffer.BlockOffset);
            //BinaryFormatter binform = new BinaryFormatter();
            //binform.Serialize(ms, bank);
            //buffer.SerialBlock = ms.ToArray();
            //ms.Dispose();
            //return buffer.SerialBlock.Length;
            return default;
        }

        public static int SetBinary(this EthHeader bank, Stream tostream)
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
    public class EthHeader : ISerialFormatter, IDisposable
    {
        [NonSerialized]
        private EthTransfer transaction;

        public EthHeader()
        {
            Context = new EthContext();
            SerialCount = 0;
            DeserialCount = 0;
        }

        public EthHeader(EthTransfer _transaction)
        {
            Context = new EthContext();
            transaction = _transaction;
            SerialCount = 0;
            DeserialCount = 0;
        }

        public EthHeader(EthTransfer _transaction, ITransferContext context)
        {
            Context = new EthContext();
            Context.LocalEndPoint = (IPEndPoint)context.Listener.LocalEndPoint;
            Context.RemoteEndPoint = (IPEndPoint)context.Listener.RemoteEndPoint;
            transaction = _transaction;
            SerialCount = 0;
            DeserialCount = 0;
        }

        public EthHeader(
            EthTransfer _transaction,
            ITransferContext context,
            MemberIdentity identity
        )
        {
            Context = new EthContext();
            Context.LocalEndPoint = (IPEndPoint)context.Listener.LocalEndPoint;
            Context.RemoteEndPoint = (IPEndPoint)context.Listener.RemoteEndPoint;
            Context.Identity = identity;
            Context.IdentitySite = identity.Site;
            transaction = _transaction;
            SerialCount = 0;
            DeserialCount = 0;
        }

        public EthHeader(EthTransfer _transaction, MemberIdentity identity)
        {
            Context = new EthContext();
            Context.Identity = identity;
            Context.IdentitySite = identity.Site;
            transaction = _transaction;
            SerialCount = 0;
            DeserialCount = 0;
        }

        public object Content { get; set; }

        public EthContext Context { get; set; }

        public int DeserialCount { get; set; }

        public int ItemsCount
        {
            get { return Context.ObjectsCount; }
        }

        public int ProgressCount { get; set; }

        public int SerialCount { get; set; }

        public void BindContext(ITransferContext context)
        {
            Context.LocalEndPoint = (IPEndPoint)context.Listener.LocalEndPoint;
            Context.RemoteEndPoint = (IPEndPoint)context.Listener.RemoteEndPoint;
        }

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
                return null;
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
                return null;
        }

        public void Dispose()
        {
            Content = null;
        }

        public object GetHeader()
        {
            return this;
        }

        public object[] GetMessage()
        {
            return null;
        }

        public int Serialize(
            ISerialBuffer buffer,
            int offset,
            int batchSize,
            SerialFormat serialFormat = SerialFormat.Binary
        )
        {
            if (serialFormat == SerialFormat.Binary)
                return this.SetBinary(buffer);
            else if (serialFormat == SerialFormat.Json)
                return this.SetJson(buffer);
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

    public static class JsonHeader
    {
        public static EthHeader GetJson(this EthHeader thdr, ISerialBuffer buffer)
        {
            try
            {
                EthHeader trs = null;

                byte[] _fromarray = buffer.DeserialBlock;
                StringBuilder sb = new StringBuilder();

                sb.Append(_fromarray.ToChars(CharEncoding.UTF8));
                trs = thdr.GetJsonObject(sb.ToString())["DealHeader"];

                _fromarray = null;
                sb = null;
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EthHeader GetJson(this EthHeader thdr, Stream fromstream)
        {
            try
            {
                fromstream.Position = 0;
                byte[] array = new byte[4096];
                int read = 0;
                StringBuilder sb = new StringBuilder();
                while ((read = fromstream.Read(array, 0, array.Length)) > 0)
                {
                    sb.Append(array.Select(b => (char)b).ToArray());
                }
                EthHeader trs = thdr.GetJsonObject(sb.ToString())["DealHeader"];
                sb = null;
                fromstream.Dispose();
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EthHeader GetJson(this EthHeader thdr, string jsonstring)
        {
            try
            {
                EthHeader trs = thdr.GetJsonObject(jsonstring)["DealHeader"];
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static EthHeader GetJson(this EthHeader thdr, StringBuilder stringbuilder)
        {
            try
            {
                StringBuilder sb = stringbuilder;
                EthHeader trs = thdr.GetJsonObject(sb.ToString())["DealHeader"];
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static void GetJsonBag(
            this EthHeader thdr,
            string JsonString,
            IDictionary<string, object> _bag
        )
        {
            _bag.Add(JsonParser.FromJson(JsonString));
        }

        public static Dictionary<string, EthHeader> GetJsonObject(
            this EthHeader thdr,
            IDictionary<string, object> _bag
        )
        {
            Dictionary<string, EthHeader> result = new Dictionary<string, EthHeader>();
            IDictionary<string, object> bags = _bag;
            foreach (KeyValuePair<string, object> bag in bags)
            {
                object inst = new object();
                IEnumerable<PropertyInfo> map = JsonParser.PrepareInstance(
                    out inst,
                    typeof(EthHeader)
                );
                JsonParser.DeserializeType(map, (IDictionary<string, object>)bag.Value, inst);
                EthHeader deck = (EthHeader)inst;
                result.Add(bag.Key, deck);
            }
            return result;
        }

        public static Dictionary<string, EthHeader> GetJsonObject(
            this EthHeader thdr,
            string JsonString
        )
        {
            Dictionary<string, EthHeader> result = new Dictionary<string, EthHeader>();
            Dictionary<string, object> bags = new Dictionary<string, object>();
            thdr.GetJsonBag(JsonString, bags);

            foreach (KeyValuePair<string, object> bag in bags)
            {
                object inst = new object();
                IEnumerable<PropertyInfo> map = JsonParser.PrepareInstance(
                    out inst,
                    typeof(EthHeader)
                );
                JsonParser.DeserializeType(map, (IDictionary<string, object>)bag.Value, inst);
                EthHeader deck = (EthHeader)inst;
                result.Add(bag.Key, deck);
            }
            return result;
        }

        public static int SetJson(this EthHeader thdr, ISerialBuffer buffer, int offset = 0)
        {
            if (offset > 0)
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(thdr.SetJsonString());
                byte[] serialBytes = new byte[jsonBytes.Length + offset];
                jsonBytes.CopyTo(serialBytes, offset);
                buffer.SerialBlock = serialBytes;
                jsonBytes = null;
            }
            else
                buffer.SerialBlock = Encoding.UTF8.GetBytes(thdr.SetJsonString());

            return buffer.SerialBlock.Length;
        }

        public static int SetJson(this EthHeader thdr, Stream tostream)
        {
            BinaryWriter binwriter = new BinaryWriter(tostream);
            binwriter.Write(thdr.SetJsonString());
            return (int)tostream.Length;
        }

        public static int SetJson(this EthHeader thdr, StringBuilder stringbuilder)
        {
            stringbuilder.AppendLine(thdr.SetJsonString());
            return stringbuilder.Length;
        }

        public static IDictionary<string, object> SetJsonBag(this EthHeader thdr)
        {
            return new Dictionary<string, object>()
            {
                {
                    "DealHeader",
                    JsonParserProperties
                        .GetJsonProperties(typeof(EthHeader), thdr.Context.Complexity)
                        .Select(
                            k => new KeyValuePair<string, object>(k.Name, k.GetValue(thdr, null))
                        )
                        .ToDictionary(k => k.Key, v => v.Value)
                }
            };
        }

        public static string SetJsonString(this EthHeader thdr)
        {
            IDictionary<string, object> toJson = thdr.SetJsonBag();
            return JsonParser.ToJson(toJson, thdr.Context.Complexity);
        }

        public static string SetJsonString(this EthHeader thdr, IDictionary<string, object> jsonbag)
        {
            return JsonParser.ToJson(jsonbag, thdr.Context.Complexity);
        }
    }
}
