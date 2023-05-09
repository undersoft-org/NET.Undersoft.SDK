namespace System.Threading.Workflow
{
    using System.Uniques;

    public class Note : IUnique
    {
        public object[] Parameters;
        public NoteBox SenderBox;

        public Note(
            WorkItem sender,
            WorkItem recipient,
            NoteEvoker Out,
            NoteEvokers In,
            params object[] Params
        )
        {
            Parameters = Params;

            if (recipient != null)
            {
                Recipient = recipient;
                RecipientName = Recipient.Worker.Name;
            }

            Sender = sender;
            SenderName = Sender.Worker.Name;

            if (Out != null)
                EvokerOut = Out;

            if (In != null)
                EvokersIn = In;
        }

        public Note(string sender, params object[] Params) : this(sender, null, null, null, Params)
        { }

        public Note(
            string sender,
            string recipient,
            NoteEvoker Out,
            NoteEvokers In,
            params object[] Params
        )
        {
            SenderName = sender;
            Parameters = Params;

            if (recipient != null)
                RecipientName = recipient;

            if (Out != null)
                EvokerOut = Out;

            if (In != null)
                EvokersIn = In;
        }

        public Note(string sender, string recipient, NoteEvoker Out, params object[] Params)
            : this(sender, recipient, Out, null, Params) { }

        public Note(string sender, string recipient, params object[] Params)
            : this(sender, recipient, null, null, Params) { }

        public IUnique Empty => new Ussc();

        public NoteEvoker EvokerOut { get; set; }

        public NoteEvokers EvokersIn { get; set; }

        public WorkItem Recipient { get; set; }

        public string RecipientName { get; set; }

        public WorkItem Sender { get; set; }

        public string SenderName { get; set; }

        public ulong UniqueKey
        {
            get => Sender.UniqueKey;
            set => Sender.UniqueKey = value;
        }

        public ulong UniqueType
        {
            get => ((IUnique)Sender).UniqueType;
            set => ((IUnique)Sender).UniqueType = value;
        }

        public int CompareTo(IUnique other)
        {
            return Sender.CompareTo(other);
        }

        public bool Equals(IUnique other)
        {
            return Sender.Equals(other);
        }

        public byte[] GetBytes()
        {
            return Sender.GetBytes();
        }

        public byte[] GetUniqueBytes()
        {
            return Sender.GetUniqueBytes();
        }
    }
}
