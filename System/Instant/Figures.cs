namespace System.Instant
{
    using System.Linq;
    using System.Uniques;

    public enum FiguresMode
    {
        Sleeve,
        Figure
    }

    public class Sleeves<T> : Figures
    {
        public Sleeves() : base(typeof(T)) { }

        public Sleeves(string figuresName) : base(typeof(T), figuresName) { }
    }

    public class Figures<T> : Figures
    {
        public Figures(FigureMode mode = FigureMode.Reference) : base(typeof(T), mode) { }

        public Figures(string figuresName, FigureMode mode = FigureMode.Reference)
            : base(typeof(T), figuresName, mode) { }
    }

    public class Figures : IInstant
    {
        private Type compiledType;
        private Figure figure;
        private Sleeve sleeve;
        private ulong key;
        private bool safeThread;
        private FiguresMode mode;

        public Figures(
            Sleeve sleeveGenerator,
            string figuresTypeName = null,
            bool safeThread = true
        )
        {
            mode = FiguresMode.Sleeve;
            if (sleeveGenerator.Type == null)
                sleeveGenerator.Combine();
            this.safeThread = safeThread;
            this.sleeve = sleeveGenerator;
            Name =
                (figuresTypeName != null && figuresTypeName != "")
                    ? figuresTypeName
                    : sleeve.Name + "_F";
        }

        public Figures(ISleeve sleeveObject, bool safeThread = true)
            : this(
                new Sleeve(sleeveObject.GetType(), sleeveObject.GetType().Name),
                null,
                safeThread
            )
        { }

        public Figures(Type sleeveModelType, bool safeThread = true)
            : this(new Sleeve(sleeveModelType), null, safeThread) { }

        public Figures(Type sleeveModelType, string figuresName, bool safeThread = true)
            : this(new Sleeve(sleeveModelType), figuresName, safeThread) { }

        public Figures(
            Figure figureGenerator,
            string figuresTypeName = null,
            bool safeThread = true
        )
        {
            mode = FiguresMode.Figure;
            if (figureGenerator.Type == null)
                figureGenerator.Combine();
            this.safeThread = safeThread;
            this.figure = figureGenerator;
            Name =
                (figuresTypeName != null && figuresTypeName != "")
                    ? figuresTypeName
                    : figure.Name + "s";
        }

        public Figures(IFigure figureObject, bool safeThread = true)
            : this(
                new Figure(
                    figureObject.GetType(),
                    figureObject.GetType().Name,
                    FigureMode.Reference
                ),
                null,
                safeThread
            )
        { }

        public Figures(
            IFigure figureObject,
            string figuresTypeName,
            FigureMode modeType = FigureMode.Reference,
            bool safeThread = true
        )
            : this(
                new Figure(figureObject.GetType(), figureObject.GetType().Name, modeType),
                figuresTypeName,
                safeThread
            )
        { }

        public Figures(
            MemberRubrics figureRubrics,
            string figuresTypeName = null,
            string figureTypeName = null,
            FigureMode modeType = FigureMode.Reference,
            bool safeThread = true
        ) : this(new Figure(figureRubrics, figureTypeName, modeType), figuresTypeName, safeThread)
        { }

        public Figures(Type figureModelType, FigureMode modeType, bool safeThread = true)
            : this(new Figure(figureModelType, null, modeType), null, safeThread) { }

        public Figures(
            Type figureModelType,
            string figuresTypeName,
            FigureMode modeType,
            bool safeThread = true
        ) : this(new Figure(figureModelType, null, modeType), figuresTypeName, safeThread) { }

        public Figures(
            Type figureModelType,
            string figuresTypeName,
            string figureTypeName,
            FigureMode modeType = FigureMode.Reference,
            bool safeThread = true
        ) : this(new Figure(figureModelType, figureTypeName, modeType), figuresTypeName, safeThread)
        { }

        public Type BaseType { get; set; }

        public string Name { get; set; }

        public IRubrics Rubrics
        {
            get => (sleeve != null) ? sleeve.Rubrics : figure.Rubrics;
        }

        public int Size
        {
            get => (sleeve != null) ? sleeve.Size : figure.Size;
        }

        public Type Type { get; set; }

        public IFigures Combine()
        {
            if (this.Type == null)
            {
                var ifc = new FiguresCompiler(this, safeThread);
                compiledType = ifc.CompileFigureType(Name);
                this.Type = compiledType.New().GetType();
                key = Unique.New;
            }
            if (mode == FiguresMode.Figure)
                return newFigures();
            else
                return newSleeves();
        }

        public object New()
        {
            return (mode == FiguresMode.Sleeve) ? newSleeves() : newFigures();
        }

        private MemberRubrics CloneRubrics()
        {
            var rubrics = new MemberRubrics(Rubrics.Select(r => r.ShalowCopy(null)));
            rubrics.KeyRubrics = new MemberRubrics(
                Rubrics.KeyRubrics.Select(r => r.ShalowCopy(null))
            );
            rubrics.Update();
            return rubrics;
        }

        private IFigures newFigures()
        {
            IFigures newfigures = newFigures((IFigures)(Type.New()));
            newfigures.Rubrics = CloneRubrics();
            newfigures.KeyRubrics = newfigures.Rubrics.KeyRubrics;
            newfigures.View = newfigures.AsQueryable();
            return newfigures;
        }

        private IFigures newFigures(IFigures newfigures)
        {
            newfigures.FigureType = figure.Type;
            newfigures.FigureSize = figure.Size;
            newfigures.Type = this.Type;
            newfigures.Instant = this;
            newfigures.Prime = true;
            newfigures.UniqueKey = key;
            newfigures.UniqueType = Name.UniqueKey64();

            return newfigures;
        }

        private IFigures newSleeves()
        {
            IFigures newsleeves = newSleeves((IFigures)(this.Type.New()));
            newsleeves.Rubrics = CloneRubrics();
            newsleeves.KeyRubrics = newsleeves.Rubrics.KeyRubrics;
            newsleeves.View = newsleeves.AsQueryable();
            return newsleeves;
        }

        private IFigures newSleeves(IFigures newsleeves)
        {
            newsleeves.FigureType = sleeve.Type;
            newsleeves.FigureSize = sleeve.Size;
            newsleeves.Type = this.Type;
            newsleeves.Instant = this;
            newsleeves.Prime = true;
            newsleeves.UniqueKey = key;
            newsleeves.UniqueType = Name.UniqueKey64();

            return newsleeves;
        }

    }
}
