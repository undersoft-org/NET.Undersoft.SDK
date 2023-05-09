
// <copyright file="ILogs.cs" company="UltimatR.Core">
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>



/// <summary>
/// The System namespace.
/// </summary>
namespace System
{



    /// <summary>
    /// Interface ILogs
    /// </summary>
    public interface ILogs
    {
        #region Methods







        /// <summary>
        /// Writes the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="information">The information.</param>
        void Write(int logLevel, Exception exception, string information = null);






        /// <summary>
        /// Writes the specified log level.
        /// </summary>
        /// <param name="logLevel">The log level.</param>
        /// <param name="information">The information.</param>
        void Write(int logLevel, String information);

        #endregion
    }
}
