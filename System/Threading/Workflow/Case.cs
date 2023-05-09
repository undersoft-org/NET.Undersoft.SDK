using System.Uniques;

namespace System.Threading.Workflow
{
    using System.Collections.Generic;
    using System.Linq;

    public class Case<TRule> : Case where TRule : class
    {
        public Case() : base(new Aspects(typeof(TRule).FullName)) { }

        public Aspect<TAspect> Aspect<TAspect>() where TAspect : class
        {
            if (!TryGet(typeof(TAspect).FullName, out Aspect aspect))
            {
                aspect = new Aspect<TAspect>();
                Add(aspect);
            }
            return aspect as Aspect<TAspect>;
        }
    }

    public class Case : Aspects
    {
        public Case(IEnumerable<IDeputy> methods, Aspects @case = null)
            : base(
                (@case == null) ? $"Case_{Unique.New}" : @case.Name,
                (@case == null) ? new WorkNotes() : @case.Notes
            )
        {
            Add($"Aspect_{Unique.New}", methods);
            Open();
        }

        public Case(Aspects @case) : base(@case.Name, @case.Notes)
        {
            Add(@case.AsValues());
        }

        public Case() : base($"Case_{Unique.New}", new WorkNotes()) { }

        public Aspect Aspect(IDeputy method, Aspect aspect)
        {
            if (aspect != null)
            {
                if (!TryGet(aspect.Name, out Aspect _aspect))
                {
                    Add(_aspect);
                    _aspect.AddWork(method);
                }
                return aspect;
            }
            return null;
        }

        public Aspect Aspect(IDeputy method, string name)
        {
            if (!TryGet(name, out Aspect aspect))
            {
                aspect = new Aspect(name);
                Add(aspect);
                aspect.AddWork(method);
            }
            return aspect;
        }

        public Aspect Aspect(string name)
        {
            if (!TryGet(name, out Aspect aspect))
            {
                aspect = new Aspect(name);
                Add(aspect);
            }
            return aspect;
        }

        public void Open()
        {
            Setup();
        }

        public void Setup()
        {
            foreach (Aspect aspect in AsValues())
            {
                if (aspect.Workator == null)
                {
                    aspect.Workator = new Workspace(aspect);
                }
                if (!aspect.Workator.Ready)
                {
                    aspect.Allocate();
                }
            }
        }

        public void Run(string laborName, params object[] input)
        {
            WorkItem[] labors = AsValues()
                .Where(m => m.ContainsKey(laborName))
                .SelectMany(w => w.AsValues())
                .ToArray();

            foreach (WorkItem labor in labors)
                labor.Execute(input);
        }

        public void Run(IDictionary<string, object[]> laborsAndInputs)
        {
            foreach (KeyValuePair<string, object[]> worker in laborsAndInputs)
            {
                object input = worker.Value;
                string workerName = worker.Key;
                WorkItem[] workerWorks = AsValues()
                    .Where(m => m.ContainsKey(workerName))
                    .SelectMany(w => w.AsValues())
                    .ToArray();

                foreach (WorkItem objc in workerWorks)
                    objc.Execute(input);
            }
        }
    }

    public class InvalidWorkException : Exception
    {
        #region Constructors


        public InvalidWorkException(string message) : base(message) { }

        #endregion
    }
}
