namespace System.Threading.Workflow
{
    using Series;
    using Uniques;

    public class Worker : IUnique, IWorker
    {
        private readonly Catalog<object> input = new Catalog<object>(true);
        private readonly Catalog<object> output = new Catalog<object>(true);
        private Uscn SerialCode;

        private Worker() { }

        public Worker(string Name, IDeputy Method) : this()
        {
            Process = Method;
            this.Name = Name;
            ulong seed = Unique.New;
            SerialCode = new Uscn((Process.UniqueKey).UniqueKey(seed), seed);
        }

        public IUnique Empty => new Ussc();

        public NoteEvokers Evokers { get; set; } = new NoteEvokers();

        public object GetInput()
        {
            object entry;
            input.TryDequeue(out entry);
            return entry;
        }

        public void SetInput(object value)
        {
            input.Enqueue(value);
        }

        public object GetOutput()
        {
            object entry;
            output.TryDequeue(out entry);
            return entry;
        }

        public void SetOutput(object value)
        {
            output.Enqueue(value);
        }

        public WorkItem Work { get; set; }

        public string Name { get; set; }

        public ulong UniqueKey
        {
            get => SerialCode.UniqueKey;
            set => SerialCode.UniqueKey = value;
        }

        public ulong UniqueType
        {
            get => SerialCode.UniqueType;
            set => SerialCode.UniqueType = value;
        }

        public IDeputy Process { get; set; }

        public Aspect FlowTo<T>()
        {
            return Work.FlowTo<T>();
        }

        public Aspect FlowTo(WorkItem recipient)
        {
            Evokers.Add(new NoteEvoker(Work, recipient, Work));
            return Work.Aspect;
        }

        public Aspect FlowTo(WorkItem Recipient, params WorkItem[] RelationWorks)
        {
            Evokers.Add(new NoteEvoker(Work, Recipient, RelationWorks));
            return Work.Aspect;
        }

        public Aspect FlowTo(string RecipientName)
        {
            Evokers.Add(new NoteEvoker(Work, RecipientName, Name));
            return Work.Aspect;
        }

        public Aspect FlowTo(string RecipientName, params string[] RelationNames)
        {
            Evokers.Add(new NoteEvoker(Work, RecipientName, RelationNames));
            return Work.Aspect;
        }

        public Aspect FlowFrom<T>()
        {
            return Work.FlowFrom<T>();
        }

        public Aspect FlowFrom(WorkItem sender)
        {
            Work.FlowFrom(sender);
            return Work.Aspect;
        }

        public Aspect FlowFrom(WorkItem Sender, params WorkItem[] RelationWorks)
        {
            Work.FlowFrom(Sender, RelationWorks);
            return Work.Aspect;
        }

        public Aspect FlowFrom(string SenderName)
        {
            Work.FlowFrom(SenderName);
            return Work.Aspect;
        }

        public Aspect FlowFrom(string SenderName, params string[] RelationNames)
        {
            Work.FlowFrom(SenderName, RelationNames);
            return Work.Aspect;
        }

        public int CompareTo(IUnique other)
        {
            return SerialCode.CompareTo(other);
        }

        public bool Equals(IUnique other)
        {
            return SerialCode.Equals(other);
        }

        public byte[] GetBytes()
        {
            return SerialCode.GetBytes();
        }

        public byte[] GetUniqueBytes()
        {
            return SerialCode.GetUniqueBytes();
        }
    }
}
