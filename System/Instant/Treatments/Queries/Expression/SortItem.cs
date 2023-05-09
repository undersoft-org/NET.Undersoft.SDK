using System.Linq;
using System.Linq.Expressions;

namespace System.Instant.Treatments
{
    public class SortItem<TEntity>
    {
        private SortExpression<TEntity> sortExpression;

        #region Constructors

        public SortItem()
        {
        }
        public SortItem(Expression<Func<TEntity, object>> expressionItem, SortDirection direction = SortDirection.ASC)
        {
            ExpressionItem = expressionItem;
            Direction = direction;
        }
        public SortItem(MemberRubric sortedRubric, SortDirection direction = SortDirection.ASC)
        {
            Direction = direction;
            Rubric = sortedRubric;
            Property = Rubric.Name;
        }
        public SortItem(string rubricName, string direction = "Asc")
        {
            Property = rubricName;
            SortDirection sortDirection;
            Enum.TryParse(direction, true, out sortDirection);
            Direction = sortDirection;
        }
        public SortItem(SortItem item) : this(item.Property, item.Direction)
        {            
        }

        #endregion

        #region Properties

        public Expression<Func<TEntity, object>> ExpressionItem { get; set; }

        public SortDirection Direction { get; set; }

        public int Position { get; set; }

        public string Property { get; set; }

        public MemberRubric Rubric { get; set; }

        #endregion

        #region Methods

        public void Assign(SortExpression<TEntity> sortExpression)
        {
            var fe = sortExpression;
            this.sortExpression = fe;
            if(fe.Rubrics.TryGet(Property, out MemberRubric rubric))
            {
                Rubric = rubric;
                ExpressionItem = e => e.ValueOf(Property);
            }
        }

        public bool Compare(SortItem<TEntity> term)
        {
            if(Property != term.Property || Direction != term.Direction)
                return false;

            return true;
        }

        #endregion
    }

}
