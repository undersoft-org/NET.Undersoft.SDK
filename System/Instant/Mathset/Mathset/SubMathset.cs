﻿namespace System.Instant.Mathset
{
    using System.Instant;
    using System.Reflection.Emit;

    [Serializable]
    public class SubMathset : LeftFormula
    {
        public int startId = 0;

        public SubMathset(MathRubric evalRubric, Mathset formuler)
        {
            if (evalRubric != null)
                Rubric = evalRubric;

            SetDimensions(formuler);
        }

        public int colCount
        {
            get { return Formuler.Rubrics.Count; }
        }

        public IFigures Data
        {
            get { return Formuler.Data; }
        }

        public int FieldId
        {
            get => Rubric.FigureFieldId;
        }

        public Mathset Formuler { get; set; }

        public int rowCount
        {
            get { return Data.Count; }
        }

        public MathRubric Rubric { get; set; }

        public string RubricName
        {
            get => Rubric.RubricName;
        }

        public Type RubricType
        {
            get => Rubric.RubricType;
        }

        public override MathsetSize Size
        {
            get { return new MathsetSize(rowCount, colCount); }
        }

        public SubMathset SubFormuler { get; set; }

        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            if (cc.IsFirstPass())
            {
                cc.Add(Data);
            }
            else
            {
                CompilerContext.GenLocalLoad(g, cc.GetSubIndexOf(Data));

                g.Emit(OpCodes.Ldc_I4, FieldId);
                g.EmitCall(
                    OpCodes.Callvirt,
                    typeof(IFigure).GetMethod("get_Item", new Type[] { typeof(int) }),
                    null
                );
                g.Emit(OpCodes.Unbox_Any, RubricType);
                g.Emit(OpCodes.Conv_R8);
            }
        }

        public override void CompileAssign(
            ILGenerator g,
            CompilerContext cc,
            bool post,
            bool partial
        )
        {
            if (cc.IsFirstPass())
            {
                cc.Add(Data);
                return;
            }

            int i1 = cc.GetIndexVariable(0);

            if (!post)
            {
                if (!partial)
                {
                    CompilerContext.GenLocalLoad(g, cc.GetIndexOf(Data));

                    if (startId != 0)
                        g.Emit(OpCodes.Ldc_I4, startId);

                    g.Emit(OpCodes.Ldloc, i1);

                    if (startId != 0)
                        g.Emit(OpCodes.Add);

                    g.EmitCall(
                        OpCodes.Callvirt,
                        typeof(IFigures).GetMethod("get_Item", new Type[] { typeof(int) }),
                        null
                    );
                    CompilerContext.GenLocalStore(g, cc.GetSubIndexOf(Data));
                    CompilerContext.GenLocalLoad(g, cc.GetSubIndexOf(Data));
                }
                else
                {
                    CompilerContext.GenLocalLoad(g, cc.GetSubIndexOf(Data));
                }
                g.Emit(OpCodes.Ldc_I4, FieldId);
            }
            else
            {
                if (partial)
                {
                    g.Emit(OpCodes.Dup);
                    CompilerContext.GenLocalStore(g, cc.GetBufforIndexOf(Data));
                }

                g.Emit(OpCodes.Box, typeof(double));
                g.EmitCall(
                    OpCodes.Callvirt,
                    typeof(IFigure).GetMethod(
                        "set_Item",
                        new Type[] { typeof(int), typeof(object) }
                    ),
                    null
                );

                if (partial)
                    CompilerContext.GenLocalLoad(g, cc.GetBufforIndexOf(Data));
            }
        }

        public void SetDimensions(Mathset formuler = null)
        {
            if (!ReferenceEquals(formuler, null))
                Formuler = formuler;
            Rubric.SubFormuler = this;
        }
    }
}
