
// <copyright file="Dataline.cs" company="UltimatR.Core">
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>



/// <summary>
/// The Mathset namespace.
/// </summary>
namespace System.Instant.Mathset
{
    using System;

    /// <summary>
    /// Class Dataline.
    /// </summary>
    public class Dataline
    {
        /// <summary>
        /// The row count
        /// </summary>
        public int RowCount;
        /// <summary>
        /// The row offset
        /// </summary>
        public int RowOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dataline" /> class.
        /// </summary>
        public Dataline()
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Dataline" /> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public Dataline(IFigures table)
        {
            Data = table;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Dataline" /> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="rowOffset">The row offset.</param>
        /// <param name="rowCount">The row count.</param>
        public Dataline(IFigures table, int rowOffset, int rowCount)
        {
            RowCount = rowCount;
            RowOffset = rowOffset;
            Data = table;
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Double" /> with the specified rowid.
        /// </summary>
        /// <param name="rowid">The rowid.</param>
        /// <param name="cellid">The cellid.</param>
        /// <returns>System.Double.</returns>
        public double this[int rowid, int cellid]
        {
            get
            {
                return Convert.ToDouble(Data[rowid, cellid]);
            }
            set
            {
                Data[rowid, cellid] = value;
            }
        }

        /// <summary>
        /// The data
        /// </summary>
        public IFigures Data;

        /// <summary>
        /// The enabled
        /// </summary>
        public bool Enabled = false;
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        { get; set; }
    }
}
