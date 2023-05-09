
// <copyright file="FigureFilter.cs" company="UltimatR.Core">
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>



/// <summary>
/// The Treatments namespace.
/// </summary>
namespace System.Instant.Treatments
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;




    /// <summary>
    /// Class FigureFilter.
    /// </summary>
    [Serializable]
    public class FigureFilter
    {
        #region Fields

        /// <summary>
        /// The evaluator
        /// </summary>
        [NonSerialized] public Func<IFigure, bool> Evaluator;
        /// <summary>
        /// The expression
        /// </summary>
        [NonSerialized] private QueryExpression expression;
        /// <summary>
        /// The figures
        /// </summary>
        [NonSerialized] private IFigures figures;
        /// <summary>
        /// The terms buffer
        /// </summary>
        [NonSerialized] private FilterTerms termsBuffer;
        /// <summary>
        /// The terms reducer
        /// </summary>
        [NonSerialized] private FilterTerms termsReducer;

        #endregion

        #region Constructors





        /// <summary>
        /// Initializes a new instance of the <see cref="FigureFilter" /> class.
        /// </summary>
        /// <param name="figures">The figures.</param>
        public FigureFilter(IFigures figures)
        {
            this.figures = figures;
            expression = new QueryExpression();
            Reducer = new FilterTerms(figures);
            Terms = new FilterTerms(figures);
            termsBuffer = expression.Conditions;
            termsReducer = new FilterTerms(figures);
        }

        #endregion

        #region Properties




        /// <summary>
        /// Gets or sets the figures.
        /// </summary>
        /// <value>The figures.</value>
        public IFigures Figures
        {
            get { return figures; }
            set { figures = value; }
        }




        /// <summary>
        /// Gets or sets the reducer.
        /// </summary>
        /// <value>The reducer.</value>
        public FilterTerms Reducer { get; set; }




        /// <summary>
        /// Gets or sets the terms.
        /// </summary>
        /// <value>The terms.</value>
        public FilterTerms Terms { get; set; }

        #endregion

        #region Methods






        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <param name="stage">The stage.</param>
        /// <returns>Expression&lt;Func&lt;IFigure, System.Boolean&gt;&gt;.</returns>
        public Expression<Func<IFigure, bool>> GetExpression(int stage = 1)
        {
            termsReducer.Clear();
            termsReducer.Add(Reducer.AsEnumerable().Concat(Terms.AsEnumerable()).ToArray());
            expression.Conditions = termsReducer;
            termsBuffer = termsReducer;
            return expression.CreateExpression(stage);
        }







        /// <summary>
        /// Queries the specified to query.
        /// </summary>
        /// <param name="toQuery">To query.</param>
        /// <param name="stage">The stage.</param>
        /// <returns>IFigure[].</returns>
        public IFigure[] Query(ICollection<IFigure> toQuery, int stage = 1)
        {
            termsReducer.Clear();
            termsReducer.Add(Reducer.AsEnumerable().Concat(Terms.AsEnumerable()).ToArray());
            expression.Conditions = termsReducer;
            termsBuffer = termsReducer;
            return toQuery.AsQueryable().Where(expression.CreateExpression(stage).Compile()).ToArray();
        }






        /// <summary>
        /// Queries the specified stage.
        /// </summary>
        /// <param name="stage">The stage.</param>
        /// <returns>IFigure[].</returns>
        public IFigure[] Query(int stage = 1)
        {
            termsReducer.Clear();
            termsReducer.Add(Reducer.AsEnumerable().Concat(Terms.AsEnumerable()).ToArray());
            expression.Conditions = termsReducer;
            termsBuffer = termsReducer;
            return figures.AsEnumerable().AsQueryable().Where(expression.CreateExpression(stage).Compile()).ToArray();
        }

        #endregion
    }
}
