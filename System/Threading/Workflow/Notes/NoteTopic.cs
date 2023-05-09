namespace System.Threading.Workflow
{
    using System.Collections.Generic;
    using System.Series;

    public class NoteTopic : Catalog<Note>
    {
        public NoteBox RecipientBox;

        public NoteTopic(string senderName, IList<Note> notelist, NoteBox recipient = null)
        {
            if (recipient != null)
            {
                RecipientBox = recipient;
            }
            if (notelist != null && notelist.Count > 0)
            {
                foreach (Note evocation in notelist)
                {
                    evocation.SenderName = SenderName;
                    Notes = evocation;
                }
            }
        }

        public NoteTopic(string senderName, Note note, NoteBox recipient = null)
        {
            if (recipient != null)
            {
                RecipientBox = recipient;
            }
            SenderName = senderName;
            Notes = note;
        }

        public NoteTopic(string senderName, NoteBox recipient = null)
        {
            if (recipient != null)
                RecipientBox = recipient;
            SenderName = senderName;
        }

        public NoteTopic(string senderName, NoteBox recipient = null, params object[] parameters)
        {
            if (recipient != null)
                RecipientBox = recipient;
            SenderName = senderName;
            if (parameters != null)
            {
                if (parameters[0].GetType() == typeof(Dictionary<string, object>))
                {
                    Note result = new Note(senderName, parameters);
                    Notes = result;
                }
            }
        }

        public Note Notes
        {
            get
            {
                Note _result = null;
                TryDequeue(out _result);
                return _result;
            }
            set
            {
                value.SenderName = SenderName;
                Enqueue(DateTime.Now.ToBinary(), value);
                if (RecipientBox != null)
                    RecipientBox.QualifyToEvoke();
            }
        }

        public string SenderName { get; set; }

        public void Notify(IList<Note> noteList)
        {
            foreach (Note result in noteList)
                Notes = result;
        }

        public void Notify(Note note)
        {
            Notes = note;
        }

        public void Notify(params object[] parameters)
        {
            if (parameters != null)
            {
                Note result = new Note(SenderName);
                result.Parameters = parameters;
                Notes = result;
            }
        }

        public void Notify(string senderName, params object[] parameters)
        {
            SenderName = senderName;
            if (parameters != null)
            {
                Note result = new Note(senderName);
                result.Parameters = parameters;
                Notes = result;
            }
        }
    }
}
