namespace System.Instant.Treatments
{
    #region Enums

    [Serializable]
    public enum MathOperand
    {
        None,
        Equal,
        GreaterOrEqual,
        LessOrEqual,
        Greater,
        Less,
        Like,
        NotLike,
        Contains       
    }
    [Serializable]
    public enum LogicOperand
    {
        And,
        Or
    }

    #endregion

    /// <summary>
    /// Defines the <see cref="FilterOperand" />.
    /// </summary>
    public static class FilterOperand
    {
        #region Methods

        /// <summary>
        /// The ParseOperand.
        /// </summary>
        /// <param name="operandString">The operandString<see cref="string"/>.</param>
        /// <returns>The <see cref="MathOperand"/>.</returns>
        public static MathOperand ParseMathOperand(string operandString)
        {
            MathOperand _operand = MathOperand.None;
            switch(operandString)
            {
                case "=":
                    _operand = MathOperand.Equal;
                    break;
                case ">=":
                    _operand = MathOperand.GreaterOrEqual;
                    break;
                case ">":
                    _operand = MathOperand.Greater;
                    break;
                case "<=":
                    _operand = MathOperand.LessOrEqual;
                    break;
                case "<":
                    _operand = MathOperand.Less;
                    break;
                case "%":
                    _operand = MathOperand.Like;
                    break;
                case "!%":
                    _operand = MathOperand.NotLike;
                    break;
                default:
                    _operand = MathOperand.None;
                    break;
            }
            return _operand;
        }

        /// <summary>
        /// The StringOperand.
        /// </summary>
        /// <param name="operand">The operand<see cref="MathOperand"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string ConvertMathOperand(MathOperand operand)
        {
            string operandString = "";
            switch(operand)
            {
                case MathOperand.Equal:
                    operandString = "=";
                    break;
                case MathOperand.GreaterOrEqual:
                    operandString = ">=";
                    break;
                case MathOperand.Greater:
                    operandString = ">";
                    break;
                case MathOperand.LessOrEqual:
                    operandString = "<=";
                    break;
                case MathOperand.Less:
                    operandString = "<";
                    break;
                case MathOperand.Like:
                    operandString = "%";
                    break;
                case MathOperand.NotLike:
                    operandString = "!%";
                    break;
                default:
                    operandString = "=";
                    break;
            }
            return operandString;
        }

        #endregion
    }


}