namespace System.Instant
{
    using Uniques;

    public interface IFigure : IUnique
    {
        object this[string propertyName] { get; set; }

        object this[int fieldId] { get; set; }

        object[] ValueArray { get; set; }

        Uscn UniqueCode { get; set; }
    }
}
