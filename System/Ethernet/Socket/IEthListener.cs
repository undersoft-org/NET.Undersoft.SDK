namespace System.Deal
{
    using System;

    public interface IEthListener : IDisposable
    {
        IDeputy HeaderReceived { get; set; }

        IDeputy HeaderSent { get; set; }

        MemberIdentity Identity { get; set; }

        IDeputy MessageReceived { get; set; }

        IDeputy MessageSent { get; set; }

        IDeputy SendEcho { get; set; }

        void ClearClients();

        void ClearResources();

        void CloseClient(int id);

        void CloseListener();

        void Echo(string message);

        void HeaderReceivedCallback(IAsyncResult result);

        void HeaderSentCallback(IAsyncResult result);

        bool IsConnected(int id);

        void MessageReceivedCallback(IAsyncResult result);

        void MessageSentCallback(IAsyncResult result);

        void OnConnectCallback(IAsyncResult result);

        void Receive(MessagePart messagePart, int id);

        void Send(MessagePart messagePart, int id);

        void StartListening();
    }
}
