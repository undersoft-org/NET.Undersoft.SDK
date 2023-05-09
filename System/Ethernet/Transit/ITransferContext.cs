using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Series;
using System.Text;
using System.Threading;

namespace System.Deal
{
    public interface ITransferContext : ISerialBuffer
    {
        ManualResetEvent BatchesReceivedNotice { get; set; }

        int BufferSize { get; }

        bool Close { get; set; }

        bool Denied { get; set; }

        string Echo { get; }

        byte[] HeaderBuffer { get; }

        ManualResetEvent HeaderReceivedNotice { get; set; }

        ManualResetEvent HeaderSentNotice { get; set; }

        Hashtable HttpHeaders { get; set; }

        Hashtable HttpOptions { get; set; }

        int Id { get; set; }

        Socket Listener { get; set; }

        byte[] MessageBuffer { get; }

        ManualResetEvent MessageReceivedNotice { get; set; }

        ManualResetEvent MessageSentNotice { get; set; }

        ProtocolMethod Method { get; set; }

        int ObjectPosition { get; set; }

        int ObjectsLeft { get; set; }

        DealProtocol Protocol { get; set; }

        bool ReceiveMessage { get; set; }

        StringBuilder RequestBuilder { get; set; }

        IDeck<byte[]> Resources { get; set; }

        StringBuilder ResponseBuilder { get; set; }

        bool SendMessage { get; set; }

        bool Synchronic { get; set; }

        EthTransfer Transfer { get; set; }

        void Append(string text);

        void HandleDeniedRequest();

        void HandleGetRequest(string content_type = "text/html");

        void HandleOptionsRequest(string content_type = "text/html");

        void HandlePostRequest(string content_type = "text/html");

        DealProtocol IdentifyProtocol();

        MarkupType IncomingHeader(int received);

        MarkupType IncomingMessage(int received);

        void Reset();
    }
}
