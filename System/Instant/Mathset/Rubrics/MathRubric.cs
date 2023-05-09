namespace System.Instant.Mathset
{
    using System;
    using System.Uniques;

    public class MathRubric : IUnique
    {
        [NonSerialized]
        private CombinedFormula formula;

        [NonSerialized]
        private MathRubrics formulaRubrics;

        [NonSerialized]
        private Mathset formuler;

        [NonSerialized]
        private MathRubrics mathlineRubrics;

        [NonSerialized]
        private MemberRubric memberRubric;

        [NonSerialized]
        private CombinedMathset reckoner;

        [NonSerialized]
        private SubMathset subFormuler;

        public MathRubric(MathRubrics rubrics, MemberRubric rubric)
        {
            memberRubric = rubric;
            mathlineRubrics = rubrics;
            SerialCode = rubric.SerialCode;
        }

        public int ComputeOrdinal { get; set; }

        public IUnique Empty => Uscn.Empty;

        public CombinedMathset Evaluator
        {
            get { return reckoner; }
            set { reckoner = value; }
        }

        public int FigureFieldId
        {
            get => memberRubric.FieldId;
        }

        public Formula Formula
        {
            get { return (!ReferenceEquals(formula, null)) ? formula : null; }
            set
            {
                if (!ReferenceEquals(value, null))
                {
                    formula = value.Prepare(Formuler[this.memberRubric.RubricName]);
                }
            }
        }

        public MathRubric FormulaRubric
        {
            get { return this; }
        }

        public MathRubrics FormulaRubrics
        {
            get { return formulaRubrics; }
            set { formulaRubrics = value; }
        }

        public Mathset Formuler
        {
            get { return formuler; }
            set { formuler = value; }
        }

        public MathRubrics MathsetRubrics
        {
            get { return mathlineRubrics; }
            set { mathlineRubrics = value; }
        }

        public bool PartialMathset { get; set; }

        public Formula RightFormula { get; set; }

        public string RubricName
        {
            get => memberRubric.RubricName;
        }

        public Type RubricType
        {
            get => memberRubric.RubricType;
        }

        public Uscn SerialCode { get; set; }

        public SubMathset SubFormuler { get; set; }

        public ulong UniqueKey
        {
            get => SerialCode.UniqueKey;
            set => SerialCode.SetUniqueKey(value);
        }

        public ulong UniqueType
        {
            get => SerialCode.UniqueType;
            set => SerialCode.SetUniqueSeed(value);
        }

        public MathRubric AssignRubric(int ordinal)
        {
            if (FormulaRubrics == null)
                FormulaRubrics = new MathRubrics(mathlineRubrics);

            MathRubric erubric = null;
            MemberRubric rubric = MathsetRubrics.Rubrics[ordinal];
            if (rubric != null)
            {
                erubric = new MathRubric(MathsetRubrics, rubric);
                assignRubric(erubric);
            }
            return erubric;
        }

        public MathRubric AssignRubric(string name)
        {
            if (FormulaRubrics == null)
                FormulaRubrics = new MathRubrics(mathlineRubrics);

            MathRubric erubric = null;
            MemberRubric rubric = MathsetRubrics.Rubrics[name];
            if (rubric != null)
            {
                erubric = new MathRubric(MathsetRubrics, rubric);
                assignRubric(erubric);
            }
            return erubric;
        }

        public Mathset CloneMathset()
        {
            return formuler.Clone();
        }

        public CombinedMathset CombineEvaluator()
        {
            if (reckoner == null)
                reckoner = formula.CombineMathset(formula);

            return reckoner;
        }

        public int CompareTo(IUnique other)
        {
            return (int)(UniqueKey - other.UniqueKey);
        }

        public LeftFormula Compute()
        {
            if (reckoner != null)
            {
                Evaluator reckon = new Evaluator(reckoner.Compute);
                reckon();
            }
            return formula.lexpr;
        }

        public bool Equals(IUnique other)
        {
            return UniqueKey == other.UniqueKey;
        }

        public byte[] GetBytes()
        {
            return SerialCode.GetBytes();
        }

        public Mathset GetMathset()
        {
            if (!ReferenceEquals(Formuler, null))
                return Formuler;
            else
            {
                formulaRubrics = new MathRubrics(mathlineRubrics);
                return Formuler = new Mathset(this);
            }
        }

        public byte[] GetUniqueBytes()
        {
            return SerialCode.GetUniqueBytes();
        }

        public Mathset NewMathset()
        {
            return Formuler = new Mathset(this);
        }

        public MathRubric RemoveRubric(int ordinal)
        {
            MathRubric erubric = null;
            MemberRubric rubric = MathsetRubrics.Rubrics[ordinal];
            if (rubric != null)
            {
                erubric = MathsetRubrics[rubric];
                removeRubric(erubric);
            }
            return erubric;
        }

        public MathRubric RemoveRubric(string name)
        {
            MathRubric erubric = null;
            MemberRubric rubric = MathsetRubrics.Rubrics[name];
            if (rubric != null)
            {
                erubric = MathsetRubrics[rubric];
                removeRubric(erubric);
            }
            return erubric;
        }

        private MathRubric assignRubric(MathRubric erubric)
        {
            if (!FormulaRubrics.Contains(erubric))
            {
                if (!MathsetRubrics.MathsetRubrics.Contains(erubric))
                {
                    MathsetRubrics.MathsetRubrics.Add(erubric);
                }

                if (
                    erubric.FigureFieldId == FormulaRubric.FigureFieldId
                    && !MathsetRubrics.FormulaRubrics.Contains(erubric)
                )
                    MathsetRubrics.FormulaRubrics.Add(erubric);

                FormulaRubrics.Add(erubric);
            }
            return erubric;
        }

        private MathRubric removeRubric(MathRubric erubric)
        {
            if (!FormulaRubrics.Contains(erubric))
            {
                FormulaRubrics.Remove(erubric);

                if (!MathsetRubrics.MathsetRubrics.Contains(erubric))
                    MathsetRubrics.MathsetRubrics.Remove(erubric);

                if (
                    !ReferenceEquals(Formuler, null)
                    && !MathsetRubrics.FormulaRubrics.Contains(erubric)
                )
                {
                    MathsetRubrics.Rubrics.Remove(erubric);
                    Formuler = null;
                    Formula = null;
                }
            }
            return erubric;
        }
    }
}
