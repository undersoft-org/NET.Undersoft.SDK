namespace System.Threading.Workflow
{
    public interface IWorker
    {
        object GetInput();
        void SetInput(object value);

        object GetOutput();
        void SetOutput(object value);

        NoteEvokers Evokers { get; set; }

        string Name { get; set; }

        IDeputy Process { get; set; }

        Aspect FlowTo(WorkItem Recipient);

        Aspect FlowTo(WorkItem Recipient, params WorkItem[] RelationWorks);

        Aspect FlowTo(string RecipientName);

        Aspect FlowTo(string RecipientName, params string[] RelationNames);

        Aspect FlowFrom(WorkItem Recipient);

        Aspect FlowFrom(WorkItem Recipient, params WorkItem[] RelationWorks);

        Aspect FlowFrom(string RecipientName);

        Aspect FlowFrom(string RecipientName, params string[] RelationNames);
    }
}
