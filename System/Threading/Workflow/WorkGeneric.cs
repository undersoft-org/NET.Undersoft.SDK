namespace System.Threading.Workflow
{
    public class Work<T> : WorkItem
    {
        public Work(Func<T, string> method) : base(new Deputy<T>(method)) { }
    }
}
