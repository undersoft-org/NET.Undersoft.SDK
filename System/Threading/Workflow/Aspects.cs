namespace System.Threading.Workflow
{
    using System.Collections.Generic;
    using System.Series;

    public class Aspects : Catalog<Aspect>
    {
        public Aspects(string name = null, WorkNotes notes = null)
        {
            Name = (name != null) ? name : "ThreadGraph";
            Notes = (Notes != null) ? notes : new WorkNotes();
            Methods = new WorkMethods();
        }

        public string Name { get; set; }

        public WorkMethods Methods { get; set; }

        public WorkNotes Notes { get; set; }

        public Aspect Get(string key)
        {
            Aspect result = null;
            TryGet(key, out result);
            return result;
        }

        public override void Add(Aspect aspect)
        {
            aspect.Case = this;
            aspect.Workator = new Workspace(aspect);
            Put(aspect.Name, aspect);
        }

        public override void Add(IEnumerable<Aspect> aspects)
        {
            foreach (var aspect in aspects)
            {
                aspect.Case = this;
                aspect.Workator = new Workspace(aspect);
                Put(aspect.Name, aspect);
            }
        }

        public override bool Add(object key, Aspect value)
        {
            value.Case = this;
            value.Workator = new Workspace(value);
            Put(key, value);
            return true;
        }

        public void Add(object key, IEnumerable<WorkItem> value)
        {
            Aspect msn = new Aspect(key.ToString(), value);
            msn.Case = this;
            msn.Workator = new Workspace(msn);
            Put(key, msn);
        }

        public Aspect Add(object key, IEnumerable<IDeputy> value)
        {
            Aspect msn = new Aspect(key.ToString(), value);
            msn.Case = this;
            msn.Workator = new Workspace(msn);
            Put(key, msn);
            return msn;
        }

        public void Add(object key, IDeputy value)
        {
            List<IDeputy> cml = new List<IDeputy>() { value };
            Aspect msn = new Aspect(key.ToString(), cml);
            msn.Case = this;
            msn.Workator = new Workspace(msn);
            Put(key, msn);
        }

        public override Aspect this[object key]
        {
            get
            {
                TryGet(key, out Aspect result);
                return result;
            }
            set
            {
                value.Case = this;
                value.Workator = new Workspace(value);
                Put(key, value);
            }
        }
    }
}
