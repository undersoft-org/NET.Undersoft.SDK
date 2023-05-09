namespace System.Instant.Mathset
{
    using System;
    using System.Reflection.Emit;

    [Serializable]
    public abstract class LeftFormula : Formula
    {
        public abstract void CompileAssign(
            ILGenerator g,
            CompilerContext cc,
            bool post,
            bool partial
        );
    }
}
