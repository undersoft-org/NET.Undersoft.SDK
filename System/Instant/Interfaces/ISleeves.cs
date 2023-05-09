namespace System.Instant
{
    public interface ISleeves : IFigures
    {
        IFigures Figures { get; set; }

        new IInstant Instant { get; set; }

        IFigures Sleeves { get; set; }
    }
}
