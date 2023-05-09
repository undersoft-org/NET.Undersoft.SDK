namespace System.Instant
{
    public interface IValueProxy : IFigure
    {
        IRubrics Rubrics { get; }

        ISleeve Valuator { get; set; }
    }
}