namespace System.Deal
{
    public class EthTransfer : IDisposable
    {
        public ITransferContext Context;
        public EthHeader HeaderReceived;
        public MemberIdentity Identity;
        public TransferManager Manager;
        public EthMessage MessageReceived;
        public EthHeader MyHeader;
        private EthMessage mymessage;

        public EthTransfer()
        {
            MyHeader = new EthHeader(this);
            Manager = new TransferManager(this);
            MyMessage = new EthMessage(this, DirectionType.Send, null);
        }

        public EthTransfer(
            MemberIdentity identity,
            object message = null,
            ITransferContext context = null
        )
        {
            Context = context;
            if (Context != null)
                MyHeader = new EthHeader(this, Context, identity);
            else
                MyHeader = new EthHeader(this, identity);

            Identity = identity;
            Manager = new TransferManager(this);
            MyMessage = new EthMessage(this, DirectionType.Send, message);
        }

        public EthMessage MyMessage
        {
            get { return mymessage; }
            set { mymessage = value; }
        }

        public void Dispose()
        {
            if (MyHeader != null)
                MyHeader.Dispose();
            if (mymessage != null)
                mymessage.Dispose();
            if (HeaderReceived != null)
                HeaderReceived.Dispose();
            if (MessageReceived != null)
                MessageReceived.Dispose();
            if (Context != null)
                Context.Dispose();
        }
    }
}
