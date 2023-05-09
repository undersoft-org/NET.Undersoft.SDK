namespace System.Instant
{
    public interface IRubric : IMemberRubric, IUnique
    {
        short IdentityOrder { get; set; }

        bool IsAutoincrement { get; set; }

        bool IsDBNull { get; set; }

        bool IsExpandable { get; set; }

        bool IsIdentity { get; set; }

        bool IsKey { get; set; }

        bool IsUnique { get; set; }

        bool Required { get; set; }

        SummarizeOperand SummaryOperand { get; set; }

        int SummaryOrdinal { get; set; }

        IRubric SummaryRubric { get; set; }
    }

    public enum SummarizeOperand
    {
        None,
        Sum,
        Avg,
        Min,
        Max,
        Bis
    }
}
