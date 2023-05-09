namespace System.Instant.Mathset
{
    using System;
    using System.Reflection.Emit;

    [Serializable]
    public class PowerOperation : BinaryFormula
    {
        public PowerOperation(Formula e1, Formula e2) : base(e1, e2) { }

        public override MathsetSize Size
        {
            get { return expr1.Size; }
        }

        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            if (cc.IsFirstPass())
            {
                expr1.Compile(g, cc);
                expr2.Compile(g, cc);
                if (!(expr2.Size == MathsetSize.Scalar))
                    throw new SizeMismatchException(
                        "Pow Operator requires a scalar second operand"
                    );
                return;
            }
            expr1.Compile(g, cc);
            expr2.Compile(g, cc);
            g.EmitCall(OpCodes.Call, typeof(Math).GetMethod("Pow"), null);
        }
    }
}
