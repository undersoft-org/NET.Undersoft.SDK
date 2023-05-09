namespace System.Instant
{
    using Linq;
    using Series;
    using System;

    public class Sleeve<T> : Sleeve
    {
        public Sleeve() : base(typeof(T)) { }

        public Sleeve(string sleeveName) : base(typeof(T), sleeveName) { }
    }

    public class Sleeve : IInstant
    {
        private IDeck<RubricBuilder> rubricBuilders;
        private InstantBuilder instantBuilder;
        private Type compiledType;

        public Sleeve(Type figureModelType) : this(figureModelType, null) { }

        public Sleeve(Type figureModelType, string figureTypeName)
        {
            Traceable = figureModelType.IsAssignableTo(typeof(ITraceable));

            BaseType = figureModelType;

            Name = figureTypeName == null
                ? figureModelType.Name
                : figureTypeName;

            instantBuilder = new InstantBuilder();
            rubricBuilders = instantBuilder.CreateBuilders(figureModelType);

            Rubrics = new MemberRubrics(rubricBuilders.Select(m => m.Member).ToArray());
            Rubrics.KeyRubrics = new MemberRubrics();
        }

        public Type BaseType { get; set; }

        public string Name { get; set; }

        public IRubrics Rubrics { get; set; }

        public int Size { get; set; }

        public Type Type { get; set; }

        public bool Traceable { get; set; }

        public object New()
        {
            return Combine();
        }

        public ISleeve Combine(object obj = null)
        {
            var s = combine();
            if (obj == null)
                obj = BaseType.New();
            s.Devisor = obj;
            return s;
        }

        public ISleeve combine()
        {
            if (Type == null)
            {
                try
                {
                    ISleeve s = compile(new SleeveCompiler(this, rubricBuilders));
                    Rubrics.Update();
                    s.Rubrics = Rubrics;
                    return s;
                }
                catch (Exception ex)
                {
                    throw new SleeveCompilerException("Sleeve compilation at runtime failed see inner exception", ex);
                }
            }

            return create();
        }

        private ISleeve compile(SleeveCompiler compiler)
        {
            var fcdt = compiler;

            compiledType = fcdt.CompileSleeveType(Name);

            Rubrics.KeyRubrics.Add(fcdt.Identities.Values.Select(v => Rubrics[v.Name]).ToArray());

            var obj = compiledType.New();

            Type = obj.GetType();

            Rubrics.Select(
                (f, y) => new object[]
                {
                    f.FieldId = y,
                    f.RubricId = y
                }).ToArray();

            return (ISleeve)obj;
        }

        private ISleeve create()
        {
            var s = (ISleeve)(Type.New());
            s.Rubrics = Rubrics;
            return s;
        }

    }

    public class SleeveCompilerException : Exception
    {
        public SleeveCompilerException(string message, Exception innerException) : base(message, innerException) { }
    }
}