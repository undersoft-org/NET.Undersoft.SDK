namespace System.Threading.Workflow
{
    using Collections.Generic;
    using Linq;
    using Series;

    public class Aspect : Catalog<WorkItem>, IWorkspace
    {
        public Aspects Case { get; set; }

        public Aspect(string Name)
        {
            this.Name = Name;
            WorkersCount = 1;
        }

        public Aspect(string Name, IEnumerable<WorkItem> WorkList) : this(Name)
        {
            foreach (WorkItem labor in WorkList)
            {
                labor.Case = Case;
                labor.Aspect = this;
                Put(labor);
            }
        }

        public Aspect(string Name, IEnumerable<IDeputy> MethodList)
            : this(Name, MethodList.Select(m => new WorkItem(m))) { }

        public int WorkersCount { get; set; }

        public Workspace Workator { get; set; }

        public string Name { get; set; }

        public override WorkItem Get(object key)
        {
            TryGet(key, out WorkItem result);
            return result;
        }

        public WorkItem AddWork(WorkItem labor)
        {
            labor.Case = Case;
            labor.Aspect = this;
            Put(labor);
            return labor;
        }

        public WorkItem AddWork(IDeputy deputy)
        {
            WorkItem labor = new WorkItem(deputy);
            labor.Case = Case;
            labor.Aspect = this;
            Put(labor);
            return labor;
        }

        public Aspect AddWork(IEnumerable<WorkItem> labors)
        {
            foreach (WorkItem labor in labors)
            {
                labor.Case = Case;
                labor.Aspect = this;
                Put(labor);
            }
            return this;
        }

        public Aspect AddWork(IEnumerable<IDeputy> methods)
        {
            foreach (IDeputy method in methods)
            {
                WorkItem labor = new WorkItem(method);
                labor.Case = Case;
                labor.Aspect = this;
                Put(labor);
            }
            return this;
        }

        public virtual Aspect AddWork<T>() where T : class
        {
            var deputy = new Deputy<T>();
            AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            return this;
        }

        public virtual Aspect AddWork<T>(Type[] arguments) where T : class
        {
            var deputy = new Deputy<T>(arguments);
            AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            return this;
        }

        public virtual Aspect AddWork<T>(params object[] consrtuctorParams) where T : class
        {
            var deputy = new Deputy<T>(consrtuctorParams);
            AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            return this;
        }

        public virtual Aspect AddWork<T>(Type[] arguments, params object[] consrtuctorParams)
            where T : class
        {
            var deputy = new Deputy<T>(arguments, consrtuctorParams);
            AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            return this;
        }

        public virtual Aspect AddWork<T>(Func<T, string> method) where T : class
        {
            var deputy = new Deputy<T>(method);
            AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            return this;
        }

        public virtual Aspect AddWork<T>(Func<T, string> method, params Type[] arguments)
            where T : class
        {
            var deputy = new Deputy<T>(method, arguments);
            AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            return this;
        }

        public virtual Aspect AddWork<T>(Func<T, string> method, params object[] consrtuctorParams)
            where T : class
        {
            var deputy = new Deputy<T>(method, consrtuctorParams);
            AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            return this;
        }

        public virtual WorkItem Work<T>() where T : class
        {
            if (!TryGet(Deputy.GetName<T>(), out WorkItem labor))
            {
                var deputy = new Deputy<T>();
                return AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            }
            return labor;
        }

        public virtual WorkItem Work<T>(Type[] arguments) where T : class
        {
            if (!TryGet(Deputy.GetName<T>(arguments), out WorkItem labor))
            {
                var deputy = new Deputy<T>();
                return AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            }
            return labor;
        }

        public virtual WorkItem Work<T>(params object[] consrtuctorParams) where T : class
        {
            if (!TryGet(Deputy.GetName<T>(), out WorkItem labor))
            {
                var deputy = new Deputy<T>(consrtuctorParams);
                return AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            }
            return labor;
        }

        public virtual WorkItem Work<T>(Type[] arguments, params object[] constructorParams)
            where T : class
        {
            if (!TryGet(Deputy.GetName<T>(arguments), out WorkItem labor))
            {
                var deputy = new Deputy<T>(arguments, constructorParams);
                return AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            }
            return labor;
        }

        public virtual WorkItem Work<T>(Func<T, string> method) where T : class
        {
            var deputy = new Deputy<T>(method);
            if (!TryGet(deputy.Name, out WorkItem labor))
            {
                return AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            }
            return labor;
        }

        public virtual WorkItem Work<T>(Func<T, string> method, params Type[] arguments)
            where T : class
        {
            var deputy = new Deputy<T>(method, arguments);
            if (!TryGet(deputy.Name, out WorkItem labor))
            {
                return AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            }
            return labor;
        }

        public virtual WorkItem Work<T>(Func<T, string> method, params object[] consrtuctorParams)
            where T : class
        {
            var deputy = new Deputy<T>(method, consrtuctorParams);
            if (!TryGet(deputy.Name, out WorkItem labor))
            {
                return AddWork(Case.Methods.EnsureGet(deputy, k => deputy).Value);
            }
            return labor;
        }

        public override WorkItem this[object key]
        {
            get => base[key];
            set
            {
                value.Case = Case;
                value.Aspect = this;
                base[key] = value;
            }
        }

        public void Close(bool SafeClose)
        {
            Workator.Close(SafeClose);
        }

        public Aspect Allocate(int workersCount = 1)
        {
            Workator.Allocate(workersCount);
            return this;
        }

        public void Run(WorkItem labor)
        {
            Workator.Run(labor);
        }

        public void Reset(int workersCount = 1)
        {
            Workator.Reset(workersCount);
        }
    }
}
