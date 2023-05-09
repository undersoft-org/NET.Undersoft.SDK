namespace System.Instant
{
    using System.Instant.Relationing;
    using System.Instant.Treatments;
    using System.IO;
    using System.Linq;
    using System.Series;
    using System.Uniques;

    public abstract class FigureCatalog : CatalogBase<IFigure>, IFigures
    {
        public IInstant Instant { get; set; }

        public abstract bool Prime { get; set; }

        public abstract object this[int index, string propertyName] { get; set; }

        public abstract object this[int index, int fieldId] { get; set; }

        public abstract IRubrics Rubrics { get; set; }

        public abstract IRubrics KeyRubrics { get; set; }

        public abstract IFigure NewFigure();

        public abstract ISleeve NewSleeve();

        public abstract Type FigureType { get; set; }

        public abstract int FigureSize { get; set; }

        public abstract Uscn UniqueCode { get; set; }

        public new abstract ulong UniqueKey { get; set; }

        public new abstract ulong UniqueType { get; set; }

        public override ICard<IFigure> EmptyCard()
        {
            return new FigureCard(this);
        }

        public override ICard<IFigure> NewCard(ulong key, IFigure value)
        {
            return new FigureCard(key, value, this);
        }

        public override ICard<IFigure> NewCard(object key, IFigure value)
        {
            return new FigureCard(key, value, this);
        }

        public override ICard<IFigure> NewCard(IFigure value)
        {
            return new FigureCard(value, this);
        }

        public override ICard<IFigure> NewCard(ICard<IFigure> value)
        {
            return new FigureCard(value, this);
        }

        public override ICard<IFigure>[] EmptyCardTable(int size)
        {
            return new FigureCard[size];
        }

        public override ICard<IFigure>[] EmptyDeck(int size)
        {
            return new FigureCard[size];
        }

        protected override bool InnerAdd(IFigure value)
        {
            return InnerAdd(NewCard(value));
        }

        protected override ICard<IFigure> InnerPut(IFigure value)
        {
            return InnerPut(NewCard(value));
        }

        public override ICard<IFigure> New()
        {
            ICard<IFigure> newCard = NewCard(Unique.New, NewFigure());
            if (InnerAdd(newCard))
                return newCard;
            return null;
        }

        public override ICard<IFigure> New(ulong key)
        {
            ICard<IFigure> newCard = NewCard(key, NewFigure());
            if (InnerAdd(newCard))
                return newCard;
            return null;
        }

        public override ICard<IFigure> New(object key)
        {
            ICard<IFigure> newCard = NewCard(unique.Key(key), NewFigure());
            if (InnerAdd(newCard))
                return newCard;
            return null;
        }

        public object[] ValueArray
        {
            get => ToObjectArray();
            set => Put(value);
        }

        public Type Type { get; set; }

        public IQueryable<IFigure> View { get; set; }

        public IFigure Summary { get; set; }

        public FigureFilter Filter { get; set; }

        public FigureSort Sort { get; set; }

        public Func<IFigure, bool> Predicate { get; set; }

        public Relationer Linker { get; set; } = new Relationer();

        private Treatment treatment;
        public Treatment Treatment
        {
            get => treatment == null ? treatment = new Treatment(this) : treatment;
            set => treatment = value;
        }

        public IDeck<IComputation> Computations { get; set; }

        object IFigure.this[int fieldId]
        {
            get => this[fieldId];
            set => this[fieldId] = (IFigure)value;
        }
        public object this[string propertyName]
        {
            get => this[propertyName];
            set => this[propertyName] = (IFigure)value;
        }

        public override byte[] GetBytes()
        {
            return UniqueCode.GetBytes();
        }

        public override byte[] GetUniqueBytes()
        {
            return UniqueCode.GetUniqueBytes();
        }

        public override bool Equals(IUnique other)
        {
            return UniqueCode.Equals(other);
        }

        public override int CompareTo(IUnique other)
        {
            return UniqueCode.CompareTo(other);
        }

        public int SerialCount { get; set; }
        public int DeserialCount { get; set; }
        public int ProgressCount { get; set; }

        public int Serialize(
            Stream stream,
            int offset,
            int batchSize,
            SerialFormat serialFormat = SerialFormat.Binary
        )
        {
            throw new NotImplementedException();
        }

        public int Serialize(
            ISerialBuffer buffor,
            int offset,
            int batchSize,
            SerialFormat serialFormat = SerialFormat.Binary
        )
        {
            throw new NotImplementedException();
        }

        public object Deserialize(Stream stream, SerialFormat serialFormat = SerialFormat.Binary)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(
            ISerialBuffer buffer,
            SerialFormat serialFormat = SerialFormat.Binary
        )
        {
            throw new NotImplementedException();
        }

        public object[] GetMessage()
        {
            return new[] { (IFigures)this };
        }

        public object GetHeader()
        {
            return this;
        }

        public int ItemsCount => Count;
    }
}
