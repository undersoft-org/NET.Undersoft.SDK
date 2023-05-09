namespace System.Instant.Relationing
{
    using System.Uniques;

    [Serializable]
    public class Relation : UniqueObject
    {
        private Uscn uniquecode;

        public Relation() { }

        public Relation(ISleeve source, ISleeve target)
        {
            RelationPair(source, target);
        }

        public Relation(ISleeve source, ISleeve target, IRubrics parentKeys, IRubrics childKeys)
            : this(source, target)
        {
            RelationParentKeys(parentKeys);
            RelationChildKeys(childKeys);
        }

        public Relation(ISleeve source, ISleeve target, string[] parentKeys, string[] childKeys)
            : this(source, target)
        {
            RelationParentKeys(parentKeys);
            RelationChildKeys(childKeys);
        }

        public Relation(ISleeve source, ISleeve node, ISleeve target)
        {
            RelationTrio(source, node, target);
        }

        public Relation(
            ISleeve source,
            ISleeve node,
            ISleeve target,
            IRubrics parentKeys,
            IRubrics nodeParentKeys,
            IRubrics nodeChildKeys,
            IRubrics childKeys
        ) : this(source, node, target)
        {
            RelationParentKeys(parentKeys);
            RelationNodeKeys(nodeParentKeys, nodeChildKeys);
            RelationChildKeys(childKeys);
        }

        public Relation(
            ISleeve source,
            ISleeve node,
            ISleeve target,
            string[] parentKeys,
            string[] nodeParentKeys,
            string[] nodeChildKeys,
            string[] childKeys
        ) : this(source, node, target)
        {
            RelationParentKeys(parentKeys);
            RelationNodeKeys(nodeParentKeys, nodeParentKeys);
            RelationChildKeys(childKeys);
        }

        public Relation SetRelation(ISleeve source, ISleeve target, IRubrics parentKeys, IRubrics childKeys)
        {
            RelationPair(source, target);
            RelationParentKeys(parentKeys);
            RelationChildKeys(childKeys);
            return this;
        }

        public Relation SetRelation(
            ISleeve source,
            ISleeve target,
            string[] parentKeynames,
            string[] childKeynames
        )
        {
            RelationPair(source, target);
            RelationParentKeys(parentKeynames);
            RelationChildKeys(childKeynames);
            return this;
        }

        public Relation RelationPair(ISleeve source, ISleeve target)
        {
            Name = source.GetType().Name + "To" + target.GetType().Name;

            UniqueKey = Name.UniqueKey64();
            UniqueType = Name.UniqueKey32();

            Source = new RelationMember(source, this, RelationSite.Source);
            Target = new RelationMember(target, this, RelationSite.Target);

            return Relationer.Map.Put(this).Value;
        }

        public Relation RelationTrio(ISleeve source, ISleeve node, ISleeve target)
        {
            Name = source.GetType().Name + "To" + target.GetType().Name;

            UniqueKey = Name.UniqueKey64();
            UniqueType = Name.UniqueKey32();

            Source = new RelationMember(source, this, RelationSite.Source);
            Node = new RelationNode(node, this);
            Target = new RelationMember(target, this, RelationSite.Target);

            return Relationer.Map.Put(this).Value;
        }

        public IUnique Empty => Uscn.Empty;

        public string Name { get; set; }

        public RelationMember Source { get; set; }

        public IRubrics SourceKeys
        {
            get { return Source.KeyRubrics; }
            set
            {
                Source.KeyRubrics.Renew(value);
                Source.KeyRubrics.Update();
            }
        }

        public string SourceName
        {
            get { return Source.Name; }
            set { Source.Name = value; }
        }

        public IRubrics SourceRubrics
        {
            get { return Source.Rubrics; }
            set { Source.Rubrics = value; }
        }

        public RelationNode Node { get; set; }

        public IRubrics NodeSourceKeys
        {
            get { return Node.SourceKeyRubrics; }
            set
            {
                Node.SourceKeyRubrics.Renew(value);
                Node.SourceKeyRubrics.Update();
            }
        }

        public IRubrics NodeTargetKeys
        {
            get { return Node.TargetKeyRubrics; }
            set
            {
                Node.TargetKeyRubrics.Renew(value);
                Node.TargetKeyRubrics.Update();
            }
        }

        public string NodeName
        {
            get { return Node.Name; }
            set { Node.Name = value; }
        }

        public IRubrics NodeRubrics
        {
            get { return Node.Rubrics; }
            set { Node.Rubrics = value; }
        }

        public RelationMember Target { get; set; }

        public IRubrics TargetKeys
        {
            get { return Target.KeyRubrics; }
            set
            {
                Target.KeyRubrics.Renew(value);
                Target.KeyRubrics.Update();
            }
        }

        public string TargetName
        {
            get { return Target.Name; }
            set { Target.Name = value; }
        }

        public IRubrics TargetRubrics
        {
            get { return Target.KeyRubrics; }
            set { Target.KeyRubrics = value; }
        }

        public void RelationParentKeys(IRubrics keyRubrics)
        {
            foreach (IUnique rubric in keyRubrics)
            {
                var sourceRubric = Source.Rubrics[rubric];
                if (sourceRubric != null)
                {
                    SourceKeys.Add(sourceRubric);
                }
                else
                    throw new IndexOutOfRangeException("Rubric not found");
                SourceKeys.Update();
            }
        }

        public void RelationNodeKeys(IRubrics sourceKeyRubric, IRubrics targetKeyRubric)
        {
            foreach (var rubric in sourceKeyRubric)
            {
                var nodeRubric = Node.Rubrics[rubric];
                if (nodeRubric != null)
                {
                    NodeSourceKeys.Add(nodeRubric);
                }
                else
                    throw new IndexOutOfRangeException("Rubric not found");
            }
            foreach (var rubric in targetKeyRubric)
            {
                var nodeRubric = Node.Rubrics[rubric];
                if (nodeRubric != null)
                {
                    NodeTargetKeys.Add(nodeRubric);
                }
                else
                    throw new IndexOutOfRangeException("Rubric not found");
            }

            SourceKeys.Update();
            NodeSourceKeys.Update();
            NodeTargetKeys.Update();
            TargetKeys.Update();
        }

        public void RelationChildKeys(IRubrics keyRubrics)
        {
            foreach (IUnique rubric in keyRubrics)
            {
                var targetRubric = Target.Rubrics[rubric];
                if (targetRubric != null)
                {
                    TargetKeys.Add(targetRubric);
                }
                else
                    throw new IndexOutOfRangeException("Rubric not found");
                TargetKeys.Update();
            }
        }

        public void RelationChildKeys(string[] keyRubricNames)
        {
            foreach (var name in keyRubricNames)
            {
                var targetRubric = Target.Rubrics[name];
                if (targetRubric != null)
                {
                    TargetKeys.Add(targetRubric);
                }
                else
                    throw new IndexOutOfRangeException("Rubric not found");
            }
            SourceKeys.Update();
            TargetKeys.Update();
        }

        public void RelationNodeKeys(string[] sourceKeyRubricNames, string[] targetKeyRubricNames)
        {
            foreach (var name in sourceKeyRubricNames)
            {
                var nodeRubric = Node.Rubrics[name];
                if (nodeRubric != null)
                {
                    NodeSourceKeys.Add(nodeRubric);
                }
                else
                    throw new IndexOutOfRangeException("Rubric not found");
            }
            foreach (var name in targetKeyRubricNames)
            {
                var nodeRubric = Node.Rubrics[name];
                if (nodeRubric != null)
                {
                    NodeTargetKeys.Add(nodeRubric);
                }
                else
                    throw new IndexOutOfRangeException("Rubric not found");
            }

            SourceKeys.Update();
            NodeSourceKeys.Update();
            NodeTargetKeys.Update();
            TargetKeys.Update();
        }

        public void RelationParentKeys(string[] keyRubricNames)
        {
            foreach (var name in keyRubricNames)
            {
                var sourceRubric = Source.Rubrics[name];
                if (sourceRubric != null)
                {
                    SourceKeys.Add(sourceRubric);
                }
                else
                    throw new IndexOutOfRangeException("Rubric not found");
            }
            SourceKeys.Update();
            TargetKeys.Update();
        }
    }
}
