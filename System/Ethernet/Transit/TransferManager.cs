using System;
using System.Linq;

namespace System.Deal
{
    public class TransferManager
    {
        private EthContext context;
        private ServiceSite site;
        private EthTransfer transaction;
        private ITransferContext transferContext;
        private EthManager treatment;

        public TransferManager(EthTransfer _transaction)
        {
            transaction = _transaction;
            transferContext = transaction.Context;
            context = transaction.MyHeader.Context;
            site = context.IdentitySite;
            treatment = new EthManager(_transaction);
        }

        public void HeaderContent(object content, object value, DirectionType _direction)
        {
            DirectionType direction = _direction;
            object _content = value;
            if (_content != null)
            {
                Type[] ifaces = _content.GetType().GetInterfaces();
                if (ifaces.Contains(typeof(ISerialFormatter)))
                {
                    transaction.MyHeader.Context.ContentType = _content.GetType();

                    if (direction == DirectionType.Send)
                        _content = ((ISerialFormatter)value).GetHeader();

                    object[] messages_ = null;
                    if (treatment.Assign(_content, direction, out messages_))
                    {
                        if (messages_.Length > 0)
                        {
                            context.ObjectsCount = messages_.Length;
                            for (int i = 0; i < context.ObjectsCount; i++)
                            {
                                ISerialFormatter message = ((ISerialFormatter[])messages_)[i];
                                ISerialFormatter head = (ISerialFormatter)
                                    ((ISerialFormatter[])messages_)[i].GetHeader();
                                message.SerialCount = message.ItemsCount;
                                head.SerialCount = message.ItemsCount;
                            }

                            if (direction == DirectionType.Send)
                                transaction.MyMessage.Content = messages_;
                            else
                                transaction.MyMessage.Content = (
                                    (ISerialFormatter)_content
                                ).GetHeader();
                        }
                    }
                }
            }
            content = _content;
        }

        public void MessageContent(ref object content, object value, DirectionType _direction)
        {
            DirectionType direction = _direction;
            object _content = value;
            if (_content != null)
            {
                if (direction == DirectionType.Receive)
                {
                    Type[] ifaces = _content.GetType().GetInterfaces();
                    if (ifaces.Contains(typeof(ISerialFormatter)))
                    {
                        object[] messages_ = ((ISerialFormatter)value).GetMessage();
                        if (messages_ != null)
                        {
                            int length = messages_.Length;
                            for (int i = 0; i < length; i++)
                            {
                                ISerialFormatter message = ((ISerialFormatter[])messages_)[i];
                                ISerialFormatter head = (ISerialFormatter)
                                    ((ISerialFormatter[])messages_)[i].GetHeader();
                                message.SerialCount = head.SerialCount;
                                message.DeserialCount = head.DeserialCount;
                            }

                            _content = messages_;
                        }
                    }
                }
            }
            content = _content;
        }
    }
}
