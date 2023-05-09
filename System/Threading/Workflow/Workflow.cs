namespace System.Threading.Workflow
{
    public class Workflow<TCase> : Case where TCase : class
    {
        public Workflow() : base(new Aspects(typeof(TCase).FullName)) { }

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
}
