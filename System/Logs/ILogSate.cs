
// <copyright file="ILogSate.cs" company="UltimatR.Core">
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>



/// <summary>
/// The Logs namespace.
/// </summary>
namespace System.Logs
{
    /// <summary>
    /// Interface ILogSate
    /// </summary>
    public interface ILogSate
    {
        /// <summary>
        /// Gets or sets the data object.
        /// </summary>
        /// <value>The data object.</value>
        object DataObject { get; set; }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        Exception Exception { get; set; }
    }

    /// <summary>
    /// Interface ILogSate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogSate<T>
    {
        /// <summary>
        /// Gets or sets the data object.
        /// </summary>
        /// <value>The data object.</value>
        T DataObject { get; set; }
        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        Exception Exception { get; set; }
    }
}