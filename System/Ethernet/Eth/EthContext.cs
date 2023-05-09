using System.Net;

namespace System.Deal
{
    [Serializable]
    public class EthContext
    {
        public string EventClass;
        public string EventMethod;

        [NonSerialized]
        public IPEndPoint LocalEndPoint;

        [NonSerialized]
        public IPEndPoint RemoteEndPoint;

        [NonSerialized]
        private Type contentType;

        public DealComplexity Complexity { get; set; } = DealComplexity.Standard;

        public Type ContentType
        {
            get
            {
                if (contentType == null && ContentTypeName != null)
                    ContentType = Assemblies.FindType(ContentTypeName);
                return contentType;
            }
            set
            {
                if (value != null)
                {
                    ContentTypeName = value.FullName;
                    contentType = value;
                }
            }
        }

        public string ContentTypeName { get; set; }

        public string Echo { get; set; }

        public int Errors { get; set; }

        public MemberIdentity Identity { get; set; }

        public ServiceSite IdentitySite { get; set; } = ServiceSite.Client;

        public int ObjectsCount { get; set; } = 0;

        public bool ReceiveMessage { get; set; } = true;

        public bool SendMessage { get; set; } = true;

        public bool Synchronic { get; set; } = false;
    }
}
