namespace System.Threading.Workflow
{
    public interface IWorkspace
    {
        void Close(bool SafeClose);

        Aspect Allocate(int antcount = 1);

        void Run(WorkItem labor);

        void Reset(int antcount = 1);
    }
}
