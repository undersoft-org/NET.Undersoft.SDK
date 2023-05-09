namespace System.Instant
{
    using System.Collections.Generic;
    using System.Extract;
    using System.Linq;
    using System.Series;
    using System.Uniques;

    public partial class MemberRubrics : AlbumBase<MemberRubric>, IRubrics
    {
        private int binarySize;
        private int[] binarySizes;
        private int[] ordinals;

        public MemberRubrics() : base() { }

        public MemberRubrics(IEnumerable<MemberRubric> collection) : base(collection) { }

        public MemberRubrics(IList<MemberRubric> collection) : base(collection) { }

        public int BinarySize
        {
            get => binarySize;
        }

        public int[] BinarySizes
        {
            get => binarySizes;
        }

        public IFigures Figures { get; set; }

        public IRubrics KeyRubrics { get; set; }

        public FieldMappings Mappings { get; set; }

        public int[] Ordinals
        {
            get => ordinals;
        }

        public Uscn SerialCode
        {
            get => Figures.UniqueCode;
            set => Figures.UniqueCode = value;
        }

        public override ulong UniqueKey
        {
            get => Figures.UniqueKey;
            set => Figures.UniqueKey = value;
        }

        public override ulong UniqueType
        {
            get => Figures.UniqueType;
            set => Figures.UniqueType = value;
        }

        public object[] ValueArray
        {
            get => Figures.ValueArray;
            set => Figures.ValueArray = value;
        }

        public new int CompareTo(IUnique other)
        {
            return Figures.CompareTo(other);
        }

        public override ICard<MemberRubric> EmptyCard()
        {
            return new RubricCard();
        }

        public override ICard<MemberRubric>[] EmptyCardTable(int size)
        {
            return new RubricCard[size];
        }

        public override ICard<MemberRubric>[] EmptyDeck(int size)
        {
            return new RubricCard[size];
        }

        public override bool Equals(IUnique other)
        {
            return Figures.Equals(other);
        }

        public override byte[] GetBytes()
        {
            return Figures.GetBytes();
        }

        public unsafe byte[] GetBytes(IFigure figure)
        {
            int size = Figures.FigureSize;
            byte* figurePtr = stackalloc byte[size * 2];
            byte* bufferPtr = figurePtr + size;
            figure.StructureTo(figurePtr);
            int destOffset = 0;
            foreach (var rubric in AsValues())
            {
                int l = rubric.RubricSize;
                Extractor.CopyBlock(bufferPtr, destOffset, figurePtr, rubric.RubricOffset, l);
                destOffset += l;
            }
            byte[] b = new byte[destOffset];
            fixed (byte* bp = b)
                Extractor.CopyBlock(bp, bufferPtr, destOffset);
            return b;
        }

        public override byte[] GetUniqueBytes()
        {
            return Figures.GetUniqueBytes();
        }

        public unsafe byte[] GetUniqueBytes(IFigure figure, uint seed = 0)
        {
            int size = Figures.FigureSize;
            byte* figurePtr = stackalloc byte[size * 2];
            byte* bufferPtr = figurePtr + size;
            figure.StructureTo(figurePtr);
            int destOffset = 0;
            foreach (var rubric in AsValues())
            {
                int l = rubric.RubricSize;
                Extractor.CopyBlock(bufferPtr, destOffset, figurePtr, rubric.RubricOffset, l);
                destOffset += l;
            }
            ulong hash = Hasher64.ComputeKey(bufferPtr, destOffset, seed);
            byte[] b = new byte[8];
            fixed (byte* bp = b)
                *((ulong*)bp) = hash;
            return b;
        }

        public unsafe ulong GetUniqueKey(IFigure figure, uint seed = 0)
        {
            int size = Figures.FigureSize;
            byte* figurePtr = stackalloc byte[size * 2];
            byte* bufferPtr = figurePtr + size;
            figure.StructureTo(figurePtr);
            int destOffset = 0;
            foreach (var rubric in AsValues())
            {
                int l = rubric.RubricSize;
                Extractor.CopyBlock(bufferPtr, destOffset, figurePtr, rubric.RubricOffset, l);
                destOffset += l;
            }
            return Hasher64.ComputeKey(bufferPtr, destOffset, seed);
        }

        public override ICard<MemberRubric> NewCard(ICard<MemberRubric> value)
        {
            return new RubricCard(value);
        }

        public override ICard<MemberRubric> NewCard(MemberRubric value)
        {
            return new RubricCard(value);
        }

        public override ICard<MemberRubric> NewCard(object key, MemberRubric value)
        {
            return new RubricCard(key, value);
        }

        public override ICard<MemberRubric> NewCard(ulong key, MemberRubric value)
        {
            return new RubricCard(key, value);
        }

        public void SetUniqueKey(IFigure figure, uint seed = 0)
        {
            figure.UniqueKey = GetUniqueKey(figure, seed);
        }

        public void Update()
        {
            ordinals = AsValues().Select(o => o.RubricId).ToArray();

            binarySizes = AsValues().Select(o => o.RubricSize).ToArray();

            binarySize = AsValues().Sum(b => b.RubricSize);

            if (KeyRubrics != null)
            {
                KeyRubrics.Update();
                KeyRubrics.ForEach(
                    (k) =>
                    {
                        var r = this[k];
                        r.IsKey = true;
                        r.IsIdentity = true;
                    }
                );
            }

            AsValues()
                .Where(r => r.IsKey || r.RubricType is IUnique)
                .ForEach(r => r.IsUnique = true)
                .ToArray();
        }
    }
}
