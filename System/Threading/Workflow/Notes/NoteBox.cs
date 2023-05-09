namespace System.Threading.Workflow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Series;

    public class NoteBox : Catalog<NoteTopic>
    {
        public NoteBox(string Recipient)
        {
            RecipientName = Recipient;
            Evokers = new NoteEvokers();
        }

        public NoteEvokers Evokers { get; set; }

        public WorkItem Work { get; set; }

        public string RecipientName { get; set; }

        public void Notify(params Note[] notes)
        {
            if (notes != null && notes.Any())
            {
                foreach (Note note in notes)
                {
                    NoteTopic queue = null;
                    if (note.SenderName != null)
                    {
                        if (!ContainsKey(note.SenderName))
                        {
                            queue = new NoteTopic(note.SenderName, this);
                            if (Add(note.SenderName, queue))
                            {
                                if (note.EvokerOut != null)
                                    Evokers.Add(note.EvokerOut);
                                queue.Notify(note);
                            }
                        }
                        else if (TryGet(note.SenderName, out queue))
                        {
                            if (notes != null && notes.Length > 0)
                            {
                                if (note.EvokerOut != null)
                                    Evokers.Add(note.EvokerOut);
                                queue.Notify(note);
                            }
                        }
                    }
                }
            }
        }

        public void Notify(Note note)
        {
            if (note.SenderName != null)
            {
                NoteTopic queue = null;
                if (!ContainsKey(note.SenderName))
                {
                    queue = new NoteTopic(note.SenderName, this);
                    if (Add(note.SenderName, queue))
                    {
                        if (note.EvokerOut != null)
                            Evokers.Add(note.EvokerOut);
                        queue.Notify(note);
                    }
                }
                else if (TryGet(note.SenderName, out queue))
                {
                    if (note.EvokerOut != null)
                        Evokers.Add(note.EvokerOut);
                    queue.Notify(note);
                }
            }
        }

        public void Notify(string key, params Note[] notes)
        {
            NoteTopic queue = null;
            if (!ContainsKey(key))
            {
                queue = new NoteTopic(key, this);
                if (Add(key, queue) && notes != null && notes.Length > 0)
                {
                    foreach (Note note in notes)
                    {
                        if (note.EvokerOut != null)
                            Evokers.Add(note.EvokerOut);
                        note.SenderName = key;
                        queue.Notify(note);
                    }
                }
            }
            else if (TryGet(key, out queue))
            {
                if (notes != null && notes.Length > 0)
                {
                    foreach (Note note in notes)
                    {
                        if (note.EvokerOut != null)
                            Evokers.Add(note.EvokerOut);
                        note.SenderName = key;
                        queue.Notify(note);
                    }
                }
            }
        }

        public void Notify(string key, Note value)
        {
            value.SenderName = key;
            NoteTopic queue = null;
            if (!ContainsKey(key))
            {
                queue = new NoteTopic(key, this);
                if (Add(key, queue))
                {
                    if (value.EvokerOut != null)
                        Evokers.Add(value.EvokerOut);
                    queue.Notify(value);
                }
            }
            else if (TryGet(key, out queue))
            {
                if (value.EvokerOut != null)
                    Evokers.Add(value.EvokerOut);
                queue.Notify(value);
            }
        }

        public void Notify(string key, object ioqueues)
        {
            NoteTopic queue = null;
            if (!ContainsKey(key))
            {
                queue = new NoteTopic(key, this);
                if (Add(key, queue) && ioqueues != null)
                {
                    queue.Notify(ioqueues);
                }
            }
            else if (TryGet(key, out queue))
            {
                if (ioqueues != null)
                {
                    queue.Notify(ioqueues);
                }
            }
        }

        public Note TakeNote(string key)
        {
            NoteTopic _ioqueue = null;
            if (TryGet(key, out _ioqueue))
                return _ioqueue.Dequeue();
            return null;
        }

        public IList<Note> TakeNotes(IList<string> keys)
        {
            return AsCards()
                .Where(q => keys.Contains(q.Value.SenderName))
                .Select(v => v.Value.Notes)
                .ToList();
        }

        public object[] GetParams(string key)
        {
            NoteTopic _ioqueue = null;
            Note temp = null;
            if (TryGet(key, out _ioqueue))
                if (_ioqueue.TryDequeue(out temp))
                    return temp.Parameters;
            return null;
        }

        public bool MeetsRequirements(IList<string> keys)
        {
            return this.AsCards()
                .Where(q => keys.Contains(q.Value.SenderName))
                .All(v => v.Value.Count > 0);
        }

        public void QualifyToEvoke()
        {
            List<NoteEvoker> toEvoke = new List<NoteEvoker>();
            foreach (NoteEvoker relay in Evokers.AsValues())
            {
                if (relay.RelatedWorkNames.All(r => ContainsKey(r)))
                    if (relay.RelatedWorkNames.All(r => this[r].AsValues().Any()))
                    {
                        toEvoke.Add(relay);
                    }
            }

            if (toEvoke.Any())
            {
                foreach (NoteEvoker evoke in toEvoke)
                {
                    if (MeetsRequirements(evoke.RelatedWorkNames))
                    {
                        IList<Note> notes = TakeNotes(evoke.RelatedWorkNames);

                        if (notes.All(a => a != null))
                        {
                            object[] parameters = new object[0];
                            object begin = Work.Worker.GetInput();
                            if (begin != null)
                                parameters = parameters.Concat((object[])begin).ToArray();
                            foreach (Note note in notes)
                            {
                                if (note.Parameters.GetType().IsArray)
                                    parameters = parameters
                                        .Concat(
                                            note.Parameters.SelectMany(a => (object[])a).ToArray()
                                        )
                                        .ToArray();
                                else
                                    parameters = parameters.Concat(note.Parameters).ToArray();
                            }

                            Work.Execute(parameters);
                        }
                    }
                }
            }
        }
    }
}
