namespace System.Instant
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    public class CompilerConstructors
    {
        protected readonly ConstructorInfo dataMemberCtor =
            typeof(DataMemberAttribute).GetConstructor(Type.EmptyTypes);
        protected readonly PropertyInfo[] dataMemberProps = new[]
        {
            typeof(DataMemberAttribute).GetProperty("Order"),
            typeof(DataMemberAttribute).GetProperty("Name")
        };
        protected readonly ConstructorInfo figureDisplayCtor =
            typeof(FigureDisplayAttribute).GetConstructor(new Type[] { typeof(string) });
        protected readonly ConstructorInfo figureIdentityCtor =
            typeof(FigureIdentityAttribute).GetConstructor(Type.EmptyTypes);
        protected readonly ConstructorInfo figureKeyCtor =
            typeof(FigureKeyAttribute).GetConstructor(Type.EmptyTypes);
        protected readonly ConstructorInfo keyCtor = typeof(KeyAttribute).GetConstructor(
            Type.EmptyTypes
        );
        protected readonly ConstructorInfo figureRequiredCtor =
            typeof(FigureRequiredAttribute).GetConstructor(Type.EmptyTypes);
        protected readonly ConstructorInfo figureLinkCtor =
            typeof(FigureLinkAttribute).GetConstructor(Type.EmptyTypes);
        protected readonly ConstructorInfo requiredCtor = typeof(RequiredAttribute).GetConstructor(
            Type.EmptyTypes
        );
        protected readonly ConstructorInfo figuresTreatmentCtor =
            typeof(FigureTreatmentAttribute).GetConstructor(Type.EmptyTypes);
        protected readonly ConstructorInfo marshalAsCtor =
            typeof(MarshalAsAttribute).GetConstructor(new Type[] { typeof(UnmanagedType) });
        protected readonly ConstructorInfo structLayoutCtor =
            typeof(StructLayoutAttribute).GetConstructor(new Type[] { typeof(LayoutKind) });
        protected readonly FieldInfo[] structLayoutFields = new[]
        {
            typeof(StructLayoutAttribute).GetField("CharSet"),
            typeof(StructLayoutAttribute).GetField("Pack")
        };
    }
}
