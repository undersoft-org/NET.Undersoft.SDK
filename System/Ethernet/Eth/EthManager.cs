namespace System.Deal
{
    public class EthManager : IDisposable
    {
        public ITransferContext transferContext;
        private EthContext dealContext;
        private ServiceSite site;
        private EthTransfer transfer;

        public EthManager(EthTransfer dealTransfer)
        {
            transfer = dealTransfer;
            transferContext = dealTransfer.Context;
            dealContext = dealTransfer.MyHeader.Context;
            site = dealContext.IdentitySite;
        }

        public bool Assign(object content, DirectionType direction, out object[] messages)
        {
            messages = null;

            EthOperation operation = new EthOperation(content, direction, transfer);
            operation.Resolve(out messages);

            if (messages != null)
                return true;
            else
                return false;
        }

        public void Dispose()
        {
            if (transferContext != null)
                transferContext.Dispose();
        }
    }
}
