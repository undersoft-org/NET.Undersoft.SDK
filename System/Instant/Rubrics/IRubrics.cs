using System.Series;

namespace System.Instant
{
    public interface IRubrics : IUnique, IDeck<MemberRubric>
    {
        int BinarySize { get; }

        int[] BinarySizes { get; }

        IFigures Figures { get; set; }

        IRubrics KeyRubrics { get; set; }

        FieldMappings Mappings { get; set; }

        int[] Ordinals { get; }

        byte[] GetBytes(IFigure figure);

        byte[] GetUniqueBytes(IFigure figure, uint seed = 0);

        ulong GetUniqueKey(IFigure figure, uint seed = 0);

        void SetUniqueKey(IFigure figure, uint seed = 0);

        void Update();
    }
}
