namespace System.Instant
{
    public interface IInstant
    {
        Type BaseType { get; set; }

        string Name { get; set; }

        IRubrics Rubrics { get; }

        int Size { get; }

        Type Type { get; set; }

        object New();
    }
}
