namespace System.Instant.Treatments
{
    using System;

    [Serializable]
    public class FigureSort
    {
        #region Fields

        public IFigures Figures;
        public SortTerms Terms;

        #endregion

        #region Constructors

        public FigureSort(IFigures figures)
        {
            this.Figures = figures;
            Terms = new SortTerms(figures);
        }

        #endregion
    }
}