namespace System.Deal
{
    using System;
    using System.Instant;
    using System.Threading;

    public interface IDeal
    {
        #region Properties

        object Content { get; set; }

        #endregion

        #region Methods

        void Close();

        object Initiate(bool isAsync = true);

        void Reconnect();

        void SetCallback(IDeputy OnCompleteEvent);

        void SetCallback(string methodName, object classObject);

        #endregion
    }

    public class EthConnection : IDeal
    {
        #region Fields

        private readonly ManualResetEvent completeNotice = new ManualResetEvent(false);
        public IDeputy CompleteEvent;
        public IDeputy EchoEvent;
        private IDeputy connected;
        private IDeputy headerReceived;
        private IDeputy headerSent;
        private IDeputy messageReceived;
        private IDeputy messageSent;
        private bool isAsync = true;

        #endregion

        #region Constructors

        public EthConnection(MemberIdentity ClientIdentity, IDeputy OnCompleteEvent = null, IDeputy OnEchoEvent = null)
        {
            MemberIdentity ci = ClientIdentity;
            ci.Site = ServiceSite.Client;
            EthClient client = new EthClient(ci);
            Transfer = new EthTransfer(ci);

            connected = new EthEvent("Connected", this);
            headerSent = new EthEvent("HeaderSent", this);
            messageSent = new EthEvent("MessageSent", this);
            headerReceived = new EthEvent("HeaderReceived", this);
            messageReceived = new EthEvent("MessageReceived", this);

            client.Connected = connected;
            client.HeaderSent = headerSent;
            client.MessageSent = messageSent;
            client.HeaderReceived = headerReceived;
            client.MessageReceived = messageReceived;

            CompleteEvent = OnCompleteEvent;
            EchoEvent = OnEchoEvent;

            Client = client;

            WriteEcho("Client Connection Created");
        }

        #endregion

        #region Properties

        public object Content
        {
            get { return Transfer.MyHeader.Content; }
            set { Transfer.MyHeader.Content = value; }
        }

        public ITransferContext Context { get; set; }

        public EthTransfer Transfer { get; set; }

        private EthClient Client { get; set; }

        #endregion

        #region Methods

        public void Close()
        {
            Client.Dispose();
        }

        public object Connected(object inetdealclient)
        {
            WriteEcho("Client Connection Established");
            Transfer.MyHeader.Context.Echo = "Client say Hello. ";
            Context = Client.Context;
            Client.Context.Transfer = Transfer;

            IEthClient idc = (IEthClient)inetdealclient;

            idc.Send(MessagePart.Header);

            return idc.Context;
        }

        public object HeaderReceived(object inetdealclient)
        {
            string serverEcho = Transfer.HeaderReceived.Context.Echo;
            WriteEcho(string.Format("Server header received"));
            if (serverEcho != null && serverEcho != "")
                WriteEcho(string.Format("Server echo: {0}", serverEcho));

            IEthClient idc = (IEthClient)inetdealclient;

            if (idc.Context.Close)
                idc.Dispose();
            else
            {
                if (!idc.Context.Synchronic)
                {
                    if (idc.Context.SendMessage)
                        idc.Send(MessagePart.Message);
                }

                if (idc.Context.ReceiveMessage)
                    idc.Receive(MessagePart.Message);
            }

            if (!idc.Context.ReceiveMessage &&
                !idc.Context.SendMessage)
            {
                if (CompleteEvent != null)
                    CompleteEvent.Execute(idc.Context);
                if (!isAsync)
                    completeNotice.Set();
            }

            return idc.Context;
        }

        public object HeaderSent(object inetdealclient)
        {
            WriteEcho("Client header sent");
            IEthClient idc = (IEthClient)inetdealclient;
            if (!idc.Context.Synchronic)
                idc.Receive(MessagePart.Header);
            else
                idc.Send(MessagePart.Message);

            return idc.Context;
        }

        public object Initiate(bool IsAsync = true)
        {
            isAsync = IsAsync;
            Client.Connect();
            if (!isAsync)
            {
                completeNotice.WaitOne();
                return Context;
            }

            return null;
        }

        public object MessageReceived(object inetdealclient)
        {
            WriteEcho(string.Format("Server message received"));

            ITransferContext context = ((IEthClient)inetdealclient).Context;
            if (context.Close)
                ((IEthClient)inetdealclient).Dispose();

            if (CompleteEvent != null)
                CompleteEvent.Execute(context);
            if (!isAsync)
                completeNotice.Set();
            return context;
        }

        public object MessageSent(object inetdealclient)
        {
            WriteEcho("Client message sent");

            IEthClient idc = (IEthClient)inetdealclient;
            if (idc.Context.Synchronic)
                idc.Receive(MessagePart.Header);

            if (!idc.Context.ReceiveMessage)
            {
                if (CompleteEvent != null)
                    CompleteEvent.Execute(idc.Context);
                if (!isAsync)
                    completeNotice.Set();
            }

            return idc.Context;
        }

        public void Reconnect()
        {
            MemberIdentity ci = new MemberIdentity()
            {
                AuthId = Client.Identity.AuthId,
                Site = ServiceSite.Client,
                Name = Client.Identity.Name,
                Token = Client.Identity.Token,
                UserId = Client.Identity.UserId,
                DeptId = Client.Identity.DeptId,
                DataPlace = Client.Identity.DataPlace,
                Id = Client.Identity.Id,
                Ip = Client.EndPoint.Address.ToString(),
                Port = Client.EndPoint.Port,
                Key = Client.Identity.Key
            };
            Transfer.Dispose();
            EthClient client = new EthClient(ci);
            Transfer = new EthTransfer(ci);
            client.Connected = connected;
            client.HeaderSent = headerSent;
            client.MessageSent = messageSent;
            client.HeaderReceived = headerReceived;
            client.MessageReceived = messageReceived;
            Client = client;
        }

        public void SetCallback(IDeputy OnCompleteEvent)
        {
            CompleteEvent = OnCompleteEvent;
        }

        public void SetCallback(string methodName, object classObject)
        {
            CompleteEvent = new EthEvent(methodName, classObject);
        }

        private void WriteEcho(string message)
        {
            if (EchoEvent != null)
                EchoEvent.Execute(message);
        }

        #endregion
    }
}