//-----------------------------------------------------------------------
// <copyright file="Aggregator.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;

namespace System.Instant.Treatments
{
    public static class Aggregator
    {
    }
    [Serializable]
    public enum AggregateOperand
    {
        None,
        Sum,
        Avg,
        Min,
        Max,
        Bis,
        First,
        Last,
        Bind,
        Count,
        Default
    }
}
