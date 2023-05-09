namespace System.Threading.Workflow
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Series;

    public class NoteEvokers : Catalog<NoteEvoker>
    {
        public bool Contains(IEnumerable<WorkItem> objectives)
        {
            return this.AsValues()
                .Any(t => t.RelatedWorks.Any(ro => objectives.All(o => ReferenceEquals(ro, o))));
        }

        public bool Contains(IEnumerable<string> relayNames)
        {
            return this.AsValues().Any(t => t.RelatedWorkNames.SequenceEqual(relayNames));
        }

        public NoteEvoker this[string relatedWorkName]
        {
            get
            {
                return this.AsValues()
                    .FirstOrDefault(c => c.RelatedWorkNames.Contains(relatedWorkName));
            }
        }
        public NoteEvoker this[WorkItem relatedWork]
        {
            get
            {
                return this.AsValues().FirstOrDefault(c => c.RelatedWorks.Contains(relatedWork));
            }
        }
    }
}
