using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace System.Instant.Treatments
{
    [Serializable]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public class FilterItem<TEntity> : ICloneable
    {
        #region Fields

        public string typeName;
        [NonSerialized] private Type type;

        private FilterExpression<TEntity> filterExpression;

        #endregion

        #region Constructors

        public FilterItem()
        {
        }    
        public FilterItem(Expression<Func<TEntity, bool>> expressionItem, LogicOperand linkOperand = LogicOperand.And)
        {
            ExpressionItem = expressionItem;
            Logic = linkOperand;
        }
        public FilterItem(MemberRubric rubric, MathOperand compareOperand, object compareValue, LogicOperand linkOperand = LogicOperand.And)
        {
            Property = rubric.Name;
            Operand = compareOperand;
            Value = compareValue;
            Logic = linkOperand;
            PropertyType = rubric.RubricType;
            Rubric = rubric;
        }
        public FilterItem(string propertyName, MathOperand compareOperand, object compareValue, LogicOperand linkOperand = LogicOperand.And)
        {
            Property = propertyName;
            Operand = compareOperand;
            Value = compareValue;
            Logic = linkOperand;
        }
        public FilterItem(string propertyName, string compareOperand, object compareValue, string linkLogic = "And")
        {
            Property = propertyName;
            Enum.TryParse(compareOperand, true, out MathOperand tempOperand);
            if(tempOperand == MathOperand.None)
                tempOperand = FilterOperand.ParseMathOperand(compareOperand);
            Operand = tempOperand;
            Value = compareValue;
            Enum.TryParse(linkLogic, true, out LogicOperand tempLogic);
            Logic = tempLogic;
        }
        public FilterItem(FilterItem item) : this(item.Property, item.Operand, item.Value, item.Logic)
        {
        }

        #endregion

        #region Properties
        [JsonIgnore]
        public Expression<Func<TEntity, bool>> ExpressionItem { get; set; }
        [JsonIgnore]
        public MemberRubric Rubric { get; set; }

        public string Property { get; set; }

        [JsonIgnore]
        public Type PropertyType
        {
            get
            {
                if(type == null && typeName != null)
                    type = Type.GetType(typeName);
                return type;
            }
            set
            {
                type = value;
                typeName = value.FullName;
            }
        }
            
        public MathOperand Operand { get; set; }

        public object Value { get; set; }

        public LogicOperand Logic { get; set; }

        #endregion

        #region Methods

        public void Assign(FilterExpression<TEntity> filterExpression)
        {
            var fe = filterExpression;
            this.filterExpression = fe;
            if(fe.Rubrics.TryGet(Property, out MemberRubric rubric))
            {
                Rubric = rubric;
                PropertyType = rubric.RubricType;
                ExpressionItem = fe.ConvertItem(this);
            }
        }

        public object Clone()
        {
            FilterItem<TEntity> clone = (FilterItem<TEntity>)this.MemberwiseClone();
            clone.Rubric = Rubric;
            return clone;
        }

        public FilterItem<TEntity> Clone(object value)
        {
            FilterItem<TEntity> clone = (FilterItem<TEntity>)this.MemberwiseClone();
            clone.Rubric = Rubric;
            clone.Value = value;
            return clone;
        }

        public bool Compare(FilterItem<TEntity> term)
        {
            if(Property != term.Property)
                return false;
            if(!Value.Equals(term.Value))
                return false;
            if(!Operand.Equals(term.Operand))
                return false;
            if(!Logic.Equals(term.Logic))
                return false;

            return true;
        }

        #endregion
    }
}
