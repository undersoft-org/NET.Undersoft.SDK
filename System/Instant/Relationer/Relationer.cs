namespace System.Instant.Relationing
{
    using Linq;
    using Series;
    using Uniques;

    public interface IRelationer
    {
        Relations SourceRelations { get; }

        Relations TargetRelations { get; }

        void Clear();

        Relation GetSource(IFigure target, string SourceName);

        IDeck<Relation> GetSources(IFigures target, string SourceName);

        Relation GetTarget(IFigure source, string TargetName);

        IDeck<Relation> GetTargets(IFigures source, string TargetName);
    }

    [Serializable]
    public class Relationer
    {
        private static Catalog<Relation> map = new Catalog<Relation>(true, PRIMES_ARRAY.Get(9));
        private Relations sourceRelations;
        private Relations targetRelations;

        public Relationer()
        {
            sourceRelations = new Relations();
            targetRelations = new Relations();
        }

        public static IDeck<Relation> Map
        {
            get => map;
        }

        public Relation Relation { get; set; }

        public Relations SourceRelations
        {
            get => sourceRelations;
        }

        public Relations TargetRelations
        {
            get => targetRelations;
        }

        public void Clear()
        {
            Map.Flush();
        }

        public Relation GetSource(ISleeve figure, string SourceName)
        {
            return map[SourceKey(figure, SourceName)];
        }

        public Relation GetSourceRelation(string SourceName)
        {
            return SourceRelations[SourceName + "_" + Relation.Name];
        }

        public RelationMember GetSourceMember(string SourceName)
        {
            Relation link = GetSourceRelation(SourceName);
            if (link != null)
                return link.Source;
            return null;
        }

        public IDeck<Relation> GetSources(Relations figures, string SourceName)
        {
            var sourceMember = GetSourceMember(SourceName);
            return new Album<Relation>(
                figures.Select(f => map[sourceMember.RelationKey(f.ToSleeve())]),
                255
            );
        }

        public Relation GetTarget(ISleeve figure, string TargetName)
        {
            return map[TargetKey(figure, TargetName)];
        }

        public Relation GetTargetRelation(string TargetName)
        {
            return TargetRelations[Relation.Name + "_&_" + TargetName];
        }

        public RelationMember GetTargetMember(string TargetName)
        {
            Relation link = GetTargetRelation(TargetName);
            if (link != null)
                return link.Target;
            return null;
        }

        public IDeck<Relation> GetTargets(IFigures figures, string TargetName)
        {
            var targetMember = GetTargetMember(TargetName);
            return new Album<Relation>(
                figures.Select(f => map[targetMember.RelationKey(f.ToSleeve())]).ToArray(),
                255
            );
        }

        public ulong SourceKey(ISleeve figure, string SourceName)
        {
            return GetSourceMember(SourceName).RelationKey(figure);
        }

        public ulong TargetKey(ISleeve figure, string TargetName)
        {
            return GetTargetMember(TargetName).RelationKey(figure);
        }
    }

    public static class RelationerExtension
    {
        public static Relation GetSourceRelation(this Sleeve figures, string SourceName)
        {
            return Relationer.Map[SourceName + "_" + figures.Name];
        }

        public static Relation GetTargetRelation(this Sleeve figures, string TargetName)
        {
            return Relationer.Map[figures.Name + "_" + TargetName];
        }
    }
}
