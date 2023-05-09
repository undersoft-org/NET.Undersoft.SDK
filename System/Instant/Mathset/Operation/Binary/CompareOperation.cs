namespace System.Instant.Mathset
{
    using System;
    using System.Reflection.Emit;

    [Serializable]
    public class CompareOperation : BinaryFormula
    {
        internal BinaryOperator oper;

        public CompareOperation(Formula e1, Formula e2, BinaryOperator op) : base(e1, e2)
        {
            oper = op;
        }

        public override MathsetSize Size
        {
            get { return expr1.Size == MathsetSize.Scalar ? expr2.Size : expr1.Size; }
        }

        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            expr1.Compile(g, cc);
            expr2.Compile(g, cc);
            if (cc.IsFirstPass())
                return;
            oper.Compile(g);
        }
    }
}
