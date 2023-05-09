namespace System.Instant.Mathset
{
    using System;
    using System.Reflection.Emit;

    [Serializable]
    public class UnsignedFormula : Formula
    {
        internal double thevalue;

        public UnsignedFormula(double vv)
        {
            thevalue = vv;
        }

        public override MathsetSize Size
        {
            get { return MathsetSize.Scalar; }
        }

        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            if (cc.IsFirstPass())
                return;
            g.Emit(OpCodes.Ldc_R8, thevalue);
        }
    }
}
