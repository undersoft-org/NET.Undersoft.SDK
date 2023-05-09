using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace System.Instant.Treatments
{
    public class SortExpression<TEntity>
    {
        private Sleeve sleeve;
        public IRubrics Rubrics;
        private Expression<Func<TEntity, object>> sortExpression { get; set; }

        public IList<SortItem<TEntity>> SortItems { get; } = new List<SortItem<TEntity>>();

        public SortExpression()
        {
            sleeve = SleeveFactory.Generate<TEntity>();
            Rubrics = sleeve.Rubrics;
        }
        public SortExpression(Expression<Func<TEntity, object>> expressionItem, SortDirection direction) : this()
        {
            Add(new SortItem<TEntity>(expressionItem, direction));
        }
        public SortExpression(params SortItem<TEntity>[] sortItems) : this()
        {
            sortItems.ForEach(fi => Add(fi));
        }
        public SortExpression(IEnumerable<SortItem<TEntity>> sortItems) : this()
        {
            sortItems.ForEach(fi => Add(fi));
        }
        public SortExpression(IEnumerable<SortItem> sortItems) : this()
        {
            sortItems.ForEach(fi => Add(new SortItem<TEntity>(fi))).ToList();
        }

        public IQueryable<TEntity> Sort(IQueryable<TEntity> query)
        {
            return Sort(query, SortItems);
        }
        public IQueryable<TEntity> Sort(IQueryable<TEntity> query, IEnumerable<SortItem<TEntity>> sortItems)
        {
          
            if(sortItems != null && sortItems.Any())
            {
                if(!SortItems.Any())
                    sortItems.ForEach(fi => Add(fi));

                bool first = true;
                IOrderedEnumerable<TEntity> orderedQuery = null;
                foreach(var sortItem in SortItems)
                {
                    if(sortItem.Direction.Equals(SortDirection.ASC))
                    {
                        orderedQuery = (first) ? query.OrderBy(sortItem.ExpressionItem.Compile()) : orderedQuery.ThenBy(sortItem.ExpressionItem.Compile());
                    }
                    else
                    {
                        orderedQuery = (first)
                            ? query.OrderByDescending(sortItem.ExpressionItem.Compile())
                            : orderedQuery.ThenByDescending(sortItem.ExpressionItem.Compile());
                    }

                    first = false;
                }

                return orderedQuery.AsQueryable();
            }
            else
            {
                return query;
            }
        }

        public SortItem<TEntity> Add(SortItem<TEntity> item)
        {
            item.Assign(this);
            SortItems.Add(item);
            return item;
        }
        public IEnumerable<SortItem<TEntity>> Add(IEnumerable<SortItem<TEntity>> sortItems)
        {
            sortItems.ForEach(fi => Add(fi));
            return SortItems;
        }
    }

    //public enum SortDirection
    //{
    //    Asc,
    //    Desc
    //}
}