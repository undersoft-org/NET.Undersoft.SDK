namespace System.Instant.Mathset
{
    using System;
    using System.Reflection.Emit;

    [Serializable]
    public class UnsignedOperator : Formula
    {
        protected Formula e;

        public UnsignedOperator(Formula ee)
        {
            e = ee;
        }

        public override MathsetSize Size
        {
            get { return e.Size; }
        }

        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            e.Compile(g, cc);
        }
    }
}
