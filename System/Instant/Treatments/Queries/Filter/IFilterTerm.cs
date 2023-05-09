namespace System.Instant.Treatments
{
    public interface IFilterTerm
    {
        #region Properties

        LogicType Logic { get; set; }

        OperandType Operand { get; set; }

        string RubricName { get; set; }

        FilterStage Stage { get; set; }

        object Value { get; set; }

        #endregion
    }
}