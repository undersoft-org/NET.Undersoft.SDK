namespace System.Threading.Workflow
{
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Uniques;

    public class WorkItem : Task<object>, IDeputy, IWorker
    {
        public IUnique Empty => Uscn.Empty;

        public WorkItem(IDeputy operation) : base(() => operation.Execute())
        {
            Name = operation.Name;
            Worker = new Worker(operation.Name, operation);
            Worker.Work = this;
            Box = new NoteBox(Worker.Name);
            Box.Work = this;

            SerialCode = new Uscn(Name.UniqueKey(), Unique.New);
        }

        public WorkItem(Worker worker) : base(() => worker.Process.Execute())
        {
            Name = worker.Name;
            Worker = worker;
            Worker.Work = this;
            Box = new NoteBox(Worker.Name);
            Box.Work = this;

            SerialCode = new Uscn(Name.UniqueKey(), Unique.New);
        }

        public string Name { get; set; }

        public string QualifiedName { get; set; }

        public Worker Worker { get; set; }

        public Aspect Aspect { get; set; }

        public Aspects Case { get; set; }

        public NoteBox Box { get; set; }

        public object[] ParameterValues
        {
            get => Worker.Process.ParameterValues;
            set => Worker.Process.ParameterValues = value;
        }
        public object this[int fieldId]
        {
            get => ParameterValues[fieldId];
            set => ParameterValues[fieldId] = value;
        }
        public object this[string propertyName]
        {
            get
            {
                for (int i = 0; i < Parameters.Length; i++)
                    if (Parameters[i].Name == propertyName)
                        return ParameterValues[i];
                return null;
            }
            set
            {
                for (int i = 0; i < Parameters.Length; i++)
                    if (Parameters[i].Name == propertyName)
                        ParameterValues[i] = value;
            }
        }

        public MethodInfo Info
        {
            get => Worker.Process.Info;
            set => Worker.Process.Info = value;
        }

        public ParameterInfo[] Parameters
        {
            get => Worker.Process.Parameters;
            set => Worker.Process.Parameters = value;
        }
        public object[] ValueArray
        {
            get => ParameterValues;
            set => ParameterValues = value;
        }

        public Uscn SerialCode { get; set; }

        public ulong UniqueKey
        {
            get => SerialCode.UniqueKey;
            set => SerialCode.SetUniqueKey(value);
        }

        public ulong UniqueType
        {
            get => SerialCode.UniqueType;
            set => SerialCode.SetUniqueSeed(value);
        }

        public MethodDeputy MethodDeputy => Process.MethodDeputy;

        public Delegate Method => Process.Method;

        public Aspect Run(params object[] input)
        {
            Worker.SetInput(input);
            Aspect.Run(this);
            return Aspect;
        }

        public object Execute(params object[] parameters)
        {
            this.Run(parameters);
            return null;
        }

        public byte[] GetBytes()
        {
            return Worker.Process.GetBytes();
        }

        public byte[] GetUniqueBytes()
        {
            return SerialCode.GetUniqueBytes();
        }

        public bool Equals(IUnique other)
        {
            return SerialCode.Equals(other);
        }

        public int CompareTo(IUnique other)
        {
            return SerialCode.CompareTo(other);
        }

        public Task<object> ExecuteAsync(params object[] parameters)
        {
            return Task.Run(() =>
            {
                return Execute(parameters);
            });
        }

        public Task<T> ExecuteAsync<T>(params object[] parameters)
        {
            return Task.Run(() =>
            {
                Execute(parameters);
                return default(T);
            });
        }

        public NoteEvokers Evokers
        {
            get => Worker.Evokers;
            set => Worker.Evokers = value;
        }

        public string WorkerName
        {
            get => Worker.Name;
            set => Worker.Name = value;
        }

        public IDeputy Process
        {
            get => Worker.Process;
            set => Worker.Process = value;
        }
        public object TargetObject
        {
            get => Process.TargetObject;
            set => Process.TargetObject = value;
        }

        public Aspect FlowTo<T>()
        {
            FlowTo(typeof(T).Name);
            return Aspect;
        }

        public Aspect FlowTo(WorkItem Recipient)
        {
            ulong recipientKey = Recipient.Name.UniqueKey();

            var relationWorks = Aspect
                .AsValues()
                .Where(l => l.Worker.Evokers.ContainsKey(l.Name.UniqueKey(recipientKey)))
                .ToArray();

            var evokers = relationWorks
                .Select(l => l.Evokers.Get(l.Name.UniqueKey(recipientKey)))
                .ToArray();
            foreach (var noteEvoker in evokers)
            {
                noteEvoker.RelatedWorks.Put(this);
                noteEvoker.RelatedWorkNames.Put(Name);
            }

            Worker.FlowTo(Recipient, relationWorks.Concat(new[] { this }).ToArray());

            return Aspect;
        }

        public Aspect FlowTo(WorkItem Recipient, params WorkItem[] RelationWorks)
        {
            Worker.FlowTo(Recipient, RelationWorks.Concat(new[] { this }).ToArray());
            return Aspect;
        }

        public Aspect FlowTo(string RecipientName)
        {
            var recipient = Case.AsValues()
                .Where(m => m.ContainsKey(RecipientName))
                .SelectMany(os => os.AsValues())
                .FirstOrDefault();

            ulong recipientKey = RecipientName.UniqueKey();

            var relationWorks = Aspect
                .AsValues()
                .Where(l => l.Worker.Evokers.ContainsKey(l.Name.UniqueKey(recipientKey)))
                .ToArray();

            var evokers = relationWorks
                .Select(l => l.Evokers.Get(l.Name.UniqueKey(recipientKey)))
                .ToArray();
            foreach (var noteEvoker in evokers)
            {
                noteEvoker.RelatedWorks.Put(this);
                noteEvoker.RelatedWorkNames.Put(Name);
            }

            Worker.FlowTo(recipient, relationWorks.Concat(new[] { this }).ToArray());
            return Aspect;
        }

        public Aspect FlowTo(string RecipientName, params string[] RelationNames)
        {
            Worker.FlowTo(RecipientName, RelationNames.Concat(new[] { this.Name }).ToArray());
            return Aspect;
        }

        public Aspect FlowFrom<T>()
        {
            FlowFrom(typeof(T).Name);
            return Aspect;
        }

        public Aspect FlowFrom(WorkItem Sender)
        {
            Sender.FlowTo(this);
            return Aspect;
        }

        public Aspect FlowFrom(WorkItem Sender, params WorkItem[] RelationWorks)
        {
            Sender.FlowTo(this, RelationWorks);
            return Aspect;
        }

        public Aspect FlowFrom(string SenderName)
        {
            var sender = Case.AsValues()
                .Where(m => m.ContainsKey(SenderName))
                .SelectMany(os => os.AsValues())
                .FirstOrDefault();

            sender.FlowTo(this);
            return Aspect;
        }

        public Aspect FlowFrom(string SenderName, params string[] RelationNames)
        {
            var sender = Case.AsValues()
                .Where(m => m.ContainsKey(SenderName))
                .SelectMany(os => os.AsValues())
                .FirstOrDefault();

            sender.FlowTo(this.Name, RelationNames);
            return Aspect;
        }

        public virtual Aspect AddWork<T>() where T : class
        {
            return Aspect.AddWork<T>();
        }

        public virtual Aspect AddWork<T>(Type[] arguments) where T : class
        {
            return Aspect.AddWork<T>(arguments);
        }

        public virtual Aspect AddWork<T>(params object[] consrtuctorParams) where T : class
        {
            return Aspect.AddWork<T>(consrtuctorParams);
        }

        public virtual Aspect AddWork<T>(Type[] arguments, params object[] consrtuctorParams)
            where T : class
        {
            return Aspect.AddWork<T>(arguments, consrtuctorParams);
        }

        public virtual Aspect AddWork<T>(Func<T, string> method) where T : class
        {
            return Aspect.AddWork<T>(method);
        }

        public virtual Aspect AddWork<T>(Func<T, string> method, params Type[] arguments)
            where T : class
        {
            return Aspect.AddWork<T>(method, arguments);
        }

        public virtual Aspect AddWork<T>(Func<T, string> method, params object[] constructorParams)
            where T : class
        {
            return Aspect.AddWork<T>(method, constructorParams);
        }

        public virtual WorkItem Work<T>() where T : class
        {
            return Aspect.Work<T>();
        }

        public virtual WorkItem Work<T>(Type[] arguments) where T : class
        {
            return Aspect.Work<T>(arguments);
        }

        public virtual WorkItem Work<T>(params object[] consrtuctorParams) where T : class
        {
            return Aspect.Work<T>(consrtuctorParams);
        }

        public virtual WorkItem Work<T>(Type[] arguments, params object[] consrtuctorParams)
            where T : class
        {
            return Aspect.Work<T>(arguments, consrtuctorParams);
        }

        public virtual WorkItem Work<T>(Func<T, string> method) where T : class
        {
            return Aspect.Work<T>(method);
        }

        public virtual WorkItem Work<T>(Func<T, string> method, params Type[] arguments)
            where T : class
        {
            return Aspect.Work<T>(method, arguments);
        }

        public virtual WorkItem Work<T>(Func<T, string> method, params object[] constructorParams)
            where T : class
        {
            return Aspect.Work<T>(method, constructorParams);
        }

        public Aspect Allocate(int laborsCount = 1)
        {
            return Aspect.Allocate(laborsCount);
        }

        public object GetInput()
        {
            return ((IWorker)Worker).GetInput();
        }

        public void SetInput(object value)
        {
            ((IWorker)Worker).SetInput(value);
        }

        public object GetOutput()
        {
            return ((IWorker)Worker).GetOutput();
        }

        public void SetOutput(object value)
        {
            ((IWorker)Worker).SetOutput(value);
        }

        public Task Publish(params object[] parameters)
        {
            return Process.Publish(parameters);
        }

        public object Execute(object target, params object[] parameters)
        {
            return Process.Execute(target, parameters);
        }

        public Task<object> ExecuteAsync(object target, params object[] parameters)
        {
            return Process.ExecuteAsync(target, parameters);
        }

        public Task<T> ExecuteAsync<T>(object target, params object[] parameters)
        {
            return Process.ExecuteAsync<T>(target, parameters);
        }

        public Task Publish(bool firstAsTarget, object target, params object[] parameters)
        {
            return Process.Publish(firstAsTarget, target, parameters);
        }

        public object Execute(bool firstAsTarget, object target, params object[] parameters)
        {
            return Process.Execute(firstAsTarget, target, parameters);
        }

        public Task<object> ExecuteAsync(
            bool firstAsTarget,
            object target,
            params object[] parameters
        )
        {
            return Process.ExecuteAsync(firstAsTarget, target, parameters);
        }

        public Task<T> ExecuteAsync<T>(
            bool firstAsTarget,
            object target,
            params object[] parameters
        )
        {
            return Process.ExecuteAsync<T>(firstAsTarget, target, parameters);
        }
    }
}
