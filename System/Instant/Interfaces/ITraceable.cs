namespace System.Instant
{
    public interface ITraceable
    {
        IVariety Variator { get; }

        IDeputy NoticeChange { get; }

        IDeputy NoticeChanging { get; }
    }
}
