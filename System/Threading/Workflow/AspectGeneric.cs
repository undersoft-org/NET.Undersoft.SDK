namespace System.Threading.Workflow
{
    public class Aspect<TAspect> : Aspect where TAspect : class
    {
        public Aspect() : base(typeof(TAspect).FullName) { }

        public override WorkItem Work<TEvent>() where TEvent : class
        {
            return base.Work<TEvent>();
        }

        public override WorkItem Work<TEvent>(Type[] arguments) where TEvent : class
        {
            return base.Work<TEvent>(arguments);
        }

        public override WorkItem Work<TEvent>(params object[] consrtuctorParams)
            where TEvent : class
        {
            return base.Work<TEvent>(consrtuctorParams);
        }

        public override WorkItem Work<TEvent>(Type[] arguments, params object[] consrtuctorParams)
            where TEvent : class
        {
            return base.Work<TEvent>(arguments, consrtuctorParams);
        }
    }
}
