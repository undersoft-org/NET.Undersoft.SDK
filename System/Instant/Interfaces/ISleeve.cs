namespace System.Instant
{
    public interface ISleeve : IFigure
    {
        IRubrics Rubrics { get; set; }

        object Devisor { get; set; }
    }
}
