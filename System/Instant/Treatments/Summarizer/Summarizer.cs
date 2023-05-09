//-----------------------------------------------------------------------
// <copyright file="Summarizer.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------


namespace System.Instant.Treatments
{
    using System.Linq;


    public static class Summarizer
    {
        static IFigure Result(IFigures figures, bool onlyView = true)
        {
            IRubrics summaryRubrics = figures.Treatment.SummaryRubrics;
            if(summaryRubrics.Count > 0)
            {
                object[] result = summaryRubrics.AsValues()
                    .AsParallel()
                    .SelectMany(
                        s => new object[]
                        {
                            (!string.IsNullOrEmpty(s.RubricName))
                            ? ((s.SummaryOperand == SummarizeOperand.Sum)
                                ? Convert.ChangeType(
                                    figures.View

                                        .Sum

                                (
                                            j => (j[s.SummaryOrdinal] is DateTime)
                                                                ? ((DateTime)j[s.SummaryOrdinal]).ToOADate()
                                                                : Convert.ToDouble(j[s.FieldId])),
                                    typeof(object))
                                : ((s.SummaryOperand == SummarizeOperand.Min)
                                    ? Convert.ChangeType(
                                        figures.View

                                            .Min

                                (
                                                j => (j[s.SummaryOrdinal] is DateTime)
                                                                        ? ((DateTime)j[s.SummaryOrdinal]).ToOADate()
                                                                        : Convert.ToDouble(j[s.FieldId])),
                                        typeof(object))
                                    : ((s.SummaryOperand == SummarizeOperand.Max)
                                        ? Convert.ChangeType(
                                            figures.View

                                                .Max

                                (
                                                    j => (j[s.SummaryOrdinal] is DateTime)
                                                                                ? ((DateTime)j[s.SummaryOrdinal]).ToOADate(
                                                                                    )
                                                                                : Convert.ToDouble(j[s.FieldId])),
                                            typeof(object))
                                        : ((s.SummaryOperand == SummarizeOperand.Avg)
                                            ? Convert.ChangeType(
                                                figures.View

                                                    .Average

                               (
                                                        j => (j[s.SummaryOrdinal] is DateTime)
                                                                                        ? ((DateTime)j[s.SummaryOrdinal]).ToOADate(
                                                                                            )
                                                                                        : Convert.ToDouble(j[s.FieldId])),
                                                typeof(object))
                                            : ((s.SummaryOperand == SummarizeOperand.Bis)
                                                ? Convert.ChangeType(
                                                    figures.View
                                                        .Select(
                                                            j => (j[s.FieldId] != DBNull.Value)
                                                                                                ? j[s.FieldId].ToString(
                                                                                                    )
                                                                                                : string.Empty)

                                                        .Aggregate((x, y) => $"{x} {y}"),
                                                    typeof(object))
                                                : null)))))
                            : null
                        })
                    .ToArray();

                figures.Summary.ValueArray = result;

                return figures.Summary;
            } else
                return null;
        }


        public static IFigure Summarize(this IFigures figures, bool onlyView = false)
        { return Result(figures, onlyView); }
    }
}
