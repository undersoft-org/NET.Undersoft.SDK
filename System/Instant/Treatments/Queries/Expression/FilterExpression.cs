using System;
using System.Collections.Generic;
using System.Globalization;
using System.Instant;
using System.Linq;
using System.Linq.Expressions;
using System.Uniques;

namespace System.Instant.Treatments
{
    public class FilterExpression<TEntity>
    {
        private NumberFormatInfo nfi = new NumberFormatInfo();
        private Sleeve sleeve;
        public IRubrics Rubrics;
        private Expression<Func<TEntity, bool>> filterExpression { get; set; }

        public IList<FilterItem<TEntity>> FilterItems { get; } = new List<FilterItem<TEntity>>();

        public FilterExpression()
        {
            nfi.NumberDecimalSeparator = ".";
            sleeve = SleeveFactory.Generate<TEntity>();
            Rubrics = sleeve.Rubrics;
        }
        public FilterExpression(params FilterItem<TEntity>[] filterItems) : this()
        {
            filterItems.ForEach(fi => Add(fi)).ToArray();
        }
        public FilterExpression(IEnumerable<FilterItem<TEntity>> filterItems) : this()
        {
            filterItems.ForEach(fi => Add(fi)).ToArray();
        }
        public FilterExpression(IEnumerable<FilterItem> filterItems) : this()
        {
            filterItems.ForEach(fi => Add(new FilterItem<TEntity>(fi))).ToArray();
        }

        public Expression<Func<TEntity, bool>> Create()
        {
            return Create(FilterItems);
        }
        public Expression<Func<TEntity, bool>> Create(IEnumerable<FilterItem<TEntity>> filterItems)
        {
            Expression<Func<TEntity, bool>> exps = null;
            //filterItems.ForEach(fi => Add(fi));
            filterExpression = null;
            LogicOperand logic = LogicOperand.And;
            foreach(FilterItem<TEntity> ft in FilterItems)
            {
                exps = null;
                if(ft.Operand != MathOperand.Contains)
                {
                    if(filterExpression != null)
                    {
                        if(logic != LogicOperand.Or)
                            filterExpression = filterExpression.And(ConvertItem(ft, exps));
                        else
                            filterExpression = filterExpression.Or(ConvertItem(ft, exps));
                    }
                    else
                        filterExpression = ConvertItem(ft, exps);
                    logic = ft.Logic;
                }
                else
                {
                    HashSet<int> list = new HashSet<int>((ft.Value.GetType() == typeof(string)) ? ft.Value.ToString().Split(';')
                                                         .Select(p => Convert.ChangeType(p, ft.Rubric.RubricType).GetHashCode()) :
                                                         (ft.Value.GetType() == typeof(List<object>)) ? ((List<object>)ft.Value)
                                                         .Select(p => Convert.ChangeType(p, ft.Rubric.RubricType).GetHashCode()) : null);

                    if(list != null && list.Count > 0)
                        exps = (r => list.Contains(r.ValueOf(ft.Rubric.RubricName).GetHashCode()));

                    if(filterExpression != null)
                        if(logic != LogicOperand.Or)
                            filterExpression = filterExpression.And(exps);
                        else
                            filterExpression = filterExpression.Or(exps);
                    else
                        filterExpression = exps;
                    logic = ft.Logic;
                }
            }
            return filterExpression;
        }
        public Expression<Func<TEntity, bool>> ConvertItem(FilterItem<TEntity> filterItem, Expression<Func<TEntity, bool>> expression = null)
        {
            var ft = filterItem;
            var ex = expression;
            if(ft.Value != null)
            {
                object value = filterItem.Value;
                MathOperand compare = ft.Operand;
                Type type = ft.Rubric.RubricType;
                bool isNumeric = type == typeof(IUnique) ||
                                 type == typeof(string) ||
                                 type == typeof(DateTime) ||
                                 type == typeof(Enum) ?
                                 false : true;


                if(compare != MathOperand.Like && compare != MathOperand.NotLike)
                {
                    switch(compare)
                    {
                        case MathOperand.Equal:

                            ex = (r => r.ValueOf(ft.Rubric.Name).Equals(value));
                            break;

                        case MathOperand.GreaterOrEqual:

                            ex = (r => r.ValueOf(ft.Rubric.Name) != null ? !isNumeric ?
                            r.ValueOf(ft.Rubric.Name).ComparableUInt64(ft.Rubric.RubricType)
                                                      >=
                            (value.ComparableUInt64(ft.Rubric.RubricType)) :
                            r.ValueOf(ft.Rubric.Name).ComparableDouble(ft.Rubric.RubricType)
                                                      >=
                            (value.ComparableDouble(ft.Rubric.RubricType)) : false);
                            break;

                        case MathOperand.Greater:

                            ex = (r => r.ValueOf(ft.Rubric.Name) != null ? !isNumeric ?
                            r.ValueOf(ft.Rubric.Name).ComparableUInt64(ft.Rubric.RubricType)
                                                     >
                            (value.ComparableUInt64(ft.Rubric.RubricType)) :
                            r.ValueOf(ft.Rubric.Name).ComparableDouble(ft.Rubric.RubricType)
                                                     >
                            (value.ComparableDouble(ft.Rubric.RubricType)) : false);
                            break;

                        case MathOperand.LessOrEqual:

                            ex = (r => r.ValueOf(ft.Rubric.Name) != null ? !isNumeric ?
                            r.ValueOf(ft.Rubric.Name).ComparableUInt64(ft.Rubric.RubricType)
                                                    <=
                            (value.ComparableUInt64(ft.Rubric.RubricType)) :
                            r.ValueOf(ft.Rubric.Name).ComparableDouble(ft.Rubric.RubricType)
                                                    <=
                            (value.ComparableDouble(ft.Rubric.RubricType)) : false);
                            break;

                        case MathOperand.Less:

                            ex = (r => r.ValueOf(ft.Rubric.Name) != null ? !isNumeric ?
                            r.ValueOf(ft.Rubric.Name).ComparableUInt64(ft.Rubric.RubricType)
                                                    <
                            (value.ComparableUInt64(ft.Rubric.RubricType)) :
                            r.ValueOf(ft.Rubric.Name).ComparableDouble(ft.Rubric.RubricType)
                                                    <
                            (value.ComparableDouble(ft.Rubric.RubricType)) : false);
                            break;
                        default:
                            break;
                    }
                }
                else if(compare != MathOperand.NotLike)

                    ex = (r => r.ValueOf(ft.Rubric.Name) != null 
                    ? Convert.ChangeType(r.ValueOf(ft.Rubric.Name), ft.Rubric.RubricType).ToString()
                        .Contains(Convert.ChangeType(value, ft.Rubric.RubricType).ToString()) 
                          : false);
                else
                    ex = (r => r.ValueOf(ft.Rubric.Name) != null 
                    ? !Convert.ChangeType(r.ValueOf(ft.Rubric.Name), ft.Rubric.RubricType).ToString()
                        .Contains(Convert.ChangeType(value, ft.Rubric.RubricType).ToString()) 
                          : false);
            }
            return ex;
        }

        public FilterItem<TEntity> Add(FilterItem<TEntity> item)
        {
            item.Assign(this);
            FilterItems.Add(item);
            return item;
        }
        public IEnumerable<FilterItem<TEntity>> Add(IEnumerable<FilterItem<TEntity>> filterItems)
        {
            filterItems.ForEach(fi => Add(fi));
            return FilterItems;
        }
    }
}