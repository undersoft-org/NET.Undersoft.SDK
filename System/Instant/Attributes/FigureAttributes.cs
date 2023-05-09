using System.Runtime.InteropServices;

namespace System.Instant
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FigureAsAttribute : FigureAttribute
    {
        public int SizeConst;
        public UnmanagedType Value;

        public FigureAsAttribute(UnmanagedType unmanaged)
        {
            Value = unmanaged;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FigureAttribute : Attribute
    {
        public FigureAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FigureDisplayAttribute : FigureAttribute
    {
        public string Name;

        public FigureDisplayAttribute(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FigureIdentityAttribute : FigureAttribute
    {
        public bool IsAutoincrement = false;
        public short Order = 0;

        public FigureIdentityAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FigureKeyAttribute : FigureIdentityAttribute
    {
        public FigureKeyAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FigureLinkAttribute : FigureAttribute
    {
        public string LinkRubric;
        public string TargetName;
        public Type TargetType;

        public FigureLinkAttribute(string targetName, string linkRubric)
        {
            TargetName = targetName;
            LinkRubric = linkRubric;
        }

        public FigureLinkAttribute(Type targetType, string linkRubric)
        {
            TargetType = targetType;
            TargetName = targetType.Name;
            LinkRubric = linkRubric;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FigureRequiredAttribute : FigureAttribute
    {
        public FigureRequiredAttribute() { }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FigureSizeAttribute : FigureAttribute
    {
        public int SizeConst;
        public UnmanagedType Value;

        public FigureSizeAttribute(int size)
        {
            SizeConst = size;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class FigureTreatmentAttribute : FigureAttribute
    {
        public SummarizeOperand SummaryOperand = SummarizeOperand.None;

        public FigureTreatmentAttribute() { }
    }
}
