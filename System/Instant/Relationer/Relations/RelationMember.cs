namespace System.Instant.Relationing
{
    using System.Extract;
    using System.Linq;
    using System.Uniques;

    [Serializable]
    public class RelationMember : UniqueObject
    {
        public RelationMember()
        {
            KeyRubrics = new MemberRubrics();
        }

        public RelationMember(ISleeve sleeve, Relation link, RelationSite site) : this()
        {
            string[] names = link.Name.Split("To");
            RelationMember member;
            Site = site;
            Relation = link;

            int siteId = 1;

            if (site == RelationSite.Source)
            {
                siteId = 0;
                member = Relation.Source;
            }
            else
                member = Relation.Target;

            Name = names[siteId];
            UniqueKey = names[siteId].UniqueKey64(link.UniqueKey);
            UniqueType = link.UniqueKey;
            Rubrics = sleeve.Rubrics;
            Sleeve = sleeve;
        }

        public RelationMember(Relation link, RelationSite site) : this()
        {
            string[] names = link.Name.Split("_&_");
            Site = site;
            Relation = link;
            RelationMember member;
            int siteId = 1;

            if (site == RelationSite.Source)
            {
                siteId = 0;
                member = Relation.Source;
            }
            else
                member = Relation.Target;

            Name = names[siteId];
            UniqueKey = names[siteId].UniqueKey64(link.UniqueKey);
            UniqueType = link.UniqueKey;
            Rubrics = member.Sleeve.Rubrics;
            Sleeve = member.Sleeve;
        }

        public IUnique Empty => Ussc.Empty;

        public ISleeve Sleeve { get; set; }

        public IRubrics KeyRubrics { get; set; }

        public Relation Relation { get; set; }

        public string Name { get; set; }

        public IRubrics Rubrics { get; set; }

        public RelationSite Site { get; set; }

        public unsafe ulong RelationKey(ISleeve figure)
        {
            byte[] b = KeyRubrics.Ordinals.SelectMany(x => figure[x].GetBytes()).ToArray();

            int l = b.Length;
            fixed (byte* pb = b)
            {
                return Hasher64.ComputeKey(pb, l);
            }
        }
    }

    [Serializable]
    public class RelationNode : UniqueObject
    {
        public RelationNode()
        {
            SourceKeyRubrics = new MemberRubrics();
            TargetKeyRubrics = new MemberRubrics();
        }

        public RelationNode(ISleeve sleeve, Relation link) : this()
        {
            Name = link.Name;
            RelationNode member;
            Site = RelationSite.Node;
            Relation = link;

            member = Relation.Node;

            UniqueKey = Name.UniqueKey64(link.UniqueKey);
            UniqueType = link.UniqueKey;
            Rubrics = sleeve.Rubrics;
            Sleeve = sleeve;
        }

        public IUnique Empty => Ussc.Empty;

        public ISleeve Sleeve { get; set; }

        public IRubrics SourceKeyRubrics { get; set; }

        public IRubrics TargetKeyRubrics { get; set; }

        public Relation Relation { get; set; }

        public string Name { get; set; }

        public IRubrics Rubrics { get; set; }

        public RelationSite Site { get; set; }

        public unsafe ulong RelationSourceKey(ISleeve figure)
        {
            byte[] b = SourceKeyRubrics.Ordinals.SelectMany(x => figure[x].GetBytes()).ToArray();

            int l = b.Length;
            fixed (byte* pb = b)
            {
                return Hasher64.ComputeKey(pb, l);
            }
        }

        public unsafe ulong RelationTargetKey(ISleeve figure)
        {
            byte[] b = SourceKeyRubrics.Ordinals.SelectMany(x => figure[x].GetBytes()).ToArray();

            int l = b.Length;
            fixed (byte* pb = b)
            {
                return Hasher64.ComputeKey(pb, l);
            }
        }
    }
}
