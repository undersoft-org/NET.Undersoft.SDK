using System;
using System.Instant;

namespace System.Deal
{
    public interface IEthClient : IDisposable
    {
        IDeputy Connected { get; set; }

        ITransferContext Context { get; set; }

        IDeputy HeaderReceived { get; set; }

        IDeputy HeaderSent { get; set; }

        IDeputy MessageReceived { get; set; }

        IDeputy MessageSent { get; set; }

        void Connect();

        bool IsConnected();

        void Receive(MessagePart messagePart);

        void Send(MessagePart messagePart);
    }
}
