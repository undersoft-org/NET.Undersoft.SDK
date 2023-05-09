namespace System.Instant.Mathset
{
    using System;
    using System.Reflection.Emit;

    [Serializable]
    public class Equal : BinaryOperator
    {
        public override double Apply(double a, double b)
        {
            return Convert.ToDouble(a == b);
        }

        public override void Compile(ILGenerator g)
        {
            g.Emit(OpCodes.Ceq);
        }
    }
}
