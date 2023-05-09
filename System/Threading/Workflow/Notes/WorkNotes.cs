namespace System.Threading.Workflow
{
    using System.Linq;
    using System.Series;

    public enum EvokerType
    {
        Always,

        Single,

        Schedule,

        Nome
    }

    public class WorkNotes : Catalog<NoteBox>
    {
        private Case Case { get; set; }

        private void send(Note parameters)
        {
            if (parameters.RecipientName != null && parameters.SenderName != null)
            {
                if (ContainsKey(parameters.RecipientName))
                {
                    NoteBox iobox = Get(parameters.RecipientName);
                    if (iobox != null)
                        iobox.Notify(parameters);
                }
                else if (parameters.Recipient != null)
                {
                    WorkItem labor = parameters.Recipient;
                    NoteBox iobox = new NoteBox(labor.Worker.Name);
                    iobox.Work = labor;
                    iobox.Notify(parameters);
                    SetOutbox(iobox);
                }
                else if (Case != null)
                {
                    var labors = Case.AsValues()
                        .Where(m => m.ContainsKey(parameters.RecipientName))
                        .SelectMany(os => os.AsValues());

                    if (labors.Any())
                    {
                        WorkItem labor = labors.FirstOrDefault();
                        NoteBox iobox = new NoteBox(labor.Worker.Name);
                        iobox.Work = labor;
                        iobox.Notify(parameters);
                        SetOutbox(iobox);
                    }
                }
            }
        }

        public void Send(params Note[] parametersList)
        {
            foreach (Note parameters in parametersList)
            {
                send(parameters);
            }
        }

        public void SetOutbox(NoteBox value)
        {
            if (value != null)
            {
                if (value.Work != null)
                {
                    Put(value.RecipientName, value);
                }
                else
                {
                    var labors = Case.AsValues()
                        .Where(m => m.ContainsKey(value.RecipientName))
                        .SelectMany(os => os.AsValues());

                    if (labors.Any())
                    {
                        WorkItem labor = labors.First();
                        value.Work = labor;
                        Put(value.RecipientName, value);
                    }
                }
            }
        }

        public void CreateOutbox(string key, NoteBox noteBox)
        {
            if (noteBox != null)
            {
                if (noteBox.Work != null)
                {
                    WorkItem labor = noteBox.Work;
                    Put(noteBox.RecipientName, noteBox);
                }
                else
                {
                    var labors = Case.AsValues()
                        .Where(m => m.ContainsKey(key))
                        .SelectMany(os => os.AsValues());

                    if (labors.Any())
                    {
                        WorkItem labor = labors.FirstOrDefault();
                        noteBox.Work = labor;
                        Put(key, noteBox);
                    }
                }
            }
            else
            {
                var labors = Case.AsValues()
                    .Where(m => m.ContainsKey(key))
                    .SelectMany(os => os.AsValues());

                if (labors.Any())
                {
                    WorkItem labor = labors.FirstOrDefault();
                    NoteBox iobox = new NoteBox(labor.Worker.Name);
                    iobox.Work = labor;
                    Put(key, iobox);
                }
            }
        }
    }
}
