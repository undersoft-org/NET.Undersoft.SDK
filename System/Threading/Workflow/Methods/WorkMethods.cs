namespace System.Threading.Workflow
{
    using System.Series;

    public class WorkMethods : Catalog<IDeputy>
    {
        public override ICard<IDeputy>[] EmptyDeck(int size)
        {
            return new WorkMethod[size];
        }

        public override ICard<IDeputy> EmptyCard()
        {
            return new WorkMethod();
        }

        public override ICard<IDeputy>[] EmptyCardTable(int size)
        {
            return new WorkMethod[size];
        }

        public override ICard<IDeputy> NewCard(IDeputy value)
        {
            return new WorkMethod(value);
        }

        public override ICard<IDeputy> NewCard(object key, IDeputy value)
        {
            return new WorkMethod(key, value);
        }

        public override ICard<IDeputy> NewCard(ulong key, IDeputy value)
        {
            return new WorkMethod(key, value);
        }
    }
}
